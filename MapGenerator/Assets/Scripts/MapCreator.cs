using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;



public class MapCreator : MonoBehaviour
{
	TreeInstance tree;
	imageProcModules imageModules;
	ColorDetection scanModules;
	ColorDetection display1;
	ColorDetection display2;
	ColorDetection display3;
	MountainGeneration montainModules;
	generator generator;
	float update;
	float[,] drawMap, emptyMap;
	public int heightOfMap = 100;
	public float[,] newHeightMap;
	public float[,] river;
	public float riverButtom = 0.2f;
	public Terrain Grass;
	public GameObject Key;
	public Terrain mainTerrain;
	public float baseHeight = 0.2f, intensity = 20f, density = 20f, mountainHeight = 0.5f;
	int biggestDimension;
	public GameObject spawn, gateObj;
	public Texture2D playerIcon;
	Texture2D HUD;

	[Range (0.0f, 1.0f)]
	public float hueMin = 0.97f;
	[Range (0.0f, 1.0f)]
	public float humMax = 0.07f;
	[Range (0.0f, 1.0f)]
	public float sat = 0.53f;
	[Range (0.0f, 1.0f)]
	public float val = 0.3f;
	public bool whiteB = true;
	public bool RGBnorm = true;
	public bool median = true;
	public bool median2 = true;
	public float thresh = 0.2f;
	public float whiteThresh = 1.8f;


	bool[,] color;

	int frames = 0;

	void Update1 ()
	{
		if (frames % 2 == 0) {
			Color[] image = scanModules.originalImage;
			display1.printTexture (image);


		
			if (whiteB) {
				image = imageModules.whiteBalance (image);
			}

			image = imageModules.cutBlacks (image, thresh);
			image = imageModules.cutWhites (image, whiteThresh);
			display1.printTexture (image);

			if (RGBnorm) {
				image = imageModules.RGBNormalize (image);
			}
			display2.printTexture (image);




			color = scanModules.colorDetection (image, hueMin, humMax, sat, val);

			display3.printBinary (color);
		


			//	scanModules.printBinary (imageModules.medianFilter (color));
			if (median) {
				scanModules.printBinary (imageModules.floodFillQueue (imageModules.medianFilter (imageModules.erosion (imageModules.dilation ((color))))));
			} else {
				scanModules.printBinary (imageModules.floodFillQueue (imageModules.erosion (imageModules.dilation (color))));
			}
				
		}
		frames++;
	}

	void Start1 ()
	{
		init ();
		biggestDimension = generator.mapSize (mainTerrain, heightOfMap, scanModules);
		HUD = imageModules.flipXAndY (scanModules.originalTexture);

		scanModules.printTexture (imageModules.whiteBalance ((scanModules.originalTexture.GetPixels ())));
	}


	void Start ()
	{
		init ();
		biggestDimension = generator.mapSize (mainTerrain, heightOfMap, scanModules);
		HUD = imageModules.flipXAndY (scanModules.originalTexture);

		Color[] scanImage = scanModules.originalImage;
		scanImage = imageModules.whiteBalance (scanImage); 
		scanModules.printTexture (scanImage);
		scanImage = imageModules.cutBlacks (scanImage, thresh); 
		scanImage = imageModules.cutWhites (scanImage, whiteThresh); 
		//	scanImage = imageModules.RGBNormalize (scanImage);




		// Scanning:
		bool[,] red = scanModules.colorDetection ((scanImage), 0.95f, 0.08f, 0.2f, 0.46f); 		// getting colors from input image

		bool[,] yellow	= scanModules.colorDetection (scanImage, 0.1f, 0.3f, 0.2f, 0.3f); 	// getting colors from input image
		yellow = imageModules.floodFillQueue (yellow);

		bool[,] green = scanModules.colorDetection ((scanImage), 0.35f, 0.55f, 0.16f, 0.2f); 	// getting colors from input image

		bool[,] blue = scanModules.colorDetection ((scanImage), 0.56f, 0.8f, 0.06f, 0.2f); 	// getting colors from input image


		// Generating
		float[,] mountains = generator.generateMountains (red, mountainHeight);
		float[,] baseMap = imageModules.perlin (emptyMap, baseHeight / 2, intensity, density);
		float[,] riversAndBase = generator.generateRiversIntoBase (blue, baseMap); //generate rivers into base perlin map

		display2.printBinary (mountains);
		display3.printBinary (riversAndBase);

		// Applying:
		float[,] finalMap = montainModules.finalizeMap (imageModules.add (riversAndBase, mountains), 5);
		mainTerrain.terrainData.SetHeights (0, 0, finalMap);
		lowPolyFy ();
		generator.generateTrees (mainTerrain, tree, green, biggestDimension, baseHeight);

		blobClassify.debug = true;
		Blob[] blobs = blobClassify.grassFire (imageModules.blackFrame (yellow));

		if (!generator.generateObjects (blobs, "Triangle", mainTerrain, spawn, 0, true))
			generator.SpawnPrefab (255, 255, mainTerrain, spawn);
		generator.generateObjects (blobs, "Square", mainTerrain, Key, 0, false);

		if (!generator.generateObjects (blobs, "Circle", mainTerrain, gateObj, 90f, true))
			generator.SpawnPrefab (255, 255, mainTerrain, gateObj, new Vector3 (90f, 90f, 0f));
		
	}


	void init ()
	{
		blobClassify.debug = true;

		//Terrain:
		mainTerrain = GameObject.Find ("Terrain").GetComponent <Terrain> ();

		//Scripts:
		generator = GetComponent<generator> ();
		imageModules = GetComponent<imageProcModules> ();
		scanModules = GameObject.Find ("colorScan").GetComponent<ColorDetection> ();
		display1 = GameObject.Find ("display1").GetComponent<ColorDetection> ();
		display2 = GameObject.Find ("display2").GetComponent<ColorDetection> ();
		display3 = GameObject.Find ("display3").GetComponent<ColorDetection> ();
		montainModules = GetComponent<MountainGeneration> ();

		emptyMap = new float[scanModules.widthOfTex, scanModules.heightOfTex]; // getting terrain data
	}


	void lowPolyFy ()
	{
		LowPolySystems.LowPolyWorld lowPoly = GameObject.Find ("lowpoly").GetComponent<LowPolySystems.LowPolyWorld> ();
		lowPoly.sampleScale = 3.9f;
		lowPoly.forceSize = true;
		lowPoly.forceResolution = false;
		Light sun = GameObject.Find ("Sun").GetComponent<Light> ();
		lowPoly.sun = sun;
		lowPoly.Generate ();
	}





	void OnGUI ()
	{
		GUI.DrawTexture (new Rect (1920 - 260, 10, 250, 250), HUD, ScaleMode.StretchToFill, true, 0f);
		int canvasX = 1920 - 260;
		int canvasY = 10;
		float canvasPlayerPosX = 250 - GameObject.Find ("FPSController(Clone)").transform.position.x / 512 * 250 + canvasX;
		float canvasPlayerPosY = GameObject.Find ("FPSController(Clone)").transform.position.z / 512 * 250 + canvasY;
		GUI.DrawTexture (new Rect ((int)canvasPlayerPosX - 15, (int)canvasPlayerPosY - 15, 30, 30), playerIcon, ScaleMode.StretchToFill, true, 0f);
	}




}
