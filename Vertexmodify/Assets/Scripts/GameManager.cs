using UnityEngine;
using System.Collections;

public class GameManager : Singleton<GameManager> {

    private float timeRemaining;

    public float TimeRemaining{ get; set; }

    private int _numKeys;

    public int NumKeys{ get; set; }

    private float maxTime = 5*60;

	// Use this for initialization
	void Start () {
        TimeRemaining = maxTime;
	}
	
	// Update is called once per frame
	void Update () {
        TimeRemaining -= Time.deltaTime;

        if(TimeRemaining <= 0){
            Application.LoadLevel(Application.loadedLevel);
            TimeRemaining = maxTime;
        }
            
	}
}
