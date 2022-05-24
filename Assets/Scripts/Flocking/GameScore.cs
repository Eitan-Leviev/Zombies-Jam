using UnityEngine;
using UnityEngine.UI;

namespace Flocking
{
    public class GameScore : MonoBehaviour
    {
        private void Awake()
        {
            if (gameObject.name == "HumansScore txt")
            {
                GetComponent<Text>().text = GameManager.HumansScore.ToString();
                return;
            }
            GetComponent<Text>().text = GameManager.ZombiesScore.ToString();
        }
    }
}