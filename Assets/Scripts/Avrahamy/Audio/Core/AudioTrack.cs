using UnityEngine;

namespace Avrahamy.Audio {
    /// <summary>
    /// Wraps an AudioSource and allows manipulating it over time.
    /// </summary>
    public class AudioTrack {
        public const float PLAY_TO_THE_END = -1f;

        private readonly AudioSource source;

        private AudioEvent audioEvent;

        // This is the time to stop playing in Time.time.
        private float stopTime;

        private float volumeChangeStartValue;
        private float volumeChangeEndValue;
        private float volumeChangeStartTime = -1f;
        private float volumeChangeEndTime;

        private bool isLooping;

        public AudioEvent AudioEvent {
            get {
                return audioEvent;
            }
        }

        public float Volume {
            get {
                return source.volume / Mathf.Max(audioEvent.Volume, 0.01f);
            }
            set {
                source.volume = value * audioEvent.Volume;
            }
        }

        public bool IsPlaying {
            get {
                return source.isPlaying;
            }
        }

        public float Time {
            get {
                return source.time;
            }
            set {
                source.time = value;
            }
        }

        public AudioTrack(AudioSource source) {
            this.source = source;
        }

        public void Play(AudioEvent audioEvent, float startTime, float endTime, float fadeInDuration, bool isLooping) {
            this.audioEvent = audioEvent;
            source.spatialize = false;
            source.spatialBlend = 0f;
            audioEvent.Play(source);
            if (startTime > 0f) {
                source.time = startTime;
            }
            if (endTime > 0f && source.clip.length < endTime) {
                var delay = endTime - startTime;
                stopTime = UnityEngine.Time.time + delay;
            } else {
                stopTime = -1f;
            }

            if (fadeInDuration > 0f) {
                ChangeVolume(0f, 1f, fadeInDuration);
            } else {
                volumeChangeEndTime = -1f;
            }

            this.isLooping = isLooping;
            source.loop = this.isLooping;
        }

        public void SetTime(float time) {
            source.time = time;
        }

        public void Stop(float fadeOutTime = 0f) {
            if (fadeOutTime <= 0.0001f) {
                isLooping = false;
                stopTime = UnityEngine.Time.time;
            } else {
                isLooping = false;

                ChangeVolume(Volume, 0f, fadeOutTime);
                stopTime = volumeChangeEndTime;
            }
        }

        public void ChangeVolume(float fromVolume, float targetVolume, float duration) {
            volumeChangeStartValue = fromVolume * audioEvent.Volume;
            volumeChangeEndValue = targetVolume * audioEvent.Volume;
            volumeChangeStartTime = UnityEngine.Time.time;
            volumeChangeEndTime = volumeChangeStartTime + duration;
        }

        public void Update(float now) {
            if (!isLooping && 0f < stopTime && stopTime <= now) {
                source.Stop();
                return;
            }
            if (volumeChangeEndTime < 0f) return;
            if (volumeChangeEndTime <= now) {
                // Volume change ended.
                volumeChangeEndTime = -1f;
                source.volume = volumeChangeEndValue;
            } else if (volumeChangeStartTime >= 0f) {
                // Volume change in progress.
                var t = Mathf.InverseLerp(volumeChangeStartTime, volumeChangeEndTime, now);
                source.volume = Mathf.Lerp(volumeChangeStartValue, volumeChangeEndValue, t);
            }
        }
    }
}
