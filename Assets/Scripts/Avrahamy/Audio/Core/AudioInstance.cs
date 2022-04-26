namespace Avrahamy.Audio {
    /// <summary>
    /// A handle to a managed audio track.
    /// </summary>
    public class AudioInstance {
        private AudioTrack track;

        public float Volume {
            get {
                return track?.Volume ?? 0f;
            }
            set {
                if (track == null) return;
                track.Volume = value;
            }
        }

        public bool IsPlaying {
            get {
                if (track == null) return false;
                return track.IsPlaying;
            }
        }

        public float Time {
            get {
                return track?.Time ?? 0f;
            }
            set {
                if (track == null) return;
                track.Time = value;
            }
        }

        public AudioInstance(AudioTrack track) {
            this.track = track;
        }

        public AudioInstance _Reuse(AudioTrack track) {
            this.track = track;
            return this;
        }

        public void PlayFromStart() {
            track.SetTime(0f);
        }

        public void Stop(float fadeOutTime = 0f) {
            if (track == null) return;
            track.Stop(fadeOutTime);
            track = null;
        }

        public void ChangeVolume(float fromVolume, float targetVolume, float duration) {
            track?.ChangeVolume(fromVolume, targetVolume, duration);
        }

        public void ChangeVolume(float targetVolume, float duration) {
            track?.ChangeVolume(Volume, targetVolume, duration);
        }
    }
}
