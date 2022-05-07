// Define the following scripting define symbols to enable debug functionality:
// DEBUG_DRAW
using UnityEngine;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Avrahamy.Math;

namespace Avrahamy {
    public static class DebugDraw {
        [Conditional("DEBUG_DRAW")]
        public static void DrawLine(Vector3 start, Vector3 end, Color color) {
            UnityEngine.Debug.DrawLine(start, end, color);
        }

        [Conditional("DEBUG_DRAW")]
        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration) {
            UnityEngine.Debug.DrawLine(start, end, color, duration);
        }

        [Conditional("DEBUG_DRAW")]
        public static void GizmosDrawLine(Vector3 start, Vector3 end, Color color) {
            var previousColor = Gizmos.color;
            Gizmos.color = color;
            Gizmos.DrawLine(start, end);
            Gizmos.color = previousColor;
        }

        [Conditional("DEBUG_DRAW")]
        public static void DrawArrow2D(Vector3 position, Vector3 direction, float tipSize, float tipAngle, Color color) {
            DrawArrow2D(position, direction, tipSize, tipAngle, color, (start, end, lineColor) => DrawLine(start, end, lineColor));
        }

        [Conditional("DEBUG_DRAW")]
        public static void DrawArrow2D(Vector3 position, Vector3 direction, float tipSize, float tipAngle, Color color, float duration) {
            DrawArrow2D(position, direction, tipSize, tipAngle, color, (start, end, lineColor) => DrawLine(start, end, lineColor, duration));
        }

        [Conditional("DEBUG_DRAW")]
        public static void GizmosDrawArrow2D(Vector3 position, Vector3 direction, float tipSize, float tipAngle, Color color) {
            DrawArrow2D(position, direction, tipSize, tipAngle, color, (start, end, lineColor) => GizmosDrawLine(start, end, lineColor));
        }

        [Conditional("DEBUG_DRAW")]
        public static void DrawArrow2D(Vector3 position, float angle, float size, float tipAngle, float tipSize, Color color) {
            DrawArrow2D(position, angle, size, tipAngle, tipSize, color, (start, end, lineColor) => DrawLine(start, end, lineColor));
        }

        [Conditional("DEBUG_DRAW")]
        public static void DrawArrow2D(Vector3 position, float angle, float size, float tipAngle, float tipSize, Color color, float duration) {
            DrawArrow2D(position, angle, size, tipAngle, tipSize, color, (start, end, lineColor) => DrawLine(start, end, lineColor, duration));
        }

        [Conditional("DEBUG_DRAW")]
        public static void GizmosDrawArrow2D(Vector3 position, float angle, float size, float tipAngle, float tipSize, Color color) {
            DrawArrow2D(position, angle, size, tipAngle, tipSize, color, (start, end, lineColor) => GizmosDrawLine(start, end, lineColor));
        }

        [Conditional("DEBUG_DRAW")]
        private static void DrawArrow2D(Vector3 position, float angle, float size, float tipAngle, float tipSize, Color color, Action<Vector3, Vector3, Color> DrawLineFunc) {
            var direction = Vector2.right.GetWithMagnitude(size).RotateInDegrees(angle);
            DrawArrow2D(position, direction, tipSize, tipAngle, color, DrawLineFunc);
        }

        [Conditional("DEBUG_DRAW")]
        private static void DrawArrow2D(Vector3 start, Vector3 direction, float tipSize, float tipAngle, Color color, Action<Vector3, Vector3, Color> DrawLineFunc) {
            var end = start + direction;
            DrawLineFunc(start, end, color);
            direction = direction.GetWithMagnitude(tipSize);
            var reversedDirection = -(Vector2)direction;
            Vector3 side = reversedDirection.RotateInDegrees(tipAngle);
            DrawLineFunc(end, end + side, color);
            side = reversedDirection.RotateInDegrees(-tipAngle);
            DrawLineFunc(end, end + side, color);
        }

        [Conditional("DEBUG_DRAW")]
        public static void DrawArrowXZ(Vector3 position, Vector3 direction, float tipSize, float tipAngle, Color color) {
            DrawArrowXZ(position, direction, tipSize, tipAngle, color, (start, end, lineColor) => DrawLine(start, end, lineColor));
        }

        [Conditional("DEBUG_DRAW")]
        public static void DrawArrowXZ(Vector3 position, Vector3 direction, float tipSize, float tipAngle, Color color, float duration) {
            DrawArrowXZ(position, direction, tipSize, tipAngle, color, (start, end, lineColor) => DrawLine(start, end, lineColor, duration));
        }

        [Conditional("DEBUG_DRAW")]
        public static void GizmosDrawArrowXZ(Vector3 position, Vector3 direction, float tipSize, float tipAngle, Color color) {
            DrawArrowXZ(position, direction, tipSize, tipAngle, color, (start, end, lineColor) => GizmosDrawLine(start, end, lineColor));
        }

        [Conditional("DEBUG_DRAW")]
        public static void DrawArrowXZ(Vector3 position, float angle, float size, float tipAngle, float tipSize, Color color) {
            DrawArrowXZ(position, angle, size, tipAngle, tipSize, color, (start, end, lineColor) => DrawLine(start, end, lineColor));
        }

        [Conditional("DEBUG_DRAW")]
        public static void DrawArrowXZ(Vector3 position, float angle, float size, float tipAngle, float tipSize, Color color, float duration) {
            DrawArrowXZ(position, angle, size, tipAngle, tipSize, color, (start, end, lineColor) => DrawLine(start, end, lineColor, duration));
        }

        [Conditional("DEBUG_DRAW")]
        public static void GizmosDrawArrowXZ(Vector3 position, float angle, float size, float tipAngle, float tipSize, Color color) {
            DrawArrowXZ(position, angle, size, tipAngle, tipSize, color, (start, end, lineColor) => GizmosDrawLine(start, end, lineColor));
        }

        [Conditional("DEBUG_DRAW")]
        private static void DrawArrowXZ(Vector3 position, float angle, float size, float tipAngle, float tipSize, Color color, Action<Vector3, Vector3, Color> DrawLineFunc) {
            var direction = Vector2.right.GetWithMagnitude(size).RotateInDegrees(angle);
            DrawArrowXZ(position, direction, tipSize, tipAngle, color, DrawLineFunc);
        }

        [Conditional("DEBUG_DRAW")]
        private static void DrawArrowXZ(Vector3 start, Vector3 direction, float tipSize, float tipAngle, Color color, Action<Vector3, Vector3, Color> DrawLineFunc) {
            var end = start + direction;
            DrawLineFunc(start, end, color);
            direction = direction.GetWithMagnitude(tipSize);
            var reversedDirection = -direction.ToVector2XZ();
            Vector3 side = reversedDirection.RotateInDegrees(tipAngle).ToVector3XZ();
            DrawLineFunc(end, end + side, color);
            side = reversedDirection.RotateInDegrees(-tipAngle).ToVector3XZ();
            DrawLineFunc(end, end + side, color);
        }

        [Conditional("DEBUG_DRAW")]
        public static void DrawCross2D(Vector3 position, float size, Color color, bool axisAligned = true) {
            DrawCross2D(position, size, color, axisAligned, (start, end, lineColor) => DrawLine(start, end, lineColor));
        }

        [Conditional("DEBUG_DRAW")]
        public static void DrawCross2D(Vector3 position, float size, Color color, float duration, bool axisAligned = true) {
            DrawCross2D(position, size, color, axisAligned, (start, end, lineColor) => DrawLine(start, end, lineColor, duration));
        }

        [Conditional("DEBUG_DRAW")]
        public static void GizmosDrawCross2D(Vector3 position, float size, Color color, bool axisAligned = true) {
            DrawCross2D(position, size, color, axisAligned, (start, end, lineColor) => GizmosDrawLine(start, end, lineColor));
        }

        [Conditional("DEBUG_DRAW")]
        private static void DrawCross2D(Vector3 position, float size, Color color, bool axisAligned, Action<Vector3, Vector3, Color> DrawLineFunc) {
            var start = position;
            start.x -= size;
            var end = position;
            end.x += size;
            if (!axisAligned) {
                start.y -= size;
                end.y += size;
            }
            DrawLineFunc(start, end, color);
            start = position;
            start.y -= size;
            end = position;
            end.y += size;
            if (!axisAligned) {
                start.x += size;
                end.x -= size;
            }
            DrawLineFunc(start, end, color);
        }

        [Conditional("DEBUG_DRAW")]
        public static void DrawBox2D(Vector2 center, Vector2 size, Color color) {
            DrawBox2D(center, size, color, (start, end, lineColor) => DrawLine(start, end, lineColor));
        }

        [Conditional("DEBUG_DRAW")]
        public static void DrawBox2D(Vector2 center, Vector2 size, Color color, float duration) {
            DrawBox2D(center, size, color, (start, end, lineColor) => DrawLine(start, end, lineColor, duration));
        }

        [Conditional("DEBUG_DRAW")]
        public static void GizmosDrawBox2D(Vector2 center, Vector2 size, Color color) {
            DrawBox2D(center, size, color, (start, end, lineColor) => GizmosDrawLine(start, end, lineColor));
        }

        [Conditional("DEBUG_DRAW")]
        private static void DrawBox2D(Vector2 center, Vector2 size, Color color, Action<Vector3, Vector3, Color> DrawLineFunc) {
            var halfSize = size * 0.5f;
            var top = center.y + halfSize.y;
            var bottom = center.y - halfSize.y;
            var right = center.x + halfSize.x;
            var left = center.x - halfSize.x;
            DrawLineFunc(new Vector3(left, top), new Vector3(left, bottom), color);
            DrawLineFunc(new Vector3(right, top), new Vector3(right, bottom), color);
            DrawLineFunc(new Vector3(left, top), new Vector3(right, top), color);
            DrawLineFunc(new Vector3(left, bottom), new Vector3(right, bottom), color);
        }

        [Conditional("DEBUG_DRAW")]
        public static void DrawCircle2D(Vector3 position, float radius, Color color) {
            DrawCircle2D(position, radius, color, (start, end, lineColor) => DrawLine(start, end, lineColor));
        }

        [Conditional("DEBUG_DRAW")]
        public static void DrawCircle2D(Vector3 position, float radius, Color color, float duration) {
            DrawCircle2D(position, radius, color, (start, end, lineColor) => DrawLine(start, end, lineColor, duration));
        }

        [Conditional("DEBUG_DRAW")]
        public static void GizmosDrawCircle2D(Vector3 position, float radius, Color color) {
            DrawCircle2D(position, radius, color, (start, end, lineColor) => GizmosDrawLine(start, end, lineColor));
        }

        [Conditional("DEBUG_DRAW")]
        public static void GizmosDrawEllipse2D(Vector3 position, Vector2 radius, Color color) {
            DrawEllipse2D(position, radius, color, (start, end, lineColor) => GizmosDrawLine(start, end, lineColor));
        }

        [Conditional("DEBUG_DRAW")]
        private static void DrawCircle2D(Vector3 position, float radius, Color color, Action<Vector3, Vector3, Color> DrawLineFunc) {
            var direction = Vector2.up * radius;
            var start = position + (Vector3)direction;
            var end = start;
            for (int i = 0; i < 36; i++) {
                direction = direction.RotateInDegrees(10f);
                start = end;
                end = position + (Vector3)direction;
                DrawLineFunc(start, end, color);
            }
        }

        [Conditional("DEBUG_DRAW")]
        private static void DrawEllipse2D(Vector3 position, Vector2 radius, Color color, Action<Vector3, Vector3, Color> DrawLineFunc) {
            var ratio = radius.y / radius.x;
            var direction = Vector2.right * radius.x;
            var start = (Vector3)direction;
            var end = start;
            for (int i = 0; i < 36; i++) {
                direction = direction.RotateInDegrees(10f);
                start = end;
                end = direction;
                end.y *= ratio;
                DrawLineFunc(position + start, position + end, color);
            }
        }

        [Conditional("DEBUG_DRAW")]
        public static void DrawSectors(Vector3 position, float radius, float[] sectorAngles, Color color) {
            DrawSectors(position, radius, sectorAngles, color, (start, end, lineColor) => DrawLine(start, end, lineColor));
        }

        [Conditional("DEBUG_DRAW")]
        public static void DrawSectors(Vector3 position, float radius, float[] sectorAngles, Color color, float duration) {
            DrawSectors(position, radius, sectorAngles, color, (start, end, lineColor) => DrawLine(start, end, lineColor, duration));
        }

        [Conditional("DEBUG_DRAW")]
        public static void GizmosDrawSectors(Vector3 position, float radius, float[] sectorAngles, Color color) {
            DrawSectors(position, radius, sectorAngles, color, (start, end, lineColor) => GizmosDrawLine(start, end, lineColor));
        }

        [Conditional("DEBUG_DRAW")]
        private static void DrawSectors(Vector3 position, float radius, float[] sectorAngles, Color color, Action<Vector3, Vector3, Color> DrawLineFunc) {
            if (sectorAngles == null) return;
            for (int i = 0; i < sectorAngles.Length / 2; i++) {
                DrawSector(position, radius, sectorAngles[i * 2], sectorAngles[i * 2 + 1], color, DrawLineFunc);
            }
        }

        [Conditional("DEBUG_DRAW")]
        public static void DrawSector(Vector3 position, float radius, float sectorStartAngle, float sectorEndAngle, Color color) {
            DrawSector(position, radius,  sectorStartAngle, sectorEndAngle, color, (start, end, lineColor) => DrawLine(start, end, lineColor));
        }

        [Conditional("DEBUG_DRAW")]
        public static void DrawSector(Vector3 position, float radius, float sectorStartAngle, float sectorEndAngle, Color color, float duration) {
            DrawSector(position, radius,  sectorStartAngle, sectorEndAngle, color, (start, end, lineColor) => DrawLine(start, end, lineColor, duration));
        }

        [Conditional("DEBUG_DRAW")]
        public static void GizmosDrawSector(Vector3 position, float radius, float sectorStartAngle, float sectorEndAngle, Color color) {
            DrawSector(position, radius,  sectorStartAngle, sectorEndAngle, color, (start, end, lineColor) => GizmosDrawLine(start, end, lineColor));
        }

        [Conditional("DEBUG_DRAW")]
        private static void DrawSector2D(Vector3 position, float radius, float sectorStartAngle, float sectorEndAngle, Color color, Action<Vector3, Vector3, Color> DrawLineFunc) {
            if (sectorEndAngle < sectorStartAngle) {
                (sectorStartAngle, sectorEndAngle) = (sectorEndAngle, sectorStartAngle);
            }
            var direction = Vector2.right * radius;
            direction = direction.RotateInDegrees(sectorStartAngle);
            var sectorAngle = sectorEndAngle - sectorStartAngle;
            var angleSteps = Mathf.CeilToInt(sectorAngle / 8f);
            var angleStepSize = sectorAngle / angleSteps;
            var start = position + (Vector3)direction;
            DrawLineFunc(position, start, color);
            var end = start;
            for (int i = 0; i < angleSteps; i++) {
                direction = direction.RotateInDegrees(angleStepSize);
                start = end;
                end = position + (Vector3)direction;
                DrawLineFunc(start, end, color);
            }
            DrawLineFunc(end, position, color);
        }

        [Conditional("DEBUG_DRAW")]
        private static void DrawSector(Vector3 position, float radius, float sectorStartAngle, float sectorEndAngle, Color color, Action<Vector3, Vector3, Color> DrawLineFunc) {
            if (sectorEndAngle < sectorStartAngle) {
                (sectorStartAngle, sectorEndAngle) = (sectorEndAngle, sectorStartAngle);
            }
            var direction = Vector3.forward * radius;
            direction = direction.RotateInDegreesAroundY(sectorStartAngle);
            var sectorAngle = sectorEndAngle - sectorStartAngle;
            var angleSteps = Mathf.CeilToInt(sectorAngle / 8f);
            var angleStepSize = sectorAngle / angleSteps;
            var start = position + direction;
            DrawLineFunc(position, start, color);
            var end = start;
            for (int i = 0; i < angleSteps; i++) {
                direction = direction.RotateInDegreesAroundY(angleStepSize);
                start = end;
                end = position + direction;
                DrawLineFunc(start, end, color);
            }
            DrawLineFunc(end, position, color);
        }

        [Conditional("DEBUG_DRAW")]
        public static void GizmosDrawBackSector(Vector3 position,
            Func<Vector3, float> radius,
            float sectorStartAngle,
            float sectorEndAngle, 
            Color color) {
            DrawBackSector(position, radius,  sectorStartAngle, sectorEndAngle, color, (start, end, lineColor) => GizmosDrawLine(start, end, lineColor));
        }
        
        [Conditional("DEBUG_DRAW")]
        private static void DrawBackSector(Vector3 position,
            Func<Vector3, float> radius,
            float sectorStartAngle, 
            float sectorEndAngle,
            Color color,
            Action<Vector3, Vector3, Color> DrawLineFunc)
        {
            if (sectorEndAngle < sectorStartAngle) {
                (sectorStartAngle, sectorEndAngle) = (sectorEndAngle, sectorStartAngle);
            }
            var direction = Vector3.forward;
            direction = direction.RotateInDegreesAroundY(sectorEndAngle);
            var sectorAngle = 360 - (sectorEndAngle - sectorStartAngle);
            var angleSteps = Mathf.CeilToInt(sectorAngle / 8f);
            var angleStepSize = sectorAngle / angleSteps;
            var start = position + direction * radius(direction);
            DrawLineFunc(position, start, color);
            var end = start;
            for (int i = 0; i < angleSteps; i++) {
                direction = direction.RotateInDegreesAroundY(angleStepSize);
                start = end;
                end = position + direction * radius(direction);
                DrawLineFunc(start, end, color);
            }
            DrawLineFunc(end, position, color);
        }
    }
}
