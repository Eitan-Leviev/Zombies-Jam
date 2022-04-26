using UnityEngine;
using System;
using Avrahamy.Math;

namespace Avrahamy {
    [Serializable]
    public class RandomPassiveTimer : ITimer {
        [SerializeField] FloatRange duration;

        public float StartTime {
            get {
                return startTime;
            }
            set {
                startTime = value;
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
                return endTime - startTime / timeScale;
            }
            set {
                endTime = startTime + value;
            }
        }

        public float MinDuration {
            get {
                return duration.min;
            }
        }

        public float MaxDuration {
            get {
                return duration.max;
            }
        }

        public float ElapsedTime {
            get {
                return Time.time - StartTime;
            }
            set {
                StartTime = Time.time - value;
            }
        }

        public float RemainingTime {
            get {
                return endTime - Time.time;
            }
            set {
                endTime = value + Time.time;
            }
        }

        public bool IsActive {
            get {
                return Time.time < endTime;
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
                if (IsActive && !Mathf.Approximately(timeScale, value)) {
                    RemainingTime *= timeScale / value;
                }
                timeScale = value;
            }
        }

        private float startTime;
        private float endTime;
        private float timeScale = 1f;

        public RandomPassiveTimer() {}

        public void Start() {
            this.startTime = Time.time;
            Duration = RandomUtils.Range(duration);
        }

        public void Start(float duration) {
            this.startTime = Time.time;
            this.endTime = Time.time + duration;
        }

        public void Clear() {
            this.endTime = 0f;
        }
    }
}