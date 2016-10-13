using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ColorDetection : MonoBehaviour
{

	Color[] pixO;
	void Start ()
	{

		Texture2D originalIm;
		Texture2D newIm;
		Color[] pixN;
		 

		newIm = new Texture2D (512, 512);
		originalIm = (Texture2D)GetComponent<Renderer> ().material.mainTexture;
		GetComponent<Renderer> ().material.mainTexture = newIm;

		pixN = originalIm.GetPixels ();
		pixO = originalIm.GetPixels ();
		newIm.SetPixels (pixN);
		newIm.Apply ();	

		// looking for green
		//colorDetection (pixN, newIm, 0.42f, 0.24f, 0.20f, 0.5f);

		// Loking for yellow
		//colorDetection (pixN, newIm, 0.23f, 0.15f, 0.25f, 0.5f);

		// overloaded method for red, since intervals is crossing 0
		colorDetection (pixN, newIm, 1f, 0f, 0.94f, 0.02f, 0.25f, 0.5f);

		// Looking for Orange
		//colorDetection (pixN, newIm, 0.14f, 0.06f, 0.25f, 0.5f);
	}

	void Update ()
	{
			
	}

	bool [,] colorDetection (Color [] pixN, Texture2D newIm, float hueMax, float hueMin, float sat, float val)
	{

		//Float 2d array to assign and return
		bool[,] pixelPosGreen = new bool[512, 512];  
		float h; 
		float s; 
		float v; 

		for (int y = 0; y < newIm.height; y++) {
			for (int x = 0; x < newIm.width; x++) {


				//Converting rgb to HSV and assigning float variables. 
				Color.RGBToHSV (pixN [y * newIm.width + x], out h, out s, out v); 

				// Looking for green
				if (h > hueMin && h < hueMax && s > sat && v > val) {
					// Assigning position x,y in float array. 
					pixelPosGreen [x, y] = true;
				} else {
					pixelPosGreen [x, y] = false; 
				}
			}
		} 
		printBinary (pixelPosGreen, pixN, newIm); 
		return pixelPosGreen;
	}
	bool [,] colorDetection (Color [] pixN, Texture2D newIm, float Max,float Min, float hueMax, float hueMin, float sat, float val)
	{

		//Float 2d array to assign and return
		bool[,] pixelPosGreen = new bool[512, 512];  
		float h; 
		float s; 
		float v; 

		for (int y = 0; y < newIm.height; y++) {
			for (int x = 0; x < newIm.width; x++) {


				//Converting rgb to HSV and assigning float variables. 
				Color.RGBToHSV (pixN [y * newIm.width + x], out h, out s, out v); 

				// Looking for green
				if (h > Min && h < hueMin && s > sat && v > val || h > hueMax && h < Max && s > sat && v > val) {
					// Assigning position x,y in float array. 
					pixelPosGreen [x, y] = true;
				} else {
					pixelPosGreen [x, y] = false; 
				}
			}
		} 
		printBinary (pixelPosGreen, pixN, newIm); 
		return pixelPosGreen;
	}


	void printBinary (bool[,] input, Color [] pixN, Texture2D newIm)
	{

		for (int y = 0; y < input.GetLength(0); y++) {
			for (int x = 0; x < input.GetLength(1); x++) {


				if (input [x, y] == true) {
					pixN [y * newIm.width + x] = new Color (0, 1, 0); 
				} else {
					pixN [y * newIm.width + x] = new Color (0, 0, 0);
				}
			}
		}

		newIm.SetPixels (pixN);
		newIm.Apply ();	
	}
}