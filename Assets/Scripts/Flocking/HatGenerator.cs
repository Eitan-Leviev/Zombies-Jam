using System.Collections.Generic;
using Avrahamy.Math;
using Avrahamy.Meshes;
using UnityEngine;

namespace Flocking
{
    [CreateAssetMenu(menuName = "HatGenerator/Hat", fileName = "HatGenerator")]
    public class HatGenerator : ScriptableObject
    {
        [SerializeField]
        public float maxRadius = 0.9f;
        [SerializeField]
        public float minRadius = 0.1f;

        [SerializeField]
        private IntRange pointsInCrossSectionRange;

        [SerializeField]
        private FloatRange radius;

        [Range(0f, 1f)]
        [SerializeField]
        private float angleRandomAmount;

        [SerializeField]
        private bool autoClampToMaxRadius = true;

        [Range(0f, 180f)]
        [SerializeField]
        private float hardEdgeMaxAngle = 120f;

        protected void OnValidate()
        {
            if (pointsInCrossSectionRange.min < 1)
            {
                Debug.LogError("Must have at least 1 point in cross section");
                pointsInCrossSectionRange.min = 1;
                pointsInCrossSectionRange.max = Mathf.Max(pointsInCrossSectionRange.max, 1);
            }

            if (radius.min < minRadius)
            {
                Debug.LogError($"Radius must be greater than {minRadius}");
                radius.min = minRadius;
                radius.max = Mathf.Max(radius.min, radius.max);
            }

            if (radius.max > maxRadius)
            {
                Debug.LogError($"Radius must be smaller than {maxRadius}");
                radius.max = maxRadius;
                radius.min = Mathf.Max(radius.min, radius.max);
            }
        }

        public void Generate(EditableMesh mesh)
        {
            var crossSection = GenerateHalfCrossSection();

            // Bottom point.
            var length = RandomUtils.Range(radius.min, radius.max);
            crossSection.Add(Vector2.down * length);

            // Spread the UVs around the first cap.
            var uvs = new List<Vector2>();
            var halfCrossSectionSize = crossSection.Count;
            var topRightIndex = (halfCrossSectionSize - 2) / 4;
            var bottomRightIndex = (halfCrossSectionSize * 3 - 2) / 4;
            // The uvs of the first cap's half-top-edge start from the middle of
            // the line (0.75,0.25)-(1,0.25) all the way up to (1,0.25).
            for (int i = 0; i < topRightIndex; i++)
            {
                uvs.Add(new Vector2(Mathf.Lerp(0.875f, 1f, (i + 1f) / (topRightIndex + 1f)), 0.25f));
            }

            // Top right.
            uvs.Add(new Vector2(1f, 0.25f));
            for (int i = topRightIndex + 1; i < bottomRightIndex; i++)
            {
                uvs.Add(new Vector2(1f,
                    Mathf.Lerp(0.25f,
                        0f,
                        (float) (i - topRightIndex) / (bottomRightIndex - topRightIndex))));
            }

            // Bottom right.
            uvs.Add(new Vector2(1f, 0f));
            for (int i = bottomRightIndex + 1; i < halfCrossSectionSize; i++)
            {
                uvs.Add(new Vector2(Mathf.Lerp(1f,
                        0.875f,
                        (i - bottomRightIndex) / (halfCrossSectionSize - bottomRightIndex - 1f)),
                    0f));
            }

            // Other half.
            if (halfCrossSectionSize == 2)
            {
                uvs.Add(new Vector2(0.75f, 0f));
            }

            for (int i = halfCrossSectionSize - 2; i >= 0; i--)
            {
                var uv = uvs[i];
                uv.x = 0.75f + 1f - uv.x;
                uvs.Add(uv);
            }

            if (halfCrossSectionSize != 2)
            {
                var uv = uvs[halfCrossSectionSize - 1];
                uv.y = 0.25f;
                uvs.Add(uv);
            }

            // Calculate the UVs of the circumference.
            for (int i = 0; i < topRightIndex; i++)
            {
                uvs.Add(new Vector2(Mathf.Lerp(0.875f, 1f, (i + 1f) / (topRightIndex + 1f)), 0.25f));
            }

            // Top right.
            uvs.Add(new Vector2(1f, 0.25f));
            // Continue wrapping outside of the 1x1 UVs in case the top right
            // vertex is not a hard edge.
            for (int i = topRightIndex + 1; i < bottomRightIndex; i++)
            {
                uvs.Add(new Vector2(Mathf.Lerp(1f,
                        1.25f,
                        (float) (i - topRightIndex) / (bottomRightIndex - topRightIndex)),
                    0.25f));
            }

            // Bottom right.
            uvs.Add(new Vector2(1.25f, 0.25f));
            for (int i = bottomRightIndex + 1; i < halfCrossSectionSize; i++)
            {
                uvs.Add(new Vector2(Mathf.Lerp(1.25f,
                        1.375f,
                        (i - bottomRightIndex) / (halfCrossSectionSize - bottomRightIndex - 1f)),
                    0.25f));
            }

            // Other half.
            if (halfCrossSectionSize == 2)
            {
                uvs.Add(new Vector2(1.5f, 0.25f));
            }

            for (int i = halfCrossSectionSize * 3 - 2; i >= halfCrossSectionSize * 2; i--)
            {
                var uv = uvs[i];
                uv.x = 2.75f - uv.x;
                uvs.Add(uv);
            }

            if (halfCrossSectionSize != 2)
            {
                uvs.Add(new Vector2(1.875f, 0.25f));
            }

            // Calculate the mirror image of half cross section - the other half.
            for (var i = 0; i < halfCrossSectionSize - 1; i++)
            {
                var point = crossSection[halfCrossSectionSize - 2 - i];
                point.x = -point.x;
                crossSection.Add(point);
            }

            // Top point.
            length = RandomUtils.Range(radius.min, radius.max);
            crossSection.Add(Vector2.up * length);

            // This will give us the triangles of the first cap.
            var crossSectionCapTriangles = Triangulator.Triangulate(crossSection.ToArray());

            var unsplitCrossSection = new List<Vector2>(crossSection);
            var hardEdgeIndices = SplitByAngle(crossSection, hardEdgeMaxAngle);
            // Always split the first point so the UVs could wrap.
            if (!hardEdgeIndices.Contains(0))
            {
                hardEdgeIndices.Insert(0, 0);
                crossSection.Insert(0, crossSection[0]);
            }

            for (int i = halfCrossSectionSize * 2 - 1; i >= 0; i--)
            {
                if (hardEdgeIndices.Contains(i))
                {
                    var uv = uvs[i + halfCrossSectionSize * 2];
                    if (i == 0)
                    {
                        // Wrap the UVs at the end.
                        uv.x += 1f;
                    }

                    uvs.Insert(i + halfCrossSectionSize * 2, uv);
                }
            }

            var uvsCount = uvs.Count;
            for (int i = halfCrossSectionSize * 2; i < uvsCount; i++)
            {
                var uv = uvs[i];
                uv.y = 0.75f;
                uvs.Add(uv);
            }

            for (int i = 0; i < halfCrossSectionSize * 2; i++)
            {
                var uv = uvs[i];
                uv.y = 1f - uv.y;
                uvs.Add(uv);
            }

            // The flat caps have a hard edge.
            var points = new Vector3[unsplitCrossSection.Count * 2 + crossSection.Count * 2];
            // Each face in the circumference can be assigned to a point in the
            // cross section. So we have 2 triangles per point.
            var circumferenceTriangles = unsplitCrossSection.Count * 2;
            // Each triangle is 3 indices.
            var triangles = new int[crossSectionCapTriangles.Length * 2 + circumferenceTriangles * 3];

            // The caps points are the un-split cross section points.
            var farCapIndexOffset = points.Length - unsplitCrossSection.Count;
            for (int i = 0; i < unsplitCrossSection.Count; i++)
            {
                var point = (Vector3) unsplitCrossSection[i];
                point.z = -maxRadius;
                points[i] = point;
                point.z = maxRadius;
                points[i + farCapIndexOffset] = point;
            }

            // Transform the hard edge indices to the new crossSection indices.
            for (int i = 0; i < hardEdgeIndices.Count; i++)
            {
                hardEdgeIndices[i] += i;
            }

            // The circumference points are the split cross section points.
            var circumferenceTrianglesOffset = crossSectionCapTriangles.Length;
            for (int i = unsplitCrossSection.Count, trianglesIndex = 0;
                 i < crossSection.Count + unsplitCrossSection.Count;
                 i++, trianglesIndex += 6)
            {
                var crossSectionIndex = i - unsplitCrossSection.Count;
                var point = (Vector3) crossSection[crossSectionIndex];
                point.z = -maxRadius;
                points[i] = point;
                point.z = maxRadius;
                points[i + crossSection.Count] = point;

                if (hardEdgeIndices.Contains(crossSectionIndex))
                {
                    // Skip the duplicated hard edge vertex. No need to connect
                    // duplicated points with triangles.
                    trianglesIndex -= 6;
                    continue;
                }

                // Connect each pair from either side with 2 triangles.
                triangles[circumferenceTrianglesOffset + trianglesIndex] = i;
                var nextIndex = (crossSectionIndex + 1) % crossSection.Count + unsplitCrossSection.Count;
                triangles[circumferenceTrianglesOffset + trianglesIndex + 1] = i + crossSection.Count;
                triangles[circumferenceTrianglesOffset + trianglesIndex + 2] = nextIndex;

                triangles[circumferenceTrianglesOffset + trianglesIndex + 3] = nextIndex;
                triangles[circumferenceTrianglesOffset + trianglesIndex + 4] = i + crossSection.Count;
                triangles[circumferenceTrianglesOffset + trianglesIndex + 5] =
                    nextIndex + crossSection.Count;
            }

            // Copy the caps triangle indices.
            var farCapTrianglesOffset = triangles.Length - crossSectionCapTriangles.Length;
            for (int i = 0; i < crossSectionCapTriangles.Length; i++)
            {
                triangles[i] = crossSectionCapTriangles[i];
                triangles[i + farCapTrianglesOffset] = crossSectionCapTriangles[i] +
                                                       crossSection.Count * 2 + unsplitCrossSection.Count;
            }

            // Flip the triangles order to get the opposite cap to have triangles
            // in clock-wise order (otherwise they will be back-faced and culled).
            FlipTriangles(triangles, farCapTrianglesOffset);

            if (autoClampToMaxRadius)
            {
                ClampToRadius(points, maxRadius);
            }

            mesh.SetPoints(points, triangles);

            mesh.Mesh.uv = uvs.ToArray();
        }

        /// <summary>
        /// Generates a random half-a-circle-cross-section without the top and
        /// bottom points.
        /// </summary>
        private List<Vector2> GenerateHalfCrossSection()
        {
            // Decide how many points will be in the half-cross section.
            var pointsInCrossSection = RandomUtils.Range(pointsInCrossSectionRange.min, pointsInCrossSectionRange.max);
            var crossSection = new List<Vector2>(pointsInCrossSection);

            // Rotate the up vector at each step to get the next point in the
            // cross section.
            var offset = Vector2.up;
            var angleStep = 180f / (pointsInCrossSection + 1);
            var sumAngle = 0f;
            for (int i = 0; i < pointsInCrossSection; i++)
            {
                // If angle noise amount is 0, the angle will be a fixed value
                // and the points will be evenly spaced. If it is 1, the angle
                // will be between 0 and angleStep.
                var angle = angleStep * (1f - Random.value * angleRandomAmount);
                offset = offset.RotateInDegrees(-angle);
                // Decide how far away from the center the current point will be.
                var length = RandomUtils.Range(radius.min, radius.max);
                crossSection.Add(offset * length);
                sumAngle += angle;
                // Divide the remaining angle by the number of points we are yet
                // to generate.
                angleStep = (180f - sumAngle) / (pointsInCrossSection - i);
            }

            return crossSection;
        }

        /// <summary>
        /// Assuming the center is (0,0,0), clamps the points to a sphere with
        /// the given radius.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="radius"></param>
        protected static void ClampToRadius(Vector3[] points, float radius)
        {
            var sqrRadius = radius * radius;
            for (var i = 0; i < points.Length; i++)
            {
                var point = points[i];
                if (point.sqrMagnitude > sqrRadius)
                {
                    points[i] = point.normalized * radius;
                }
            }
        }

        /// <summary>
        /// Assuming the center is (0,0,0), clamps the points to a sphere with
        /// the given radius.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="radius"></param>
        protected static void ClampToRadius(List<Vector3> points, float radius)
        {
            var sqrRadius = radius * radius;
            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];
                if (point.sqrMagnitude > sqrRadius)
                {
                    points[i] = point.normalized * radius;
                }
            }
        }
        
         /// <summary>
        /// Split points in a cross section that should extrude to a hard edge.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="angleThreshold"></param>
        /// <returns>The indices in the original list that were duplicated.</returns>
        protected static List<int> SplitByAngle(List<Vector2> points, float angleThreshold) {
            var splits = new List<int>();
            var n = points.Count;
            for (int i = 0; i < n; i++) {
                // Calculate the angle between each set of 3 points.
                var p0 = points[(i - 1 + n) % n];
                var p1 = points[i];
                var p2 = points[(i + 1) % n];
                var l0 = p0 - p1;
                if (l0.sqrMagnitude < 0.001f) {
                    // This is already a hard edge.
                    continue;
                }
                var l1 = p2 - p1;
                if (l1.sqrMagnitude < 0.001f) {
                    // This is already a hard edge.
                    continue;
                }
                var angle = Vector2.Angle(l0, l1);
                if (angle > angleThreshold) continue;
                // Should split the point.
                splits.Add(i);
            }
            if (splits.Count == 0) return splits;

            // Duplicate the split points.
            for (int i = n - 1; i >= 0; i--) {
                if (splits.Contains(i)) {
                    points.Insert(i, points[i]);
                }
            }
            return splits;
        }

        /// <summary>
        /// Flips the facing side of the triangles in the range.
        /// </summary>
        /// <param name="triangles"></param>
        /// <param name="fromIndex"></param>
        /// <param name="toIndex"></param>
        protected static void FlipTriangles(int[] triangles, int fromIndex = 0, int toIndex = -1) {
            Debug.Assert(fromIndex % 3 == 0, $"FlipTriangles got fromIndex={fromIndex}. Index must divide by 3!");
            if (toIndex < 0) {
                toIndex = triangles.Length - 1;
            }
            for (int i = fromIndex; i <= toIndex; i += 3) {
                (triangles[i], triangles[i + 1]) = (triangles[i + 1], triangles[i]);
            }
        }
    }
}