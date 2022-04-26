using UnityEngine;
using System.Collections.Generic;
using Avrahamy.Collections;

namespace Avrahamy.Math {
    public static class RandomUtils {
        private static bool keptState = false;
        private static Random.State originalState;

        public static Random.State OverrideState(Random.State state) {
            if (!keptState) {
                originalState = Random.state;
                keptState = true;
            }
            Random.state = state;
            return originalState;
        }

        public static Random.State OverrideSeed(int seed) {
            if (!keptState) {
                originalState = Random.state;
                keptState = true;
            }
            Random.InitState(seed);
            return originalState;
        }

        public static Random.State ResumeSeed() {
            if (!keptState) {
                // No seed to resume.
                return Random.state;
            }
            keptState = false;
            var oldState = Random.state;
            Random.state = originalState;
            return oldState;
        }

        public static T ChooseRandom<T>(this T[] array, int fromIndex = 0) {
            if (array.Length == 0) {
                throw new System.Exception("ChooseRandom got empty array");
            }
            return array[Random.Range(fromIndex, array.Length)];
        }

        public static T ChooseRandom<T>(this T[] array, int fromIndex, int toIndex) {
            if (array.Length == 0) {
                throw new System.Exception("ChooseRandom got empty array");
            }
            return array[Random.Range(fromIndex, toIndex)];
        }

        public static T ChooseRandom<T>(this List<T> array, int fromIndex = 0) {
            if (array.Count == 0) {
                throw new System.Exception("ChooseRandom got empty List");
            }
            return array[Random.Range(fromIndex, array.Count)];
        }

        public static T ChooseRandom<T>(this List<T> array, int fromIndex, int toIndex) {
            if (array.Count == 0) {
                throw new System.Exception("ChooseRandom got empty List");
            }
            return array[Random.Range(fromIndex, toIndex)];
        }

        public static TKey ChooseRandom<TKey, TValue>(this Dictionary<TKey, TValue>.KeyCollection collection, int fromIndex = 0) {
            if (collection.Count == 0) {
                throw new System.Exception("ChooseRandom got empty List");
            }
            var index = Random.Range(fromIndex, collection.Count);
            foreach (var item in collection) {
                if (index == 0) {
                    return item;
                }
                --index;
            }
            throw new System.Exception("ChooseRandom from key collection didn't choose anything");
        }

        /// <summary>
        /// Chooses a random element from the array.
        /// Chances array values don't have to be normalized so they sum to 1.
        /// </summary>
        public static T ChooseRandom<T>(this T[] array, float[] chances) {
            DebugAssert.Assert(array.Length == chances.Length, "ChooseRandom with chances got mismatching array sizes");
            if (array.Length == 0) {
                throw new System.Exception("ChooseRandom with chances got empty array");
            }
            var sumChance = 0f;
            for (int i = 0; i < chances.Length; i++) {
                sumChance += chances[i];
            }

            var rnd = Random.value * sumChance;

            for (int i = 0; i < chances.Length; i++) {
                rnd -= chances[i];
                // Checking for less than 0 so if rnd was 0, something with 0 chance
                // won't get picked.
                if (rnd < 0f) {
                    return array[i];
                }
            }
            return array[array.Length - 1];
        }

        [System.Serializable]
        public class ValueWithChance<T> {
            public T value;
            public float chance;
        }

        [System.Serializable]
        public abstract class ClassWithChance {
            public float chance;
        }

        /// <summary>
        /// Chooses a random element from the array.
        /// Chances values don't have to be normalized so they sum to 1.
        /// </summary>
        public static T ChooseRandomWithChances<T>(this ValueWithChance<T>[] array) {
            if (array.Length == 0) {
                throw new System.Exception("ChooseRandom with chances got empty array");
            }
            var sumChance = 0f;
            foreach (var t in array) {
                sumChance += t.chance;
            }
            if (Mathf.Approximately(sumChance, 0f)) {
                foreach (var t in array) {
                    t.chance = 1f;
                }
                sumChance = array.Length;
            }

            var rnd = Random.value * sumChance;

            foreach (var t in array) {
                rnd -= t.chance;
                // Checking for less than 0 so if rnd was 0, something with 0 chance
                // won't get picked.
                if (rnd < 0f) {
                    return t.value;
                }
            }
            return array[array.Length - 1].value;
        }

        /// <summary>
        /// Chooses a random element from the list.
        /// Chances values don't have to be normalized so they sum to 1.
        /// </summary>
        public static T ChooseRandomWithChances<T, S>(this List<S> list) where S : ValueWithChance<T> {
            if (list.Count == 0) {
                throw new System.Exception("ChooseRandom with chances got empty array");
            }
            var sumChance = 0f;
            foreach (var t in list) {
                sumChance += t.chance;
            }

            var rnd = Random.value * sumChance;

            foreach (var t in list) {
                rnd -= t.chance;
                // Checking for less than 0 so if rnd was 0, something with 0 chance
                // won't get picked.
                if (rnd < 0f) {
                    return t.value;
                }
            }
            return list[list.Count - 1].value;
        }

        /// <summary>
        /// Chooses a random element from the array.
        /// Chances values don't have to be normalized so they sum to 1.
        /// </summary>
        public static T ChooseRandomWithChancesC<T>(this T[] array) where T : ClassWithChance {
            if (array.Length == 0) {
                throw new System.Exception("ChooseRandom with chances got empty array");
            }
            var sumChance = 0f;
            foreach (var t in array) {
                sumChance += t.chance;
            }

            var rnd = Random.value * sumChance;

            foreach (var t in array) {
                rnd -= t.chance;
                // Checking for less than 0 so if rnd was 0, something with 0 chance
                // won't get picked.
                if (rnd < 0f) {
                    return t;
                }
            }
            return array[array.Length - 1];
        }

        public static bool RandBool(float trueChance = 0.5f) {
            DebugAssert.Assert(0f <= trueChance && trueChance <= 1f, "RandBool got chance out of range: " + trueChance);
            return Random.value <= trueChance;
        }

        public static float Range(float minInclusive, float maxExclusive) {
            return Random.Range(minInclusive, maxExclusive);
        }

        public static int Range(int minInclusive, int maxExclusive) {
            return Random.Range(minInclusive, maxExclusive);
        }

        public static uint Range(uint minInclusive, uint maxExclusive) {
            return (uint)Random.Range((int)minInclusive, (int)maxExclusive);
        }

        public static int Range(IntRange range) {
            return Random.Range(range.min, range.max);
        }

        public static float Range(FloatRange range) {
            return Random.Range(range.min, range.max);
        }

        public static float Range(FloatRange range, List<FloatRange> invalidRanges) {
            var validRanges = new List<FloatRange>() {range};
            validRanges = GetValidRangesRec(validRanges, invalidRanges.ToStack());

            return Range(validRanges.ChooseRandom());
        }

        private static List<FloatRange> GetValidRangesRec(List<FloatRange> validRanges, Stack<FloatRange> invalidRanges) {
            if (invalidRanges.Count == 0) {
                return validRanges;
            }

            var nextInvalidRange = invalidRanges.Pop();
            var newValidRanges = new List<FloatRange>();
            for (var i = 0; i < validRanges.Count; i++) {
                var validRange = validRanges[i];
                // No overlap - do nothing
                if (nextInvalidRange.min > validRange.max || nextInvalidRange.max < validRange.min) continue;
                // Full overlap - split to two ranges
                if (nextInvalidRange.min > validRange.min && nextInvalidRange.max < validRange.max) {
                    validRanges[i] = new FloatRange(validRange.min, nextInvalidRange.min);
                    newValidRanges.Add(new FloatRange(nextInvalidRange.max, validRange.max));
                    continue;
                }
                // Partial overlap - shorten range
                if (nextInvalidRange.min > validRange.min) {
                    validRanges[i] = new FloatRange(validRange.min, nextInvalidRange.min);
                } else {
                    validRanges[i] = new FloatRange(nextInvalidRange.max, validRange.max);
                }
            }

            validRanges.AddRange(newValidRanges);

            return GetValidRangesRec(validRanges, invalidRanges);
        }

        /// <summary>
        /// Gaussian distribution based on the Marsaglia polar method.
        /// Standard deviation is 1.
        /// </summary>
        public static float RandomGaussian() {
            // Choose a random point where x and y are in (-1, 1) until we get a
            // point that is inside the unit circle.
            float u, v, S;
            do {
                u = 2f * Random.value - 1f;
                v = 2f * Random.value - 1f;
                S = u * u + v * v;
            } while (S >= 1f);

            // Return x*sqrt(-2*ln(S)/S)
            float fac = Mathf.Sqrt(-2f * Mathf.Log(S) / S);
            return u * fac;
        }

        /// <summary>
        /// Gaussian distribution based on the Marsaglia polar method.
        /// Standard deviation is sigma.
        /// </summary>
        public static float RandomGaussian(float mean, float sigma) {
            return RandomGaussian() * sigma + mean;
        }

        /// <summary>
        /// Returns a random number with a gaussian distribution over the given range.
        /// If mean is given, the 2 sides of the curve are not symmetrical.
        /// Using the 3 sigma rule to limit the range.
        /// </summary>
        public static float GaussianRange(float minValue, float maxValue, float mean = float.NegativeInfinity) {
            float sigma = 0f;
            if (mean < minValue) {
                // Use a normal mean based on minValue and maxValue.
                mean = (minValue + maxValue) / 2f;
                sigma = (maxValue - mean) / 3f;
            } else {
                sigma = Mathf.Max(mean - minValue, maxValue - mean) / 3f;
            }
            DebugAssert.Assert(minValue <= maxValue, "GaussianRange got minValue " + minValue + " > maxValue " + maxValue);
            DebugAssert.Assert(minValue <= mean, "GaussianRange got minValue " + minValue + " > mean " + mean);
            DebugAssert.Assert(mean <= maxValue, "GaussianRange got mean " + mean + " > maxValue " + maxValue);
            var value = RandomGaussian() * sigma + mean;
            if (value < minValue || value > maxValue) {
                return GaussianRange(minValue, maxValue, mean);
            }
            return value;
        }

        public static void Shuffle<T>(this T[] arr) {
            var length = arr.Length;
            for (int i = 0; i < length - 1; i++) {
                int k = Random.Range(i, length);
                var tmp = arr[i];
                arr[i] = arr[k];
                arr[k] = tmp;
            }
        }

        public static void Shuffle<T>(this IList<T> list) {
            var length = list.Count;
            for (int i = 0; i < length - 1; i++) {
                int k = Random.Range(i, length);
                var tmp = list[i];
                list[i] = list[k];
                list[k] = tmp;
            }
        }
    }
}
