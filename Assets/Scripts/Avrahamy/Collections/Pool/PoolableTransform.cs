using UnityEngine;

namespace Avrahamy.Collections {
    public class PoolableTransform : OptimizedBehaviour, IPoolable {
        public void ReturnSelf() {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Expected params: (Vector2)position, [optional](float)angle
        /// </summary>
        public int Activate(params object[] activateParams) {
            float angle = 0;

            var index = 0;
            var position = (Vector3)activateParams[index++];
            if (activateParams.Length > index) {
                angle = (float)activateParams[index++];
            }

            transform.localPosition = position;
            transform.localRotation = Quaternion.Euler(0, angle, 0);

            return index;
        }
    }
}
