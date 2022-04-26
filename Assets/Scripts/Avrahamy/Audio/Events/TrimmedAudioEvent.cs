using UnityEngine;
using Avrahamy.EditorGadgets;
using Avrahamy.Math;

namespace Avrahamy.Audio {
    [CreateAssetMenu(menuName = "Avrahamy/Audio/Events/Trimmed")]
    public class TrimmedAudioEvent : SimpleAudioEvent {
        [SerializeField] bool isTrimmed = true;
        [ConditionalHide("isTrimmed")]
        [SerializeField] public FloatRange playSegmentInSeconds;

        public override void Play(AudioSource source) {
            base.Play(source);
            if (!isTrimmed) return;

            source.time = playSegmentInSeconds.min;
            var delay = playSegmentInSeconds.max - playSegmentInSeconds.min;
            PureCoroutines.DelaySeconds(source.Stop, delay);
        }
    }
}
