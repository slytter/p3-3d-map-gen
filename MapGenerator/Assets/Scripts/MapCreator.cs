using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;



public class MapCreator : MonoBehaviour
{
	TreeInstance tree;
	imageProcModules imageModules;
	ColorDetection scanModules;
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


	void Start ()
	{
		init ();
		biggestDimension = generator.mapSize (mainTerrain, heightOfMap, scanModules);
		HUD = imageModules.flipXAndY (scanModules.originalTexture);

		// Scanning:
		bool[,] yellow	= scanModules.colorDetection ((scanModules.originalImage), 0.09f, 0.2f, 0.3f, 0.5f); 	// getting colors from input image
		bool[,] red = scanModules.colorDetection ((scanModules.originalImage), 0.96f, 0.02f, 0.43f, 0.5f); 		// getting colors from input image
		bool[,] green = scanModules.colorDetection ((scanModules.originalImage), 0.3f, 0.52f, 0.13f, 0.3f); 	// getting colors from input image
		bool[,] blue = scanModules.colorDetection ((scanModules.originalImage), 0.55f, 0.65f, 0.13f, 0.3f); 	// getting colors from input image

		// Generating
		float[,] mountains = generator.generateMountains (red, mountainHeight);
		float[,] baseMap = imageModules.perlin (emptyMap, baseHeight / 2, intensity, density);
		float[,] riversAndBase = generator.generateRiversIntoBase (blue, baseMap); //generate rivers into base perlin map

		// Applying:
		float[,] finalMap = montainModules.finalizeMap (imageModules.add (riversAndBase, mountains), 5);
		mainTerrain.terrainData.SetHeights (0, 0, finalMap);
		lowPolyFy ();
		generator.generateTrees (mainTerrain, tree, green, biggestDimension, baseHeight);

		blobClassify.debug = false;
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
