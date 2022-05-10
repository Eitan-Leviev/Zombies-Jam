using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Text>().text = SafeZone.SafePeepsCounter.ToString();
        GetComponent<AudioSource>().Play();
    }

    public void DestroyMe()
    {
        this.gameObject.SetActive(false);
    }
}
