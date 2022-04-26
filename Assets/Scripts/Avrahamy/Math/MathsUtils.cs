using UnityEngine;
using System;
using System.Collections.Generic;
using Avrahamy.Collections;

namespace Avrahamy.Math {
    public enum RoundingMethod {
        Ceil,
        Round,
        Floor,
    }

    public enum Comparison {
        LessThan,
        LessThanOrEqual,
        Equal,
        GreaterThan,
        GreaterThanOrEqual,
    }

    public static class MathsUtils {
        public static int BinarySearch<T, V>(this T[] array, V value, Func<T, V> GetValue) {
            var min = 0;
            var max = array.Length - 1;
            var comparer = Comparer<V>.Default;
            while (min <= max) {
                var mid = (min + max) / 2;
                var currentValue = GetValue(array[mid]);
                var comparison = comparer.Compare(value, currentValue);
                if (comparison == 0) {
                    return mid;
                } else if (comparison < 0) {
                    max = mid - 1;
                } else {
                    min = mid + 1;
                }
            }
            return -1;
        }

        public static int BinarySearch<T, V>(this List<T> array, V value, Func<T, V> GetValue) {
            var min = 0;
            var max = array.Count - 1;
            var comparer = Comparer<V>.Default;
            while (min <= max) {
                var mid = (min + max) / 2;
                var currentValue = GetValue(array[mid]);
                var comparison = comparer.Compare(value, currentValue);
                if (comparison == 0) {
                    return mid;
                } else if (comparison < 0) {
                    max = mid - 1;
                } else {
                    min = mid + 1;
                }
            }
            return -1;
        }

        public static float SignWithZero(float value, float threshold = 0.0001f) {
            if (value > threshold) return 1f;
            if (value < -threshold) return -1f;
            return 0f;
        }

        public static float Round(float value, int digits) {
            var multiplier = Mathf.Pow(10f, digits);
            return Mathf.Round(value * multiplier) / multiplier;
        }

        public static float Round(float value, RoundingMethod roundingMethod) {
            switch (roundingMethod) {
            case RoundingMethod.Ceil:
                return Mathf.Ceil(value);
            case RoundingMethod.Floor:
                return Mathf.Floor(value);
            default:
                return Mathf.Round(value);
            }
        }

        public static float RoundCustom(float value, float customInterval) {
            return customInterval * Mathf.Round(value / customInterval);
        }

        public static double Round(double value, RoundingMethod roundingMethod) {
            switch (roundingMethod) {
            case RoundingMethod.Ceil:
                return System.Math.Ceiling(value);
            case RoundingMethod.Floor:
                return System.Math.Floor(value);
            default:
                return System.Math.Round(value);
            }
        }

        public static int Round(int value, int multiples) {
            return multiples * Mathf.RoundToInt((float)value / multiples);
        }

        public static IntRange LerpIntRange(IntRange a, IntRange b, float t) {
            var minValueFloat = Mathf.Lerp(a.min, b.min, t);
            var minValueInt = (int)Mathf.Round(minValueFloat);
            var maxValueFloat = Mathf.Lerp(a.max, b.max, t);
            var maxValueInt = (int)Mathf.Round(maxValueFloat);

            return new IntRange(minValueInt, maxValueInt);
        }

        public static int LerpRoundInt(int a, int b, float t) {
            return (int)Mathf.Round(Mathf.Lerp(a, b, t));
        }

        public static float InverseLerpUnclamped(float a, float b, float value) {
            return (double) a != (double) b ? (float) (((double) value - (double) a) / ((double) b - (double) a)) : 0.0f;
        }

        public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax) {
            var t = Mathf.InverseLerp(fromMin, fromMax, value);
            return Mathf.LerpUnclamped(toMin, toMax, t);
        }

        public static int CeilToMultiple(float value, int multiple) {
            return Mathf.CeilToInt(value / multiple) * multiple;
        }

        public static int FloorToMultiple(float value, int multiple) {
            return Mathf.FloorToInt(value / multiple) * multiple;
        }

        public static bool Approximately(float lhs, float rhs, float precision = 0.001f) {
            return Mathf.Abs(lhs - rhs) <= precision;
        }

        public static bool Approximately(int lhs, int rhs, int precision) {
            return Mathf.Abs(lhs - rhs) <= precision;
        }

        /// <summary>
        /// C# modulus is strange and allows negative numbers. Use this to get
        /// only non-negative values.
        /// </summary>
        public static int Mod(int x, int m) {
            var r = x % m;
            while (r < 0) {
                r += m;
            }
            return r;
        }

        /// <summary>
        /// C# modulus is strange and allows negative numbers. Use this to get
        /// only non-negative values.
        /// </summary>
        public static float Mod(float x, float m) {
            var r = x % m;
            while (r < 0) {
                r += m;
            }
            return r;
        }

        /// <summary>
        /// C# modulus is strange and allows negative numbers. Use this to get
        /// only non-negative values.
        /// </summary>
        public static Vector2 Mod(this Vector2 vector, float m) {
            var x = Mod(vector.x, m);
            var y = Mod(vector.y, m);
            return new Vector2(x, y);
        }

        public static uint Max(uint lhs, uint rhs) {
            return lhs < rhs ? rhs : lhs;
        }

        public static uint Min(uint lhs, uint rhs) {
            return lhs > rhs ? rhs : lhs;
        }

        public static double Max(double lhs, double rhs) {
            return lhs < rhs ? rhs : lhs;
        }

        public static double Min(double lhs, double rhs) {
            return lhs > rhs ? rhs : lhs;
        }

        public static bool Compare(Comparison comparison, int lhs, int rhs) {
            switch (comparison) {
            case Comparison.Equal:
                return lhs == rhs;
            case Comparison.GreaterThan:
                return lhs > rhs;
            case Comparison.GreaterThanOrEqual:
                return lhs >= rhs;
            case Comparison.LessThan:
                return lhs < rhs;
            case Comparison.LessThanOrEqual:
                return lhs <= rhs;
            default:
                return false;
            }
        }

        public static bool Compare(Comparison comparison, uint lhs, uint rhs) {
            switch (comparison) {
            case Comparison.Equal:
                return lhs == rhs;
            case Comparison.GreaterThan:
                return lhs > rhs;
            case Comparison.GreaterThanOrEqual:
                return lhs >= rhs;
            case Comparison.LessThan:
                return lhs < rhs;
            case Comparison.LessThanOrEqual:
                return lhs <= rhs;
            default:
                return false;
            }
        }

        /// <summary>
        /// Calculates the intersection of 2 parametric line equations:
        /// p0+t*d0 and p1+v*d1
        /// NOTE: If 't' is in range [0,1], it means the infinite line that passes
        ///       at p1 with direction d1 intersects the first line between p0
        ///       and p0+d0. It does not mean that 'v' is also in range [0,1].
        /// NOTE: To calculate v: (p0.y-p1.y+t*d0.y)/d1.y or use the overloaded
        ///       call to FindLinesIntersection.
        /// </summary>
        /// <param name="p0">A point on the first line.</param>
        /// <param name="d0">The direction of the first line.</param>
        /// <param name="p1">A point on the second line.</param>
        /// <param name="d1">The direction of the second line.</param>
        /// <param name="t">The value 't' used to calculate the intersection
        /// point using the formula p0+t*d0.</param>
        public static Vector2 FindLinesIntersection(Vector2 p0, Vector2 d0, Vector2 p1, Vector2 d1, out float t) {
            t = float.NaN;
            var delta = p1 - p0;
            var numerator = d1.y * delta.x - d1.x * delta.y;
            var denominator = d0.x * d1.y - d1.x * d0.y;
            if (denominator == 0f) return p0;
            t = numerator / denominator;
            return p0 + t * d0;
        }

        /// <summary>
        /// Calculates the intersection of 2 parametric line equations:
        /// p0+t*d0 and p1+v*d1
        /// </summary>
        /// <param name="p0">A point on the first line.</param>
        /// <param name="d0">The direction of the first line.</param>
        /// <param name="p1">A point on the second line.</param>
        /// <param name="d1">The direction of the second line.</param>
        /// <param name="t">The value 't' used to calculate the intersection
        /// point using the formula p0+t*d0.</param>
        /// <param name="v">The value 'v' used to calculate the intersection
        /// point using the formula p1+v*d1.</param>
        public static Vector2 FindLinesIntersection(Vector2 p0, Vector2 d0, Vector2 p1, Vector2 d1, out float t, out float v) {
            v = float.NaN;
            var intersection = FindLinesIntersection(p0, d0, p1, d1, out t);
            if (!float.IsNaN(t)) {
                v = (p0.y - p1.y + t * d0.y) / d1.y;
            }
            return intersection;
        }

        public static int FindLineCircleIntersections(
                Vector2 center,
                float radius,
                Vector2 start,
                Vector2 end,
                out Vector2 intersection1,
                out Vector2 intersection2) {
            var dx = end.x - start.x;
            var dy = end.y - start.y;

            var A = dx * dx + dy * dy;
            var B = 2f * (dx * (start.x - center.x) + dy * (start.y - center.y));
            var C = (start.x - center.x) * (start.x - center.x) +
                (start.y - center.y) * (start.y - center.y) -
                radius * radius;

            var det = B * B - 4 * A * C;
            if (A <= 0.0000001f || det < 0f) {
                // No real solutions.
                intersection1 = Vector2.zero;
                intersection2 = Vector2.zero;
                return 0;
            }

            float t;
            if (Mathf.Approximately(det, 0f)) {
                // One solution.
                t = -B / (2f * A);
                intersection1 = new Vector2(start.x + t * dx, start.y + t * dy);
                intersection2 = Vector2.zero;
                return 1;
            }
            // Two solutions.
            var sqrtDet = Mathf.Sqrt(det);
            t = (-B + sqrtDet) / (2f * A);
            intersection1 = new Vector2(start.x + t * dx, start.y + t * dy);
            t = (-B - sqrtDet) / (2f * A);
            intersection2 = new Vector2(start.x + t * dx, start.y + t * dy);
            return 2;
        }

        public static bool IsPointOverLappingEllipse(Vector2 point, Vector2 ellipseCenter, Vector2 radius) {
            var x = point.x - ellipseCenter.x;
            var y = point.y - ellipseCenter.y;
            var xPart = x * x / (radius.x * radius.x);
            var yPart = y * y / (radius.y * radius.y);

            return xPart - yPart <= 1;
        }

        public static Vector3 GetBezierCurveValueRecursive(Vector3 from, Vector3 to, Vector3[] controls, float t) {
            if (controls.Length == 0) {
                return Vector3.Lerp(from, to, t);
            }
            if (controls.Length == 1) {
                // Edge case
                return GetBezierCurveValue(from, to, controls[0], t);
            }
            var firstControlPoint = controls[0];
            var nextFrom = Vector3.Lerp(from, firstControlPoint, t);
            var nextControls = new Vector3[controls.Length - 1];
            for (var i = 0; i < controls.Length - 1; i++) {
                var currentControl = controls[i];
                var nextControl = controls[i+1];
                nextControls[i] = Vector3.Lerp(currentControl, nextControl, t);
            }
            var lastControlPoint = controls.Last();
            var nextTo = Vector3.Lerp(lastControlPoint, to, t);

            return GetBezierCurveValueRecursive(nextFrom, nextTo, nextControls, t);
        }

        public static Vector3 GetBezierCurveValueRecursive(Vector3 from, Vector3 to, List<Vector3> controls, float t) {
            return GetBezierCurveValueRecursive(from, to ,controls.ToArray(), t);
        }

        public static Vector3 GetBezierCurveValue(Vector3 from, Vector3 to, Vector3 control, float t) {
            var lerpVal1 = Vector3.Lerp(from, control, t);
            var lerpVal2 = Vector3.Lerp(control, to, t);

            return Vector3.Lerp(lerpVal1, lerpVal2, t);
        }
    }
}
