using System;
using UnityEngine;
using Random = UnityEngine.Random;
using Avrahamy;
using Avrahamy.Math;
using BitStrap;
using UnityEngine.SceneManagement;

namespace Flocking
{
    public class ZombieConvert : MonoBehaviour
    {
        #region Inspector

        [SerializeField]
        PeepController peep;

        [TagSelector]
        [SerializeField]
        string peepTag;

        [SerializeField]
        private int zombieGroup = 0;

        [SerializeField]
        private float biteDistance;

        [SerializeField]
        private GameObject zombiePeep;

        #endregion

        #region Public Methods

        public bool CheckConvert(PeepController other, float distance)
        {
            // DebugLog.Log(LogTag.Gameplay, $"Checking bite: {distance}", other);
            if (distance < biteDistance && other.gameObject.activeSelf)
            {
                Convert(other.Position, other.transform.rotation, other);
                return true;
            }

            return false;
        }

        #endregion

        #region MonoBehaviour

        protected void Reset()
        {
            if (peep == null)
            {
                peep = GetComponent<PeepController>();
            }
        }

        private void OnEnable()
        {
            if (peep == null)
            {
                peep = GetComponent<PeepController>();
            }

            peep.Group = zombieGroup;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag(peepTag))
            {
                var otherPeep = collision.gameObject.GetComponent<PeepController>();
                if (otherPeep.Group != zombieGroup && otherPeep.gameObject.activeSelf)
                {
                    Convert(collision.transform.position, collision.transform.rotation, otherPeep);
                }
            }
        }

        #endregion

        #region Private Methods

        private void Convert(Vector3 pos, Quaternion rot, PeepController otherPeep)
        {
            if (otherPeep.gameObject.layer == LayerMask.NameToLayer("Leader"))
            {
                GameManager.ZombiesScore++;
                DebugLog.Log(GameManager.ZombiesScore);
                print("ZOMBIES WON");
                SceneManager.LoadScene(0);
            }

            SafeZone.PeepsNum--;
            // Debug.Log(SafeZone.peepsNum, otherPeep);
            // DebugLog.Log(LogTag.Gameplay, "Zombie Tagged", otherPeep);
            otherPeep.gameObject.SetActive(false);
            var newZombie = Instantiate(zombiePeep,
                pos,
                rot,
                transform.parent);
        }

        #endregion
    }
}