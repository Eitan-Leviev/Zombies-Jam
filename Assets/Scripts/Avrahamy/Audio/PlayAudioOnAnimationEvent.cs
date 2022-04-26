using UnityEngine;
using Avrahamy;
using Avrahamy.Audio;

namespace Product {
    /// <summary>
    /// Use this component with an Animator to play an audio event on an animation
    /// event.
    /// </summary>
    public class PlayAudioOnAnimationEvent : OptimizedBehaviour {
        [SerializeField] AudioEvent[] audioEvents;
        [SerializeField] bool playIn3DSpace;

        public void AnimEvent_PlayAudio(int audioIndex) {
            DebugAssert.Assert(
                0 <= audioIndex && audioIndex < audioEvents.Length,
                $"{this} attempt to play audio at index {audioIndex} when there are only {audioEvents.Length} audio events");
            var audioEvent = audioEvents[audioIndex];
            if (audioEvent == null) {
                DebugLog.LogWarning(LogTag.Audio, $"{this} has null audio event!");
                return;
            }

            if (playIn3DSpace) {
                audioEvent.Play(transform.position);
            } else {
                audioEvent.Play();
            }
        }
    }
}
