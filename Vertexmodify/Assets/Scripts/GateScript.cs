using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GateScript : MonoBehaviour
{
    
	private Text winText;
	//public Camera main; 
     

	//GameObject UIMain;
	//public Canvas UIMain; 

	private GameObject timerLabel;

    
	private GameObject keysLabel;

    //[SerializeField]
	private GameObject checkmark;
	//[SerializeField]
	private GameObject key;
	//[SerializeField]
	private GameObject timer;


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
		//UIMain = GameObject.FindGameObjectWithTag("UIMain"); 
		timerLabel = GameObject.Find("TimerText"); 
		keysLabel = GameObject.Find ("KeyText"); 
		checkmark = GameObject.Find ("Checkmark"); 
		key = GameObject.Find ("KeyIcon"); 
		timer = GameObject.Find ("TimerIcon"); 
		winText = GameObject.Find ("WinText").GetComponent<Text>(); 
		//main = Camera.; 

		//main.enabled = false; 
		//UIMain.enabled = true; 
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
		//GameObject.FindGameObjectWithTag("Player").SetActive(false);
		//GameObject.Destroy(GameObject.FindGameObjectWithTag("Player"));
	
       // GameObject.FindGameObjectWithTag("MainCamera").SetActive(true);
		//main.SetActive(true);
		//main.enabled = true; 
		timerLabel.SetActive(false); 
		keysLabel.SetActive(false); 
		timer.SetActive(false); 
		key.SetActive(false); 
		checkmark.SetActive(false); 
        GameObject.Destroy(checkmark); 
	
		//UIMain.enabled = false; 
		winText.enabled = true; 

    }
}
