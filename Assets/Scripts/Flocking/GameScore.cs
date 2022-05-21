using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScore : MonoBehaviour
{
    private void Awake()
    {
        if (gameObject.name == "HumansScore txt")
        {
            GetComponent<Text>().text = GameManager.HumansScore.ToString();
        }
        else
        {
            GetComponent<Text>().text = GameManager.ZombiesScore.ToString();
        }
    }
}
