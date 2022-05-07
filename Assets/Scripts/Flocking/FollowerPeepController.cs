using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Avrahamy;
using Avrahamy.Math;
using BitStrap;

namespace Flocking
{
    public class FollowerPeepController : MonoBehaviour
    {
        [SerializeField]
        PeepController peep;

        [SerializeField]
        float senseRadius = 2f;

        [SerializeField]
        [Range(0, 180)]
        private float visionAngle = 120f;

        [SerializeField]
        PassiveTimer navigationTimer;

        [SerializeField]
        LayerMask navigationMask;

        [TagSelector]
        [SerializeField]
        string peepTag;

        [SerializeField]
        bool repelFromSameGroup;

        [SerializeField]
        private Weights weights = new Weights();

        [Serializable]
        private class Weights
        {
            [SerializeField]
            [Min(0)]
            public int sep = 1;

            [SerializeField]
            [Min(0)]
            public int alg = 1;

            [SerializeField]
            [Min(0)]
            public int coh = 1;
            
            public int Sum => sep + alg + coh;
        }


        private static readonly Collider[] COLLIDER_RESULTS = new Collider[10];

        protected void Reset()
        {
            if (peep == null)
            {
                peep = GetComponent<PeepController>();
            }
        }

        protected void OnEnable()
        {
            navigationTimer.Start();
            // peep.DesiredVelocity = Random.insideUnitCircle.normalized;
        }

        protected void Update()
        {
            if (navigationTimer.IsActive)
            {
                return;
            }

            navigationTimer.Start();

            var position = peep.Position;

            // Check for colliders in the sense radius.
            var hits = Physics.OverlapSphereNonAlloc(
                position,
                senseRadius,
                COLLIDER_RESULTS,
                navigationMask.value);

            // There will always be at least one hit on our own collider.
            if (hits <= 1)
            {
                // peep.DesiredVelocity *= 0.8f;
                return;
            }

            var desiredVelocity = DesiredVelocity(hits, position);

            if (desiredVelocity.sqrMagnitude < 0.1f)
            {
                return;
            }

            peep.DesiredVelocity = desiredVelocity.normalized.ToVector2XZ();
        }

        private Vector3 DesiredVelocity(int hits, Vector3 position)
        {
            var separation = Vector3.zero;
            var alignment = Vector3.zero;
            var cohesion = Vector3.zero;
            var coCount = 0;
            var sumWeights = 0f;

            for (int i = 0; i < hits; i++)
            {
                var hit = COLLIDER_RESULTS[i];
                // Ignore self.
                if (hit.attachedRigidbody != null &&
                    hit.attachedRigidbody.gameObject == peep.gameObject)
                {
                    continue;
                }

                // TODO: do we want this? specifically for zombies?
                // Always repel from walls.
                var repel = true;

                if (hit.CompareTag(peepTag))
                {
                    // Sensed another peep.
                    var otherPeed = hit.attachedRigidbody.GetComponent<PeepController>();
                    // Ignore peeps that are not from this group.
                    if (otherPeed.Group != peep.Group) // TODO: Zombies chase, humans run away
                    {
                        continue;
                    }

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
                var forceWeight = 1f / distancePercent - 1f; // TODO: the article says 1/r^2?
                // TODO: also try linear -
                // var forceWeight = 1 / distancePercent * distancePercent;
                // var forceWeight = 1 - distancePercent;
                var simpleWeight = 1f;

                // Angle between forward to other collider.
                // TODO: do we want the angle to do decide in our game at all? maybe different for each team?
                var angle = transform.forward.GetAngleBetweenXZ(direction);
                var absAngle = Mathf.Abs(angle);
                if (absAngle > visionAngle)
                {
                    // Decrease weight of colliders that are behind the peep.
                    // The closer to the back, the lower the weight.
                    var t = Mathf.InverseLerp(180f, visionAngle, absAngle);
                    forceWeight *= Mathf.Lerp(0.1f, 1f, t);
                    simpleWeight *= Mathf.Lerp(0.1f, 1f, t);
                }


                // TODO: should be refactored with the first if
                if (hit.CompareTag(peepTag))
                {
                    var otherPeed = hit.attachedRigidbody.GetComponent<PeepController>();
                    if (otherPeed.Group != peep.Group)
                    {
                        continue;
                    }

                    // TODO: only if same group for now, require thinking of desired state
                    alignment += simpleWeight * otherPeed.transform.forward;//.GetWithMagnitude(forceWeight);
                    // alignment += otherPeed.transform.forward; //.GetWithMagnitude(forceWeight); //TODO: should we apply weight for this 2? 
                    // cohesion += simpleWeight > 0.8f ? closestPoint : Vector3.zero; //.GetWithMagnitude(distancePercent);
                    cohesion += simpleWeight * closestPoint;//.GetWithMagnitude(distancePercent);
                    coCount++;
                    sumWeights += simpleWeight;
                }

                direction = direction.normalized * forceWeight;
                if (repel)
                {
                    separation -= direction;
                    DebugDraw.DrawArrowXZ(
                        position + Vector3.up,
                        -direction * 3f,
                        1f,
                        30f,
                        Color.magenta,
                        navigationTimer.Duration / 2);
                }
                else
                {
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

            // TODO: change to desired velocity instead of forward?
            alignment = weights.alg * (alignment / coCount - peep.transform.forward);
            // alignment = weights.alg * (alignment / sumWeights - peep.transform.forward);
            // alignment = alignment.normalized;
            // cohesion = cohesion / coCount - position;
            cohesion = weights.coh * (cohesion / coCount - position); // TODO: coh = 9/100

            separation = weights.sep * separation;
            
            

            var desiredVelocity = (cohesion + alignment + separation) / weights.Sum;
            DebugDraw.DrawArrowXZ(
                position + Vector3.up,
                desiredVelocity,
                1f,
                30f,
                Color.black,
                navigationTimer.Duration / 2);
            // return desiredVelocity;

            // cohesion = cohesion.normalized;
            // alignment = alignment.normalized;
            // separation = separation.normalized;

            DebugDraw.DrawArrowXZ(
                position + Vector3.up,
                separation,
                1f,
                30f,
                Color.cyan,
                navigationTimer.Duration / 2);
            DebugDraw.DrawArrowXZ(
                position + Vector3.up,
                alignment,
                1f,
                30f,
                Color.green,
                navigationTimer.Duration / 2);
            DebugDraw.DrawArrowXZ(
                position + Vector3.up,
                cohesion,
                1f,
                30f,
                Color.yellow,
                navigationTimer.Duration / 2);

            // desiredVelocity =
                // (weights.sep * separation + weights.alg * alignment + weights.coh * cohesion) /
                // (weights.alg + weights.sep + weights.coh);
            return desiredVelocity;
        }

        protected void OnDrawGizmos()
        {
            var angle = transform.forward.GetAngleXZ();
            DebugDraw.GizmosDrawSector(
                transform.position,
                senseRadius,
                visionAngle + angle,
                -visionAngle + angle,
                Color.green);
            DebugDraw.GizmosDrawBackSector(
                transform.position,
                (direction) =>
                {
                    var a = transform.forward.GetAngleBetweenXZ(direction);
                    var absAngle = Mathf.Abs(a);
                    var t = Mathf.InverseLerp(180f, visionAngle, absAngle);

                    return senseRadius * Mathf.Lerp(0.1f, 1f, t);
                },
                visionAngle + angle,
                -visionAngle + angle,
                Color.red);
        }
    }
}