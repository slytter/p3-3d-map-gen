using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ColorDetection : MonoBehaviour
{


	void Start ()
	{

		Texture2D originalIm;
		Texture2D newIm;
		Color[] pixN;
		float[,] testArray = new float[512, 512]; 
		 

		newIm = new Texture2D (512, 512);
		originalIm = (Texture2D)GetComponent<Renderer> ().material.mainTexture;
		GetComponent<Renderer> ().material.mainTexture = newIm;

		pixN = originalIm.GetPixels ();
		newIm.SetPixels (pixN);
		newIm.Apply ();	

		// looking for green
		Array.Copy(colorDetection (pixN, newIm, 0.42f, 0.24f, 0.20f, 0.5f), testArray, 0);

		// Looking for yellow
		//colorDetection (pixN, newIm, 0.23f, 0.15f, 0.25f, 0.5f);

		// overloaded method for red, since intervals is crossing 0
		//colorDetection (pixN, newIm, 1f, 0f, 0.94f, 0.02f, 0.25f, 0.5f);

		// Looking for Orange
		//colorDetection (pixN, newIm, 0.14f, 0.06f, 0.25f, 0.5f);
	}

	float [,] grassFire(float [,] testArray, int nodeX, int nodeY)
	{
		float [,] objectsDefined = new float[512, 512];



		return objectsDefined;
	}



	float [,] colorDetection (Color[] pixN, Texture2D newIm, float hueMax, float hueMin, float sat, float val)
	{

		//Float 2d array to assign and return
		float[,] pixelPosOutput = new float[512, 512];


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
					pixelPosOutput[x,y] = 1f; 

				} else {
					pixelPosOutput [x, y] = 0f; 

				}
			}
		}
		 
		printBinary (pixelPosOutput, pixN, newIm); 

		return pixelPosOutput; 

	}

	float [,] colorDetection (Color[] pixN, Texture2D newIm, float Max, float Min, float hueMax, float hueMin, float sat, float val)
	{

		//Float 2d array to assign and return
		float[,] pixelPosRed = new float[512, 512];  
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
					pixelPosRed [x, y] = 1f;
				} else {
					pixelPosRed [x, y] = 0f; 
				}
			}
		} 
		//printBinary (pixelPosGreen, pixN, newIm); 
		return pixelPosRed;
	}


	// only for testing
	void printBinary (float[,] input, Color[] pixN, Texture2D newIm)
	{

		for (int y = 0; y < input.GetLength (1); y++) {
			for (int x = 0; x < input.GetLength (0); x++) {


				if (input [x, y] == 1f) {
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