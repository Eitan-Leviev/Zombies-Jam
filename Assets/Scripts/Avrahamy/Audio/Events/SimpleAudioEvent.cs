using UnityEngine;
using UnityEngine.Audio;
using Avrahamy.EditorGadgets;
using Avrahamy.Math;

namespace Avrahamy.Audio {
    [CreateAssetMenu(menuName = "Avrahamy/Audio/Events/Simple")]
    public class SimpleAudioEvent : AudioEvent {
        [SerializeField] AudioClip clip;
        [SerializeField] AudioMixerGroup mixerGroup;
        [MinMaxRange(0f, 2f)]
        [SerializeField] FloatRange volume = new FloatRange(1f);
        [MinMaxRange(0.1f, 3f)]
        [SerializeField] FloatRange pitch = new FloatRange(1f);
        [Range(-1f, 1f)]
        [SerializeField] float stereoPan;

        public override float Volume {
            get {
                return RandomUtils.Range(volume);
            }
        }

        public override void Play(AudioSource source) {
            source.clip = clip;
            source.outputAudioMixerGroup = mixerGroup;
            source.volume = Volume;
            source.pitch = RandomUtils.Range(pitch);
            source.panStereo = stereoPan;
            source.Play();
        }
    }
}
