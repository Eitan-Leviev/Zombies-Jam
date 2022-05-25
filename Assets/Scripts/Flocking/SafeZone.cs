using Avrahamy;
using BitStrap;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Flocking
{
    public class SafeZone : MonoBehaviour
    {
        #region Constants

        private static readonly int plopPos = Shader.PropertyToID("_PlopPos");
        private static readonly int resetPlop = Shader.PropertyToID("_ResetPlop");
        private static readonly int lastPlop = Shader.PropertyToID("_LastPlop");

        #endregion

        #region Inspector

        [TagSelector]
        [SerializeField]
        string peepTag;

        [SerializeField]
        private int zombieGroup = 0;

        [SerializeField]
        private GameObject humanGroupHolder;

        [Header("Shader Effects")]
        [SerializeField]
        private PassiveTimer waveTimer;

        #endregion

        public static int PeepsNum { get; set; } = 15;
        public static int SafePeepsCounter { get; } = 0;

        private Material _myMat;
        private bool _bool = true;

        #region MonoBehaviour

        private void Awake()
        {
            _myMat = GetComponent<MeshRenderer>().material;
            PeepsNum = CheckHumansNum();
        }

        private void Update()
        {
            if (waveTimer.IsSet && !waveTimer.IsActive)
            {
                waveTimer.Clear();
                _myMat.SetInt(resetPlop, 0);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // if (!_bool)
            // {
            //     return;
            // }

            if (other.CompareTag(peepTag))
            {
                var peepController = other.GetComponentInParent<PeepController>();
                // if peep got safe
                if (peepController.Group != zombieGroup && other.gameObject.layer != LayerMask.NameToLayer("Leader"))
                {
                    _myMat.SetVector(plopPos, peepController.transform.position);
                    _myMat.SetInt(resetPlop, 1);
                    _myMat.SetFloat(lastPlop, Time.time);
                    waveTimer.Start();

                    // disappear 
                    peepController.gameObject.SetActive(false);
                    // count safe peeps
                    PeepsNum--;
                }

                // if leader got safe
                // if (peepController.Group != 0 && other.gameObject.layer == LayerMask.NameToLayer("Leader"))
                // {
                //     if (PeepsNum == 1)
                //     {
                //         GameManager.HumansScore++;
                //         DebugLog.Log(GameManager.HumansScore);
                //         DebugLog.Log("HUMANS WON");
                //         _bool = false;
                //         SceneManager.LoadScene(0);
                //     }
                // }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!_bool)
            {
                return;
            }

            // if leader got safe
            if (other.gameObject.layer == LayerMask.NameToLayer("Leader"))
            {
                var peepController = other.GetComponentInParent<PeepController>();

                if (PeepsNum == 1 && peepController.Group != zombieGroup)
                {
                    GameManager.HumansScore++;
                    GameManager.WhoWon = 0;
                    DebugLog.Log(GameManager.HumansScore);
                    Debug.Log("HUMANS WON", other);
                    _bool = false;
                    SceneManager.LoadScene(0);
                }
            }
        }

        #endregion

        #region Private Methods

        private int CheckHumansNum()
        {
            var blueParent = humanGroupHolder;
            // var blueParent = GameObject.Find("Blue");
            int humansNum = 0;
            for (int i = 0; i < blueParent.transform.childCount; i++)
            {
                GameObject blue = blueParent.transform.GetChild(i).gameObject;
                if (blue.activeSelf)
                {
                    humansNum++;
                }
            }

            return humansNum;
        }

        #endregion
    }
}