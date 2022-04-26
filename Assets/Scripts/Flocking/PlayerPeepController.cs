using UnityEngine;
using UnityEngine.InputSystem;

namespace Flocking {
    public class PlayerPeepController : MonoBehaviour {
        [SerializeField] PeepController peep;
        [SerializeField] InputActionReference moveAction;

        protected void Reset() {
            if (peep == null) {
                peep = GetComponent<PeepController>();
            }
        }

        protected void OnEnable() {
            moveAction.action.Enable();
            moveAction.action.started += OnMove;
            moveAction.action.performed += OnMove;
            moveAction.action.canceled += OnMove;
        }

        protected void OnDisable() {
            moveAction.action.started -= OnMove;
            moveAction.action.performed -= OnMove;
            moveAction.action.canceled -= OnMove;
        }

        private void OnMove(InputAction.CallbackContext context) {
            var value = context.action.ReadValue<Vector2>();
            peep.DesiredVelocity = value;
        }
    }
}
