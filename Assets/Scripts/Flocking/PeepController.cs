using UnityEngine;
using Avrahamy;
using Avrahamy.Math;

namespace Flocking {
    public class PeepController : MonoBehaviour {
        [SerializeField] int group;
        [SerializeField] float maxSpeed = 10f;
        [Range(0f, 1f)]
        [SerializeField] float rotationSpeed = 0.2f;
        [SerializeField] float acceleration = 2f;
        [SerializeField] float deceleration = 1f;
        [SerializeField] float minSqrSpeed = 0.1f;

        public int Group {
            get {
                return group;
            }
            set {
                group = value;
            }
        }

        public float MaxSpeed {
            get {
                return maxSpeed;
            }
            set {
                maxSpeed = value;
            }
        }

        public float Acceleration {
            get {
                return acceleration;
            }
            set {
                acceleration = value;
            }
        }

        public Vector3 Forward => body.transform.forward;

        public Vector3 Position {
            get {
                return body.position;
            }
            set {
                body.position = value;
            }
        }

        public Vector2 Velocity {
            get {
                return body.velocity.ToVector2XZ();
            }
            set {
                body.velocity = value.ToVector3XZ();
            }
        }

        public Vector2 DesiredVelocity {
            get {
                return desiredVelocity;
            }
            set {
                desiredVelocity = value;
            }
        }

        public float SelfRadius {
            get {
                return selfRadius;
            }
        }

        private Rigidbody body;
        private Vector2 desiredVelocity;
        private float selfRadius;

        protected void Awake() {
            body = GetComponent<Rigidbody>();
            selfRadius = GetComponentInChildren<CapsuleCollider>().radius;
        }

        protected void Update() {
            DebugDraw.DrawArrowXZ(
                Position + Vector3.up,
                desiredVelocity.ToVector3XZ() * 3f,
                1f,
                30f,
                group == 0 ? Color.red : Color.blue);
        }

        protected void FixedUpdate() {
            var currentVelocity = Velocity;
            if (desiredVelocity.sqrMagnitude > 0.1f) {
                var targetRotation = Quaternion.LookRotation(desiredVelocity.ToVector3XZ(), Vector3.up);
                body.rotation = Quaternion.LerpUnclamped(body.rotation, targetRotation, rotationSpeed);
                Velocity = Vector2.MoveTowards(currentVelocity, desiredVelocity * maxSpeed, acceleration);
            } else if (currentVelocity.sqrMagnitude > minSqrSpeed) {
                Velocity = Vector2.MoveTowards(currentVelocity, Vector2.zero, deceleration);
            } else {
                Velocity = Vector2.zero;
            }
        }
    }
}
