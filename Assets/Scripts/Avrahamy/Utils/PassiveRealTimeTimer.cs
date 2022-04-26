using UnityEngine;
using System;

namespace Avrahamy {
    [Serializable]
    public class PassiveRealTimeTimer : ITimer {
        [SerializeField] float duration;

        public float StartTime {
            get {
                return endTime - Duration;
            }
            set {
                endTime = value + Duration;
            }
        }

        public float EndTime {
            get {
                return endTime;
            }
            set {
                endTime = value;
            }
        }

        public float Duration {
            get {
                return duration / timeScale;
            }
            set {
                duration = value;
            }
        }

        public float ElapsedTime {
            get {
                return Time.realtimeSinceStartup - StartTime;
            }
            set {
                StartTime = Time.realtimeSinceStartup - value;
            }
        }

        public float RemainingTime {
            get {
                return EndTime - Time.realtimeSinceStartup;
            }
            set {
                endTime = value + Time.realtimeSinceStartup;
            }
        }

        public bool IsActive {
            get {
                return Time.realtimeSinceStartup < endTime;
            }
        }

        public bool IsSet {
            get {
                return endTime > 0f;
            }
        }

        public float Progress {
            get {
                return ElapsedTime / Duration;
            }
            set {
                ElapsedTime = value * Duration;
            }
        }

        public float TimeScale {
            get {
                return timeScale;
            }
            set {
                if (IsActive) {
                    RemainingTime *= timeScale / value;
                }
                timeScale = value;
            }
        }

        private float endTime;
        private float timeScale = 1f;

        public PassiveRealTimeTimer() {}

        public PassiveRealTimeTimer(float duration) {
            this.duration = duration;
        }

        public void Start() {
            this.endTime = Time.realtimeSinceStartup + Duration;
        }

        public void Start(float duration) {
            this.duration = duration;
            this.endTime = Time.realtimeSinceStartup + duration;
        }

        public void Clear() {
            this.endTime = 0f;
        }
    }
}