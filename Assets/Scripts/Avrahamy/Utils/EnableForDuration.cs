using UnityEngine;

namespace Avrahamy.Utils {
    public class EnableForDuration : MonoBehaviour {
        [SerializeField] float disableAfterTimeout;

        protected void OnEnable() {
            if (disableAfterTimeout > 0) {
                this.DelaySeconds(Disable, disableAfterTimeout);
            }
        }

        public void Disable() {
            gameObject.SetActive(false);
        }
    }
}
