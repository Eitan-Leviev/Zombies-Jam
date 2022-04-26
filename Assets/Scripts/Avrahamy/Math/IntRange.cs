using UnityEngine;
using System;

namespace Avrahamy.Math {
    [Serializable]
    public struct IntRange {
        public int min;
        public int max;

        public IntRange(int defaultValue) : this(defaultValue, defaultValue) {}

        public IntRange(int min, int max) {
            this.min = min;
            this.max = max;
        }

        public int Clamp(int value) {
            return Mathf.Clamp(value, min, max);
        }

        public float Lerp(float t) {
            return Mathf.Lerp(min, max, t);
        }

        /// <summary>
        /// Inclusive.
        /// </summary>
        public bool IsInRange(int value) {
            return min <= value && value <= max;
        }

        /// <summary>
        /// Inclusive min, exclusive max
        /// </summary>
        public bool IsInRangeExclusive(int value) {
            return min <= value && value < max;
        }
    }
}
