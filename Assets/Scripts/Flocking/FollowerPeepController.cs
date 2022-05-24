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

        [SerializeField]
        private LayerMask leaderMask;

        [TagSelector]
        [SerializeField]
        string peepTag;

        [SerializeField]
        bool repelFromSameGroup;

        [SerializeField]
        private bool repelFromOtherGroup;

        [SerializeField]
        private float otherGroupWeight = 1.5f;

        [SerializeField]
        private Weights weights = new Weights();

        [Serializable]
        private class Weights
        {
            [SerializeField]
            [Min(0)]
            public int separation = 4;

            [SerializeField]
            [Min(0)]
            public int alignment = 3;

            [SerializeField]
            [Min(0)]
            public int cohesion = 1;

            [SerializeField]
            [Min(0)]
            public int self = 1;

            public int Sum => separation + alignment + cohesion + self;
        }


        private static readonly Collider[] COLLIDER_RESULTS = new Collider[10];
        [SerializeField]
        private float distanceToLeader = 1;

        [SerializeField]
        [Min(0)]
        private float slowDistanceFromLeader;

        [SerializeField]
        [Min(0)]
        private float leaderWeight = 2;

        [SerializeField]
        private bool ignoreWalls = false;

        [SerializeField]
        private int zombieGroup = 0;

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

            var leader = Vector3.zero;

            var hits = Physics.OverlapSphereNonAlloc(
                position,
                senseRadius,
                COLLIDER_RESULTS,
                leaderMask.value);
            if (hits > 0)
            {
                for (int i = 0; i < hits; i++)
                {
                    var hit = COLLIDER_RESULTS[i];
                    if (hit.attachedRigidbody != null &&
                        hit.attachedRigidbody.gameObject == peep.gameObject)
                    {
                        continue;
                    }

                    var otherPeed = hit.attachedRigidbody.GetComponent<PeepController>();
                    if (otherPeed.Group != peep.Group)
                    {
                        continue;
                    }

                    leader += LeaderFollow(otherPeed, position);
                }
            }

            // Check for colliders in the sense radius.
            hits = Physics.OverlapSphereNonAlloc(
                position,
                senseRadius,
                COLLIDER_RESULTS,
                navigationMask.value);

            var desiredVelocity = Vector3.zero;
            // There will always be at least one hit on our own collider.
            if (hits > 1)
            {
                desiredVelocity = DesiredVelocity(hits, position);
            }

            if (!leader.Equals(Vector3.zero))
            {
                desiredVelocity = (desiredVelocity + leaderWeight * leader) / (leaderWeight + 1);
            }


            if (desiredVelocity.sqrMagnitude < 0.5f)
            {
                return;
            }

            peep.DesiredVelocity = desiredVelocity.normalized.ToVector2XZ();
        }

        private Vector3 LeaderFollow(PeepController leader, Vector3 position)
        {
            var followTarget = leader.Position - leader.Forward * distanceToLeader;
            followTarget.y = 0f;
            DebugDraw.DrawLine(
                position + Vector3.up,
                followTarget + Vector3.up,
                Color.yellow,
                navigationTimer.Duration / 2);
            var targetOffset = followTarget - position;

            var distance = targetOffset.magnitude;
            var rampedSpeed = peep.MaxSpeed * (distance / slowDistanceFromLeader);
            var clippedSpeed = Mathf.Min(rampedSpeed, peep.MaxSpeed);
            var desiredVelocity = (clippedSpeed / distance) * targetOffset;
            DebugDraw.DrawArrowXZ(
                position + Vector3.up,
                desiredVelocity - peep.Forward, //Velocity.ToVector3XZ(),
                1f,
                30f,
                Color.yellow,
                navigationTimer.Duration / 2);
            return desiredVelocity - peep.Forward; //Velocity.ToVector3XZ();
        }

        private Vector3 DesiredVelocity(int hits, Vector3 position)
        {
            var separation = Vector3.zero;
            var alignment = Vector3.zero;
            var cohesion = Vector3.zero;
            var otherSep = Vector3.zero;
            var otherAli = Vector3.zero;
            var otherCoh = Vector3.zero;
            var coCount = 0;
            var otCount = 0;

            for (int i = 0; i < hits; i++)
            {
                var hit = COLLIDER_RESULTS[i];
                // Ignore self.
                if (hit.attachedRigidbody != null &&
                    hit.attachedRigidbody.gameObject == peep.gameObject)
                {
                    continue;
                }

                // Always repel from walls.
                var repel = true;

                if (hit.CompareTag(peepTag))
                {
                    // Sensed another peep.
                    var otherPeed = hit.attachedRigidbody.GetComponent<PeepController>();
                    // Ignore peeps that are not from this group.
                    repel = otherPeed.Group != peep.Group ? repelFromOtherGroup : repelFromSameGroup;
                }
                else if (ignoreWalls)
                {
                    continue;
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
                var simpleWeight = 1f;

                // Angle between forward to other collider.
                var angle = peep.Forward.GetAngleBetweenXZ(direction);
                var absAngle = Mathf.Abs(angle);
                if (absAngle > visionAngle)
                {
                    // Decrease weight of colliders that are behind the peep.
                    // The closer to the back, the lower the weight.
                    var t = Mathf.InverseLerp(180f, visionAngle, absAngle);
                    forceWeight *= Mathf.Lerp(0.1f, 1f, t);
                    simpleWeight *= Mathf.Lerp(0.1f, 1f, t);
                }


                if (hit.CompareTag(peepTag))
                {
                    var otherPeed = hit.attachedRigidbody.GetComponent<PeepController>();
                    if (otherPeed.Group == peep.Group || !repelFromOtherGroup)
                    {
                        alignment += simpleWeight * otherPeed.Forward;
                        cohesion += simpleWeight * closestPoint; //.GetWithMagnitude(distancePercent);
                        coCount++;
                    }
                    else
                    {
                        otherCoh += simpleWeight * closestPoint; //.GetWithMagnitude(distancePercent);
                        otCount++;
                    }
                    if (peep.Group == zombieGroup && otherPeed.Group != zombieGroup)
                    {
                        var zombie = GetComponent<ZombieConvert>();
                        if (zombie.CheckConvert(otherPeed, magnitude))
                        {
                            return Vector3.zero;
                        }
                    }
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
                        Color.white,
                        navigationTimer.Duration / 2);
                }
            }

            var self = weights.self * peep.Velocity.ToVector3XZ();
            separation = weights.separation * separation;

            Vector3 desiredVelocity;
            if (coCount == 0)
            {
                desiredVelocity = (self + separation) / (weights.self + weights.separation);
            }
            else
            {
                alignment = weights.alignment * (alignment / coCount - peep.Forward);
                cohesion = weights.cohesion * (cohesion / coCount - position);
                if (otCount > 0)
                {
                    cohesion -= otherGroupWeight * weights.cohesion * ((otherCoh / otCount) - position);
                }

                desiredVelocity = (cohesion + alignment + separation + self) / weights.Sum;
            }

            DebugDraw.DrawArrowXZ(
                position + Vector3.up,
                desiredVelocity,
                1f,
                30f,
                Color.black,
                navigationTimer.Duration / 2);

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