using UnityEngine;

namespace Avrahamy.Audio {
    public class GetAudioSourceMessage {
        private static readonly GetAudioSourceMessage instance = new GetAudioSourceMessage();
        private AudioSource source;

        public AudioSource AudioSource {
            get {
                return source;
            }
            set {
                source = value;
            }
        }

        public static GetAudioSourceMessage Instance {
            get {
                instance.source = null;
                return instance;
            }
        }
    }
}
