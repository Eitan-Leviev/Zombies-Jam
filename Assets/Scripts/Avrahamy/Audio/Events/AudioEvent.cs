using UnityEngine;
using Avrahamy.Messages;

namespace Avrahamy.Audio {
    /// <summary>
    /// Base class for playing an audio clip on an AudioSource.
    /// Provides an implementation to request an audio source from the
    /// AudioController.
    /// </summary>
    public abstract class AudioEvent : ScriptableObject {
        public abstract float Volume { get; }

        /// <summary>
        /// Play the audio using the AudioController.
        /// </summary>
        public void Play() {
            var message = GetAudioSourceMessage.Instance;
            GlobalMessagesHub.Instance.Dispatch(message);
            var source = message.AudioSource;
            if (source != null) {
                source.spatialize = false;
                source.spatialBlend = 0f;
                Play(source);
            }
        }

        public void Play(Transform position) {
            Play(position.position);
        }

        /// <summary>
        /// Play the audio using the AudioController at a certain position.
        /// </summary>
        public void Play(Vector3 position) {
            var message = GetAudioSourceMessage.Instance;
            GlobalMessagesHub.Instance.Dispatch(message);
            var source = message.AudioSource;
            if (source != null) {
                source.spatialize = true;
                source.spatialBlend = 1f;
                source.transform.position = position;
                Play(source);
            }
        }

        /// <summary>
        /// Play the audio through the given AudioSource.
        /// </summary>
        public abstract void Play(AudioSource source);
    }
}
