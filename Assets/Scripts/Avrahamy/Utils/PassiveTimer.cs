using UnityEngine;
using System;

namespace Avrahamy {
    [Serializable]
    public class PassiveTimer : ITimer {
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
                if (IsActive) {
                    RemainingTime *= timeScale / value;
                }
                timeScale = value;
            }
        }

        private float endTime;
        private float timeScale = 1f;

        public PassiveTimer() {}

        public PassiveTimer(float duration) {
            this.duration = duration;
        }

        public void Start() {
            this.endTime = Time.time + Duration;
        }

        public void Start(float duration) {
            this.duration = duration;
            this.endTime = Time.time + duration;
        }

        public void Clear() {
            this.endTime = 0f;
        }

        public void ContinueTimer(PassiveTimer fromTimer) {
            if (!fromTimer.IsSet) return;
            endTime = fromTimer.endTime;
            timeScale = fromTimer.timeScale;
        }

        public void SetRemainingTimeAndPreserveStartTime(float remainingTime) {
            // remainingTime + Time.time - endTime
            AddTimeAndPreserveStartTime(remainingTime - RemainingTime);
            // endTime = remainingTime + Time.time
            // duration = duration + remainingTime + Time.time - endTime
        }

        public void AddTimeAndPreserveStartTime(float additionalTime) {
            endTime += additionalTime;
            duration += additionalTime;
        }
    }
}