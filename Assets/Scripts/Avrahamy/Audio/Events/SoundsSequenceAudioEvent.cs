using UnityEngine;
using UnityEngine.Audio;

namespace Avrahamy.Audio {
    /// <summary>
    /// Plays audio clips one after the other each time.
    /// </summary>
    [CreateAssetMenu(menuName = "Avrahamy/Audio/Events/Sounds Sequence")]
    public class SoundsSequenceAudioEvent : AudioEvent {
        [SerializeField] int nextClipIndex;
        [SerializeField] AudioClip[] clips;
        [SerializeField] AudioMixerGroup mixerGroup;
        [Range(0f, 10f)]
        [SerializeField] float volume = 1f;
        [Range(0f, 2f)]
        [SerializeField] float pitch = 1f;
        [Range(-1f, 1f)]
        [SerializeField] float stereoPan;

        public override float Volume {
            get {
                return volume;
            }
        }

        public override void Play(AudioSource source) {
            source.clip = clips[nextClipIndex++];
            if (nextClipIndex == clips.Length) {
                nextClipIndex = 0;
            }
            source.outputAudioMixerGroup = mixerGroup;
            source.volume = volume;
            source.pitch = pitch;
            source.panStereo = stereoPan;
            source.Play();
        }
    }
}
