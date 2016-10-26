using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ColorDetection : MonoBehaviour
{
	public Color[] originalImage;
	//imageProcModules m;
	void Start () {
		//m = GetComponent<imageProcModules>();
		Texture2D originalTexture;
		Texture2D newTexture; 

		originalTexture = (Texture2D)GetComponent<Renderer> ().material.mainTexture;
		newTexture = new Texture2D (originalTexture.width, originalTexture.height);
		GetComponent<Renderer> ().material.mainTexture = newTexture;

		originalImage = originalTexture.GetPixels ();
		newTexture.SetPixels (originalImage);
		newTexture.Apply ();

		//Texture2D backupImage = GetComponent<Renderer> ().material.mainTexture;


		// Looking for yellow
		// colorDetection (pixN, newIm, 0.23f, 0.15f, 0.25f, 0.5f);

		// overloaded method for red, since intervals is crossing 0
		// colorDetection (pixN, newIm, 1f, 0f, 0.94f, 0.02f, 0.25f, 0.5f);
		// NOT WORKING SINCE OVERLOAD METHOD WAS REMOVED.

		// Looking for Orange
		// colorDetection (pixN, newIm, 0.14f, 0.06f, 0.25f, 0.5f);
	}




	public bool [,] colorDetection (Color[] pixN, float hueMax, float hueMin, float sat, float val) {

		Texture2D newIm = new Texture2D (512, 512); // Only used for width & height.

		bool[,] pixelPosOutput = new bool[512, 512]; // Float 2d array to assign and return

		float h; 
		float s; 
		float v; 
		Debug.Log(pixN.GetLength(0));

		for (int y = 0; y < newIm.height; y++) {
			for (int x = 0; x < newIm.width; x++) {
				Color.RGBToHSV (pixN [y * newIm.width + x], out h, out s, out v); //Converting rgb to HSV and assigning float variables. 

				if (h > hueMin && h < hueMax && s > sat && v > val) {
					pixelPosOutput[x,y] = true; // Assigning position x,y in float array. 
				} else {
					pixelPosOutput [x, y] = false; 
				}
			}
		}
		printBinary (pixelPosOutput); 
		return (pixelPosOutput); 
	}



	/// <summary>
	/// Prints the binary.
	/// </summary>
	/// <param name="input">Input.</param>
	/// <param name="pixN">Pix n.</param>
	/// <param name="newIm">New im.</param>
	public void printBinary (bool[,] input) {
		Color[] pixN = new Color[input.GetLength (0) * input.GetLength (1)];
		for (int y = 0; y < input.GetLength(1); y++) {
			for (int x = 0; x < input.GetLength(0); x++) {

				if (input [x, y]) {
					pixN [y * input.GetLength(0) + x] = new Color (1, 1, 1);
				} else {
					pixN [y * input.GetLength(0) + x] = new Color (0, 0, 0);
				}
			}
		}
		Texture2D image = (Texture2D)GetComponent<Renderer> ().material.mainTexture;
		image.SetPixels (pixN);
		image.Apply ();	
	}

	public void printBinary (float[,] input) {
		Color[] pixN = new Color[input.GetLength (0) * input.GetLength (1)];
		for (int y = 0; y < input.GetLength(1); y++) {
			for (int x = 0; x < input.GetLength(0); x++) {
				pixN [y * input.GetLength (0) + x].r = input [x, y];
				pixN [y * input.GetLength (0) + x].g = input [x, y];
				pixN [y * input.GetLength (0) + x].b = input [x, y];
			}
		}
		Texture2D image = (Texture2D)GetComponent<Renderer> ().material.mainTexture;
		image.SetPixels (pixN);
		image.Apply ();	
	}
}