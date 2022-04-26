using UnityEngine;
using UnityEngine.Audio;
using Avrahamy.EditorGadgets;
using Avrahamy.Math;

namespace Avrahamy.Audio {
    /// <summary>
    /// Plays a random audio event with random volume, pitch and stereo pan.
    /// </summary>
    [CreateAssetMenu(menuName = "Avrahamy/Audio/Events/Sound Pool")]
    public class SoundPoolAudioEvent : AudioEvent {
        [SerializeField] AudioClip[] clips;
        [SerializeField] AudioMixerGroup mixerGroup;
        [MinMaxRange(0f, 2f)]
        [SerializeField] FloatRange volume = new FloatRange(1f);
        [MinMaxRange(0f, 2f)]
        [SerializeField] FloatRange pitch = new FloatRange(1f);
        [MinMaxRange(-1f, 1f)]
        [SerializeField] FloatRange stereoPan;

        public override float Volume {
            get {
                return RandomUtils.Range(volume);
            }
        }

        public override void Play(AudioSource source) {
            if (clips.Length == 0) return;

            source.clip = clips[Random.Range(0, clips.Length)];
            source.outputAudioMixerGroup = mixerGroup;
            source.volume = Volume;
            source.pitch = RandomUtils.Range(pitch);
            source.panStereo = RandomUtils.Range(stereoPan);
            source.Play();
        }
    }
}
