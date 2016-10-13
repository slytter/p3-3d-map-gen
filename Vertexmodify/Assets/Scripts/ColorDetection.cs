using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ColorDetection : MonoBehaviour
{
	Texture2D originalIm;
	Texture2D newIm;
	Color[] pixN;
	Color[] pixO;

	void Start ()
	{
		newIm = new Texture2D (512, 512);
		originalIm = (Texture2D)GetComponent<Renderer> ().material.mainTexture;
		GetComponent<Renderer> ().material.mainTexture = newIm;

		pixN = originalIm.GetPixels ();
		pixO = originalIm.GetPixels ();
		newIm.SetPixels (pixN);
		newIm.Apply ();	

		//TxtBox = GameObject.Find ("Txt");
	}

	void Update ()
	{

		if (Input.GetKeyDown (KeyCode.F1))
			colorDetection ();
	}

	void updateOldImAndApplyNewIm ()
	{

		newIm.SetPixels (pixN);
		newIm.Apply ();	
		pixO = newIm.GetPixels ();
	}

	bool [,] colorDetection ()
	{
		
		//Float 2d array to assign and return
		bool [,] pixelPos = new bool[512, 512];  
		float h; 
		float s; 
		float v; 

		for (int y = 1; y < newIm.height - 1; y++) {
			for (int x = 1; x < newIm.width - 1; x++) {


				//Converting rgb to HSV and assigning float variables. 
				Color.RGBToHSV (pixO [y * newIm.width + x], out h, out s, out v); 

				// Looking for green
				if (h > 0.194f  && h < 0.42f && s > 0.3f && v > 0.6f) {
					// Assigning position x,y in float array. 
					pixelPos [x, y] = true;
				} else {
					pixelPos [x, y] = false; 
				}
			}
		} 
		return pixelPos;
	}
}