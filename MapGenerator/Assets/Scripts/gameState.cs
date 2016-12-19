using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class gameState : MonoBehaviour
{

	void Awake ()
	{
		Debug.Log ("gamestate init");
		GameObject.DontDestroyOnLoad (this.gameObject);
	}

	public Texture2D image;

	public string test = "yo";
	public string chosenImage = "";

	public string[] color = new string[4];
	public float[] hueMin = new float[4];
	public float[] hueMax = new float[4];
	public float[] sat = new float[4];
	public float[] val = new float[4];
}
