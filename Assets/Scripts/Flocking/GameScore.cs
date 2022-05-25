using System;
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
                if (GameManager.WhoWon == 0) // if humans won
                { GetComponent<Animator>().SetTrigger("winning"); }
                return;
            }
            if (GameManager.WhoWon == 1) // if zombies won
            { GetComponent<Animator>().SetTrigger("winning"); }
            GetComponent<Text>().text = GameManager.ZombiesScore.ToString();
        }
    }
    
    
}