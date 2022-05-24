using System;
using System.Collections;
using System.Collections.Generic;
using Avrahamy;
using BitStrap;
using Flocking;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SafeZone : MonoBehaviour
{
    [SerializeField]
    private LayerMask peeps;

    [SerializeField]
    private LayerMask leaderMask;

    [TagSelector]
    [SerializeField]
    string peepTag;

    [SerializeField] 
    private GameObject scorePrefab;
    
    public static int peepsNum = 15;

    [Header("Shader Effects")]
    [SerializeField]
    private PassiveTimer waveTimer;
    
    private GameObject canvas;
    public static int SafePeepsCounter = 0;

    private Material _myMat;
    private static readonly int plopPos = Shader.PropertyToID("_PlopPos");
    private static readonly int resetPlop = Shader.PropertyToID("_ResetPlop");
    private static readonly int lastPlop = Shader.PropertyToID("_LastPlop");
    private bool _bool = true;

    private void Awake()
    {
        canvas = GameObject.Find("Canvas");
        _myMat = GetComponent<MeshRenderer>().material;
        peepsNum = CheckHumansNum();
    }

    private void Update()
    {
        // print("peepsNum: " + SafeZone.peepsNum);
        // print(CheckHumansNum());

        if (waveTimer.IsSet && !waveTimer.IsActive)
        {
            waveTimer.Clear();
            _myMat.SetInt(resetPlop, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_bool)
        {
            return;
        }
        if (other.CompareTag(peepTag))
        {
            var peepController = other.GetComponentInParent<PeepController>();
            // if peep got safe
            if (peepController.Group != 0 && other.gameObject.layer != LayerMask.NameToLayer("Leader"))
            {
                _myMat.SetVector(plopPos, peepController.transform.position);
                _myMat.SetInt(resetPlop, 1);
                _myMat.SetFloat(lastPlop, Time.time);
                waveTimer.Start();

                // disappear 
                peepController.gameObject.SetActive(false);
                // count safe peeps
                peepsNum--;
                // SafePeepsCounter++;
                // print("SafePeepsCounter: " + SafePeepsCounter);
                // display
                Instantiate(scorePrefab, canvas.transform);
            }
            
            // if leader got safe
            if (peepController.Group != 0  && other.gameObject.layer == LayerMask.NameToLayer("Leader"))
            {
                // print("SafePeepsCounter "+SafePeepsCounter+"peepsNum "+peepsNum);
                if (peepsNum == 1)
                {
                    GameManager.HumansScore++;
                    print(GameManager.HumansScore);
                    print("HUMANS WON");
                    _bool = false;
                    SceneManager.LoadScene(0);
                }
                // if (SafePeepsCounter == peepsNum)
                // {
                //     GameManager.HumansScore++;
                //     print(GameManager.HumansScore);
                //     print("HUMANS WON");
                //     SceneManager.LoadScene(0);
                // }
                // DebugLog.Log("Leader", Color.blue, other);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!_bool)
        {
            return;
        }
        // print("SafePeepsCounter "+SafePeepsCounter+"peepsNum "+peepsNum);
        // if leader got safe
        if (other.gameObject.layer == LayerMask.NameToLayer("Leader"))
        {
            var peepController = other.GetComponentInParent<PeepController>();

            if (peepsNum == 1 && peepController.Group != 0)
            {
                GameManager.HumansScore++;
                print(GameManager.HumansScore);
                Debug.Log("HUMANS WON", other);
                _bool = false;
                SceneManager.LoadScene(0);
            }
            // if (SafePeepsCounter == peepsNum)
            // {
            //     GameManager.HumansScore++;
            //     print(GameManager.HumansScore);
            //     print("HUMANS WON");
            //     SceneManager.LoadScene(0);
            // }
        }
    }

    private int CheckHumansNum()
    {
        var blueParent = GameObject.Find("Blue");
        int HumansNum = 0;
        for(int i = 0; i < blueParent.transform.childCount; i++)
        {
            GameObject blue = blueParent.transform.GetChild(i).gameObject;
            if (blue.activeSelf)
            {
                HumansNum++;
            }
        }
        return HumansNum;
    }
}
