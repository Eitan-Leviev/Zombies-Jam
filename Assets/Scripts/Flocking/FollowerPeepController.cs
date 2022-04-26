using UnityEngine;
using Random = UnityEngine.Random;
using Avrahamy;
using Avrahamy.Math;
using BitStrap;

namespace Flocking {
    public class FollowerPeepController : MonoBehaviour {
        [SerializeField] PeepController peep;
        [SerializeField] float senseRadius = 2f;
        [SerializeField] PassiveTimer navigationTimer;
        [SerializeField] LayerMask navigationMask;
        [TagSelector]
        [SerializeField] string peepTag;
        [SerializeField] bool repelFromSameGroup;
        
        [SerializeField]
        private int[] weights = new[]{1,4,2};

        private static readonly Collider[] COLLIDER_RESULTS = new Collider[10];

        protected void Reset() {
            if (peep == null) {
                peep = GetComponent<PeepController>();
            }
        }

        protected void OnEnable() {
            navigationTimer.Start();
            peep.DesiredVelocity = Random.insideUnitCircle.normalized;
        }

        protected void Update() {
            if (navigationTimer.IsActive) return;
            navigationTimer.Start();

            var position = peep.Position;

            // Check for colliders in the sense radius.
            var hits = Physics.OverlapSphereNonAlloc(
                position,
                senseRadius,
                COLLIDER_RESULTS,
                navigationMask.value);

            // There will always be at least one hit on our own collider.
            if (hits <= 1) return;

            var separation = Vector3.zero;
            var alignment = Vector3.zero;
            var cohesion = Vector3.zero;
            for (int i = 0; i < hits; i++) {
                var hit = COLLIDER_RESULTS[i];
                // Ignore self.
                if (hit.attachedRigidbody != null && hit.attachedRigidbody.gameObject == peep.gameObject) continue;

                // Always repel from walls.
                var repel = true;
                if (hit.CompareTag(peepTag)) {
                    // Sensed another peep.
                    var otherPeed = hit.attachedRigidbody.GetComponent<PeepController>();
                    // Ignore peeps that are not from this group.
                    if (otherPeed.Group != peep.Group) continue;
                    repel = repelFromSameGroup;
                }

                var closestPoint = hit.ClosestPoint(position);
                closestPoint.y = 0f;
                DebugDraw.DrawLine(
                    position + Vector3.up,
                    closestPoint + Vector3.up,
                    Color.cyan,
                    navigationTimer.Duration / 2);
                var direction = closestPoint - position;

                var magnitude = direction.magnitude;
                var distancePercent = repel
                    ? Mathf.InverseLerp(peep.SelfRadius, senseRadius, magnitude)
                    // Inverse attraction factor so peeps won't be magnetized to
                    // each other without being able to separate.
                    : Mathf.InverseLerp(senseRadius, peep.SelfRadius, magnitude);

                // Make sure the distance % is not 0 to avoid division by 0.
                distancePercent = Mathf.Max(distancePercent, 0.01f);

                // Force is stronger when distance percent is closer to 0 (1/x-1).
                var forceWeight = 1f / distancePercent - 1f;

                // Angle between forward to other collider.
                var angle = transform.forward.GetAngleBetweenXZ(direction);
                var absAngle = Mathf.Abs(angle);
                if (absAngle > 90f) {
                    // Decrease weight of colliders that are behind the peep.
                    // The closer to the back, the lower the weight.
                    var t = Mathf.InverseLerp(180f, 90f, absAngle);
                    forceWeight *= Mathf.Lerp(0.1f, 1f, t);
                }

                
                if (hit.CompareTag(peepTag)) {
                    var otherPeed = hit.attachedRigidbody.GetComponent<PeepController>();
                    alignment += otherPeed.transform.forward.GetWithMagnitude(forceWeight);
                    cohesion += otherPeed.Position.GetWithMagnitude(forceWeight);
                }
                
                direction = direction.normalized * forceWeight;
                if (repel) {
                    separation -= direction;
                    DebugDraw.DrawArrowXZ(
                        position + Vector3.up,
                        -direction * 3f,
                        1f,
                        30f,
                        Color.magenta,
                        navigationTimer.Duration / 2);
                } else {
                    separation += direction;
                    DebugDraw.DrawArrowXZ(
                        position + Vector3.up,
                        direction * 3f,
                        1f,
                        30f,
                        Color.green,
                        navigationTimer.Duration / 2);
                }
            }

            var desiredVelocity = (weights[0] * separation + weights[1] * alignment + weights[2] * cohesion) / (weights[0] + weights[1] + weights[2]);
            if (desiredVelocity.sqrMagnitude < 0.1f) return;

            peep.DesiredVelocity = desiredVelocity.normalized.ToVector2XZ();
        }

        protected void OnDrawGizmos() {
            var angle = transform.forward.GetAngleXZ();
            DebugDraw.GizmosDrawSector(
                transform.position,
                senseRadius,
                180f + angle,
                -180f + angle,
                Color.green);
        }
    }
}
