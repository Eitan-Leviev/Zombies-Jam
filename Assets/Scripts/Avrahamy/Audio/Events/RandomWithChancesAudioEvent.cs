using UnityEngine;
using System;
using Avrahamy.Math;

namespace Avrahamy.Audio {
    /// <summary>
    /// Plays a random audio event out of a list of weighted audio events.
    /// </summary>
    [CreateAssetMenu(menuName = "Avrahamy/Audio/Events/Random With Chances")]
    public class RandomWithChancesAudioEvent : AudioEvent {
        [Serializable]
        public class CompositeEntry : RandomUtils.ValueWithChance<AudioEvent> {}

        [SerializeField] CompositeEntry[] entries;

        public override float Volume {
            get {
                return entries[0].value.Volume;
            }
        }

        public override void Play(AudioSource source) {
            var audioEvent = entries.ChooseRandomWithChances();
            audioEvent.Play(source);
        }
    }
}
