using UnityEngine;
using Avrahamy.Messages;

namespace Avrahamy.Audio {
    public class RequestPlayManagedAudioMessageGeneratorComponent : MessageGeneratorComponent {
        [SerializeField] AudioEvent audioEvent;
        [SerializeField] bool isLooping;
        [SerializeField] float fadeInDuration;

        private AudioInstance musicHandler;

        protected void OnDestroy() {
            if (musicHandler == null) return;
            musicHandler.Stop();
            musicHandler = null;
        }

        public override object New() {
            return new RequestPlayManagedAudioMessage(audioEvent, isLooping, fadeInDuration);
        }

        public override void Dispatch() {
            var message = New() as RequestPlayManagedAudioMessage;
            GlobalMessagesHub.Instance.Dispatch(message);
            musicHandler = message.PlayedAudioInstance;
        }

        public void Stop(float fadeOutTime = 0f) {
            if (musicHandler == null) return;
            musicHandler.Stop(fadeOutTime);
        }

        public void SetVolume(float volume) {
            if (musicHandler == null) return;
            musicHandler.Volume = volume;
        }
    }
}