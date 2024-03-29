﻿using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using System;
using System.IO;


public class calibrateColor : MonoBehaviour
{
	public Color[] originalImage;
	public int widthOfTex;
	public int heightOfTex;
	TextureImporter importer;
	string path;
	Texture2D originalTexture;
	int frameCount;
	gameState gameState;
	public int number;
	//imageProcModules m;

	[Range (0.0f, 1.0f)]
	public float hueMin = 0.9f;
	[Range (0.0f, 1.0f)]
	public float humMax = 0.10f;
	[Range (0.0f, 1.0f)]
	public float sat = 0.5f;
	[Range (0.0f, 1.0f)]
	public float val = 0.3f;

	public string currentColor;

	public bool dilateImage;
	public bool dilateImageAgain;
	public bool erodeImage;
	public bool floodFillImage;
	imageProcModules imageModules;


	void Awake ()
	{
		try {
			gameState = GameObject.Find ("gameState").GetComponent<gameState> ();
			path = "WebcamBilleder/" + gameState.chosenImage;
			print ("Assets/Resources/" + path);
			AssetDatabase.Refresh ();
			importer = (TextureImporter)TextureImporter.GetAtPath ("Assets/Resources/" + path);
			importer.textureType = TextureImporterType.Advanced;
			importer.textureFormat = TextureImporterFormat.ARGB32;
			importer.isReadable = true;
			importer.maxTextureSize = 512;
			importer.npotScale = TextureImporterNPOTScale.ToLarger;
			importer.SaveAndReimport ();
			AssetDatabase.Refresh ();
		} catch {
			print ("Could not import webcam image from Awake()");
		}


	}

	void Start ()
	{ 
		imageModules = GetComponent<imageProcModules> ();
		Texture2D newTexture;

		try {
			gameState = GameObject.Find ("gameState").GetComponent<gameState> ();

			originalTexture = Resources.Load ("WebcamBilleder/" + Path.GetFileNameWithoutExtension (path), typeof(Texture2D)) as Texture2D; 
			print ("looking for image in: " + "WebcamBilleder/" + gameState.chosenImage);

			GetComponent<Renderer> ().material.mainTexture = originalTexture;
			originalImage = originalTexture.GetPixels ();
		} catch {
			print ("gameState object non existing, using test img");
		}

		originalTexture = (Texture2D)GetComponent<Renderer> ().material.mainTexture;

		newTexture = new Texture2D (originalTexture.width, originalTexture.height);
		GetComponent<Renderer> ().material.mainTexture = newTexture;

		originalImage = originalTexture.GetPixels ();
		newTexture.SetPixels (originalImage);
		newTexture.Apply ();



		widthOfTex = originalTexture.width;
		heightOfTex = originalTexture.height;





	}

	void Update ()
	{
		frameCount++;
		if (frameCount % 2 == 0) {
			bool[,] color = colorDetection (originalImage, hueMin, humMax, sat, val);
			if (dilateImage)
				color = imageModules.dilation (color);
			if (dilateImageAgain)
				color = imageModules.dilation (color);
			if (erodeImage)
				color = imageModules.dilation (color);
			if (floodFillImage)
				color = imageModules.floodFillQueue (color);

			printBinary (color);
		}
	}


	public void exportIntoGameState ()
	{
		try {
			gameState gameState = GameObject.Find ("gameState").GetComponent<gameState> ();
			gameState.hueMin [number] = hueMin;
			gameState.hueMax [number] = humMax;
			gameState.sat [number] = sat;
			gameState.val [number] = val;
			gameState.color [number] = currentColor;

		} catch {
			Debug.LogError ("No gamestate is present. Make sure this scene was loaded from camerascript");
		}

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
	public bool [,] colorDetection (Color[] pixN, float hueMin, float hueMax, float sat, float val)
	{

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
		return (pixelPosOutput); 
	}



	/// <summary>
	/// Prints out image, on the colorScan plane.
	/// </summary>
	/// <param name="input">Input.</param>
	/// <param name="pixN">Pix n.</param>
	/// <param name="newIm">New im.</param>
	public void printBinary (bool[,] input)
	{
		Color[] pixN = new Color[input.GetLength (0) * input.GetLength (1)];
		for (int y = 0; y < input.GetLength (1); y++) {
			for (int x = 0; x < input.GetLength (0); x++) {

				if (input [x, y]) {
					pixN [y * input.GetLength (0) + x] = new Color (1, 1, 1);
				} else {
					pixN [y * input.GetLength (0) + x] = new Color (0, 0, 0);
				}
			}
		}
		Texture2D image = (Texture2D)GetComponent<Renderer> ().material.mainTexture;
		image.SetPixels (pixN);
		image.Apply ();	
	}

	public void printBinary (float[,] input)
	{
		Color[] pixN = new Color[input.GetLength (0) * input.GetLength (1)];
		for (int y = 0; y < input.GetLength (1); y++) {
			for (int x = 0; x < input.GetLength (0); x++) {
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