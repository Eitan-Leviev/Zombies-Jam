using UnityEngine;

namespace Avrahamy.Audio {
   public class PlayAudioOnEnable : OptimizedBehaviour {
        [SerializeField] AudioEvent audioEvent;
        [SerializeField] bool playIn3DSpace;

        protected void OnEnable() {
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
