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

    private int safePeepsCounter = 0;

    private const int peepsNum = 8;

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
                safePeepsCounter++;
            }
            
            // if leader got safe
            if (other.gameObject.layer == LayerMask.NameToLayer("Leader"))
            {
                if (safePeepsCounter == peepsNum)
                {
                    print("HUMANS WON");
                    other.gameObject.SetActive(false);
                }
                // DebugLog.Log("Leader", Color.blue, other);
            }
        }
    }
}
