using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Automation;
using UnityAutomation;

public class DestinationOnEnterTrigger : MonoBehaviour
{
    public bool playerEntered;
    // Start is called before the first frame update
    void Start()
    {
        playerEntered = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerEntered = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {
            playerEntered = false;
        }

    }
}
