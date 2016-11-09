using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ColorDetection : MonoBehaviour
{
	public Color[] originalImage;
	public int widthOfTex;
	public int heightOfTex;


    //imageProcModules m;
    void Start () {
		Texture2D originalTexture;
		Texture2D newTexture; 
		Texture2D webcamImage;



		if (GameObject.Find("gameState") == null) {
			print("no image chosen, using test image");
			originalTexture = (Texture2D)GetComponent<Renderer> ().material.mainTexture;
		}else {

			//we = (Texture2D)Resources.LoadAssetAtPath(texture, typeof(Texture2D)
			//webcamImage = Resources.Load("WebcamBilleder/05-11-2016-18-47-36") as Texture2D;
			originalTexture = (GameObject.Find("gameState").GetComponent<gameState>()).image;
			originalTexture.Resize(512, 512);
			originalImage = originalTexture.GetPixels();
		}

        //m = GetComponent<imageProcModules>();



		newTexture = new Texture2D (originalTexture.width, originalTexture.height);
		GetComponent<Renderer> ().material.mainTexture = newTexture;

		originalImage = originalTexture.GetPixels ();
		newTexture.SetPixels (originalImage);
		newTexture.Apply ();

        //Texture2D backupImage = GetComponent<Renderer> ().material.mainTexture;
        // colorDetection (pixN, newIm, 0.23f, 0.15f, 0.25f, 0.5f); //yellow
        // colorDetection (pixN, newIm, 0.14f, 0.06f, 0.25f, 0.5f); //orange

		widthOfTex = originalTexture.width;
		heightOfTex = originalTexture.height;

    }


	/// <summary>
	/// Detect colors in an image
	/// </summary>
	/// <returns>The detection.</returns>
	/// <param name="pixN">Pix n.</param>
	/// <param name="hueMax">Hue max.</param>
	/// <param name="hueMin">Hue minimum.</param>
	/// <param name="sat">Sat.</param>
	/// <param name="val">Value.</param>
	public bool [,] colorDetection (Color[] pixN, float hueMin, float hueMax, float sat, float val) {

		Texture2D newIm = new Texture2D (widthOfTex, heightOfTex); // Only used for width & height.

		bool[,] pixelPosOutput = new bool[widthOfTex, heightOfTex]; // Float 2d array to assign and return

		float h; 
		float s; 
		float v; 

		if (hueMax < hueMin) {
			for (int y = 0; y < newIm.height; y++) {
				for (int x = 0; x < newIm.width; x++) {
					Color.RGBToHSV (pixN [y * newIm.width + x], out h, out s, out v); //Converting rgb to HSV and assigning float variables. 

					if (h > hueMin || h < hueMax && s > sat && v > val) {
						pixelPosOutput [x, y] = true; // Assigning position x,y in float array. 
					} else {
						pixelPosOutput [x, y] = false; 
					}

				}
			}
		} else {
			for (int y = 0; y < newIm.height; y++) {
				for (int x = 0; x < newIm.width; x++) {
					Color.RGBToHSV (pixN [y * newIm.width + x], out h, out s, out v); //Converting rgb to HSV and assigning float variables. 

					if (h > hueMin && h < hueMax && s > sat && v > val) {
						pixelPosOutput [x, y] = true; // Assigning position x,y in float array. 
					} else {
						pixelPosOutput [x, y] = false; 
					}
				}
			}
		}
		printBinary (pixelPosOutput); 
		return (pixelPosOutput); 
	}



	/// <summary>
	/// Prints out image, on the colorScan plane.
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