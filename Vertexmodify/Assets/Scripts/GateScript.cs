using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GateScript : MonoBehaviour
{
    [SerializeField]
    public Text winText;

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
        //winText.enabled = false;
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

    public void EnterGate()
    {
        winText.text = "You won, thank you for participating \nYou spent: "
        + Mathf.Round(GameManager.instance.countedTime) + " seconds to complete the map";


        GameObject.FindGameObjectWithTag("MainCamera").SetActive(false);
    }
}
