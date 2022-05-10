using System;
using System.Collections;
using System.Collections.Generic;
using Avrahamy;
using BitStrap;
using Flocking;
using UnityEngine;

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
    
    private GameObject canvas;
    public static int SafePeepsCounter = 0;

    public const int peepsNum = 8;

    private void Awake()
    {
        canvas = GameObject.Find("Canvas");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(peepTag))
        {
            var peepController = other.GetComponentInParent<PeepController>();

            // if peep got safe
            if (peepController.Group != 0 && other.gameObject.layer != LayerMask.NameToLayer("Leader"))
            {
                // disappear 
                other.gameObject.SetActive(false);
                // count safe peeps
                SafePeepsCounter++;
                // display
                Instantiate(scorePrefab, canvas.transform);
            }
            
            // if leader got safe
            if (other.gameObject.layer == LayerMask.NameToLayer("Leader"))
            {
                if (SafePeepsCounter == peepsNum)
                {
                    print("HUMANS WON");
                    other.gameObject.SetActive(false);
                }
                // DebugLog.Log("Leader", Color.blue, other);
            }
        }
    }
}
