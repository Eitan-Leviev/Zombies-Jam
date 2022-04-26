using UnityEngine;
using System;
using Avrahamy.Messages;
using Avrahamy.Utils;

namespace Avrahamy.Audio {
    /// <summary>
    /// Plays multiple audio events at once with an optional delay.
    /// </summary>
    [CreateAssetMenu(menuName = "Avrahamy/Audio/Events/Composite (Multiple)")]
    public class CompositeAudioEvent : AudioEvent {
        [Serializable]
        public struct CompositeEntry {
            public float delay;
            public AudioEvent audioEvent;
        }

        [SerializeField] AudioEvent firstEvent;
        [SerializeField] CompositeEntry[] additionalEvents;

        public override float Volume {
            get {
                return firstEvent.Volume;
            }
        }

        public override void Play(AudioSource source) {
            firstEvent.Play(source);
            foreach (var audioEvent in additionalEvents) {
                if (audioEvent.delay <= 0f) {
                    Play(audioEvent);
                }
                PureCoroutines.DelaySeconds(() => {
                    Play(audioEvent);
                }, audioEvent.delay);
            }
        }

        private void Play(CompositeEntry audioEvent) {
            var message = GetAudioSourceMessage.Instance;
            GlobalMessagesHub.Instance.Dispatch(message);
            var source = message.AudioSource;
            if (source != null) {
                audioEvent.audioEvent.Play(source);
            }
        }
    }
}
