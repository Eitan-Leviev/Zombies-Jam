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
        [SerializeField]
        PeepController peep;
        
        [TagSelector]
        [SerializeField]
        string peepTag;

        [SerializeField]
        private int zombieGroup = 0;

        [SerializeField]
        private float biteDistance;
        // TODO: if we use that instead of collision we need to get the info from the followerScript

        [SerializeField]
        private GameObject zombiePeep;

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

        // private void OnCollisionEnter(Collision collision)
        // {
        //     if (collision.gameObject.CompareTag(peepTag))
        //     {
        //         var otherPeep = collision.gameObject.GetComponent<PeepController>();
        //         if (otherPeep.Group != zombieGroup && otherPeep.gameObject.activeSelf)
        //         {
        //             // if leader got caught
        //             if (collision.gameObject.name == "PeepLeaderBlue")
        //             {
        //                 GameManager.ZombiesScore++;
        //                 print(GameManager.ZombiesScore);
        //                 print("ZOMBIES WON");
        //                 SceneManager.LoadScene(0);
        //             }
        //             
        //             Convert(collision.transform.position, collision.transform.rotation, otherPeep);
        //         }
        //     }
        // }

        private void Convert(Vector3 pos, Quaternion rot, PeepController otherPeep)
        {
            SafeZone.peepsNum--;
            // Debug.Log(SafeZone.peepsNum, otherPeep);
            // DebugLog.Log(LogTag.Gameplay, "Zombie Tagged", otherPeep);
            otherPeep.gameObject.SetActive(false);
            var newZombie = Instantiate(zombiePeep,
                pos,
                rot,
                transform.parent);
        }
    }
}