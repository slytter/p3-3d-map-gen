using UnityEngine;
using System.Collections;

public class GateScript : MonoBehaviour
{

    // Creating a collider variable that is set to the collider attached to the gate
    // otherwise it is not possible to access the isTrigger function
    Collider gateCollider;

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            EnterGate();
        }
    }

    void Start()
    {
        gateCollider = gameObject.GetComponent<Collider>();
        gateCollider.isTrigger = false;
    }

    void Update()
    {
        // Ensures that the gate can only be entered when all keys are collected
        if (GameManager.instance.NumKeys == GameManager.instance.maxKeys)
        {
            gateCollider.isTrigger = true;
 
        }
    }

    private void EnterGate()
    {
        GameManager.instance.RestartLevel();
    }
}
