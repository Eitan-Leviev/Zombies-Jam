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

    [SerializeField] private GameObject scorePrefab;
    
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

    private void Awake()
    {
        canvas = GameObject.Find("Canvas");
        _myMat = GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        // print("peepsNum: " + SafeZone.peepsNum);

        if (waveTimer.IsSet && !waveTimer.IsActive)
        {
            waveTimer.Clear();
            _myMat.SetInt(resetPlop, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
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
                SafePeepsCounter++;
                // print("SafePeepsCounter: " + SafePeepsCounter);
                // display
                Instantiate(scorePrefab, canvas.transform);
            }
            
            // if leader got safe
            if (peepController.Group != 0  && other.gameObject.layer == LayerMask.NameToLayer("Leader"))
            {
                if (SafePeepsCounter == peepsNum)
                {
                    print("HUMANS WON");
                    SceneManager.LoadScene(0);
                }
                // DebugLog.Log("Leader", Color.blue, other);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // if leader got safe
        if (other.gameObject.layer == LayerMask.NameToLayer("Leader"))
        {
            if (SafePeepsCounter == peepsNum)
            {
                print("HUMANS WON");
                SceneManager.LoadScene(0);
            }
        }
    }
}
