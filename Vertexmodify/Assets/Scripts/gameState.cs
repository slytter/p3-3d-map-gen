using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class gameState : MonoBehaviour {

	void Awake(){
		Debug.Log("gamestate init");
		GameObject.DontDestroyOnLoad (this.gameObject);
	}
    
	public Texture2D image;

	public string test = "yo";
    public string chosenImage = "";


}
