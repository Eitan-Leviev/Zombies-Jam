using UnityEngine;
using System.Collections.Generic;
using Avrahamy.Collections;
using Avrahamy.Messages;

namespace Avrahamy.Audio {
    public class AudioController : MonoBehaviour, IMessageHandler {
        [SerializeField] GameObject _soundsSources;

        private static AudioController instance;
        // Contains all of the sound sources available for the AudioController.
        // The first few are managed AudioTracks that can have their volume lerped
        // over time, are trimmed or looping. The rest are unmanaged - they play
        // and are done when they are done. AKA simple audio sources.
        private List<AudioSource> audioSources;
        // The next index to check when looking for an available simple audio source.
        private int nextSimpleAudioSourceIndex;
        // managedTracks.Count is the first index in audioSources of the simple sound sources.
        private List<AudioTrack> managedTracks;
        private List<AudioInstance> audioInstances;

        protected void Awake() {
            if (instance != null) {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(this);

            audioSources = new List<AudioSource>(_soundsSources.GetComponentsInChildren<AudioSource>());
            var simpleSourcesStartIndex = Mathf.Min(audioSources.Count, 1);
            nextSimpleAudioSourceIndex = simpleSourcesStartIndex;
            managedTracks = new List<AudioTrack>();
            audioInstances = new List<AudioInstance>();
            for (int i = 0; i < simpleSourcesStartIndex; i++) {
                var track = new AudioTrack(audioSources[i]);
                managedTracks.Add(track);
                audioInstances.Add(new AudioInstance(track));
            }

            foreach (var soundsSource in audioSources) {
                soundsSource.loop = false;
            }

            GlobalMessagesHub.Instance.Subscribe<GetAudioSourceMessage>(this);
            GlobalMessagesHub.Instance.Subscribe<RequestPlayManagedAudioMessage>(this);
        }

        protected void OnDisable() {
            if (audioInstances == null) return;
            // Clear audio instances so they won't null ref.
            foreach (var audioInstance in audioInstances) {
                audioInstance._Reuse(null);
            }
        }

        protected void OnDestroy() {
            GlobalMessagesHub.Instance.Unsubscribe<GetAudioSourceMessage>(this);
            GlobalMessagesHub.Instance.Unsubscribe<RequestPlayManagedAudioMessage>(this);
        }

        public void OnMessage(object message) {
            var messageType = message.GetType();

            if (messageType == typeof(GetAudioSourceMessage)) {
                var getAudioSourceMessage = message as GetAudioSourceMessage;
                getAudioSourceMessage.AudioSource = GetAvailableAudioSource();
                return;
            }
            if (messageType == typeof(RequestPlayManagedAudioMessage)) {
                var requestPlayMessage = message as RequestPlayManagedAudioMessage;
                requestPlayMessage.PlayedAudioInstance = PlayManagedAudio(
                    requestPlayMessage.Event,
                    requestPlayMessage.StartTime,
                    requestPlayMessage.EndTime,
                    requestPlayMessage.FadeInDuration,
                    requestPlayMessage.IsLooping);
            }
        }

        protected void Update() {
            var now = Time.time;
            foreach (var track in managedTracks) {
                if (track.IsPlaying) {
                    track.Update(now);
                }
            }
        }

        private AudioInstance PlayManagedAudio(AudioEvent audioEvent, float startTime, float endTime, float fadeInDuration, bool isLooping) {
            // Find an available AudioTrack.
            AudioTrack availableTrack = null;
            int trackIndex = 0;
            var simpleSourcesStartIndex = managedTracks.Count;
            for (int i = 0; i < simpleSourcesStartIndex; i++) {
                var track = managedTracks[i];
                if (track.IsPlaying) continue;
                availableTrack = track;
                trackIndex = i;
                break;
            }

            if (availableTrack == null) {
                // No available managed track. Allocate a new managed AudioTrack.
                var audioSourceIndex = GetAvailableAudioSourceIndex();
                var availableAudioSource = audioSources[audioSourceIndex];
                if (audioSourceIndex != simpleSourcesStartIndex) {
                    // Swap the audio sources to the available audio source is at
                    // managedTracks.Count.
                    audioSources[audioSourceIndex] = audioSources[simpleSourcesStartIndex];
                    audioSources[simpleSourcesStartIndex] = availableAudioSource;
                }
                availableTrack = new AudioTrack(availableAudioSource);
                trackIndex = audioInstances.Count;
                managedTracks.Add(availableTrack);
                audioInstances.Add(new AudioInstance(availableTrack));
                DebugAssert.Assert(managedTracks.Count == audioInstances.Count);
            }

            availableTrack.Play(audioEvent, startTime, endTime, fadeInDuration, isLooping);
            return audioInstances[trackIndex]._Reuse(availableTrack);
        }

        private AudioSource GetAvailableAudioSource() {
            var index = GetAvailableAudioSourceIndex();
            var soundSource = audioSources[index];
            soundSource.spatialize = false;
            soundSource.spatialBlend = 0f;
            return soundSource;
        }

        private int GetAvailableAudioSourceIndex() {
            // Look for the next available sound source.
            for (int i = nextSimpleAudioSourceIndex; i < audioSources.Count; i++) {
                var soundSource = audioSources[i];
                if (!soundSource.isPlaying) {
                    nextSimpleAudioSourceIndex = i;
                    return nextSimpleAudioSourceIndex;
                }
            }
            for (int i = managedTracks.Count; i < nextSimpleAudioSourceIndex; i++) {
                var soundSource = audioSources[i];
                if (!soundSource.isPlaying) {
                    nextSimpleAudioSourceIndex = i;
                    return nextSimpleAudioSourceIndex;
                }
            }

            // No available sources.
            var sourcesToAdd = Mathf.Min(2, 20 - audioSources.Count);
            if (sourcesToAdd <= 0) {
                // Can't add new sources. Look for the most completed next source
                // and replace it.
                float maxProgress = 0f;
                for (int j = nextSimpleAudioSourceIndex + 1; j <= 5; j++) {
                    var i = j % audioSources.Count;
                    var soundSource = audioSources[i];
                    var clip = soundSource.clip;
                    var duration = clip == null ? 0f : clip.length;
                    if (duration <= 0) {
                        nextSimpleAudioSourceIndex = i;
                        return nextSimpleAudioSourceIndex;
                    }
                    var progress = soundSource.time / duration;
                    if (progress > maxProgress) {
                        maxProgress = progress;
                        nextSimpleAudioSourceIndex = i;
                    }
                }
                return nextSimpleAudioSourceIndex;
            }

            // All sound sources are being used. Create some new sources.
            nextSimpleAudioSourceIndex = audioSources.Count;
            var prototypeSource = audioSources[0].gameObject;
            var parentTransform = prototypeSource.transform.parent;
            for (int i = 0; i < sourcesToAdd; i++) {
                var obj = Instantiate(prototypeSource, parentTransform, false);
                obj.name = "Source" + (nextSimpleAudioSourceIndex + i + 1);
                var newSource = obj.GetComponent<AudioSource>();
                newSource.clip = null;
                newSource.playOnAwake = false;
                newSource.loop = false;
                audioSources.Add(newSource);
            }

            return nextSimpleAudioSourceIndex;
        }
    }
}
