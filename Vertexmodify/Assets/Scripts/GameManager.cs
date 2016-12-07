using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager> {
    
    private float _counterTime;
    public float countedTime{ get; set; }


    private int _numKeys;
    public int NumKeys{ get; set;}

    public int maxKeys;

    //private float maxTime = 5*60;

	// Use this for initialization
	void Start () {
        countedTime = 0;
        maxKeys = GameObject.FindGameObjectsWithTag("Key").Length;
	}
	

	void Update () {
        countedTime += Time.deltaTime;
	}

    public void RestartLevel()
    {

//        SceneManager.LoadScene("sagen");
//        TimeRemaining = maxTime;
//        NumKeys = 0;
//        maxKeys = GameObject.FindGameObjectsWithTag("Key").Length;
    }
}
