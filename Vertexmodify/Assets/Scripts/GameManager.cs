using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager> {
    
    private float _timeRemaining;
    public float TimeRemaining{ get; set; }

    private int _numKeys;
    public int NumKeys{ get; set;}

    public int maxKeys;

    private float maxTime = 5*60;

	// Use this for initialization
	void Start () {
        TimeRemaining = maxTime;
        maxKeys = GameObject.FindGameObjectsWithTag("Key").Length;
	}
	
	// Update is called once per frame
	void Update () {
        TimeRemaining -= Time.deltaTime;

        if(TimeRemaining <= 0){
            RestartLevel();
        }
            
	}

    public void RestartLevel()
    {
        SceneManager.LoadScene("sagen");
        TimeRemaining = maxTime;
        NumKeys = 0;
        maxKeys = GameObject.FindGameObjectsWithTag("Key").Length;
    }
}
