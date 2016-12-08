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
		mainTerrain = mapSize (mainTerrain);

		//GENERATION: 		////////////

		bool[,] yellow	= scanModules.colorDetection ((scanModules.originalImage), 0.09f, 0.2f, 0.3f, 0.5f); // getting colors from input image
		bool[,] red = scanModules.colorDetection ((scanModules.originalImage), 0.96f, 0.02f, 0.43f, 0.5f); // getting colors from input image
		bool[,] green = scanModules.colorDetection ((scanModules.originalImage), 0.3f, 0.52f, 0.11f, 0.5f); // getting colors from input image
		bool[,] blue = scanModules.colorDetection ((scanModules.originalImage), 0.55f, 0.65f, 0.17f, 0.3f); // getting colors from input image
		bool[,] black = imageModules.invert (scanModules.colorDetection ((scanModules.originalImage), 0f, 1f, 0.0f, 0.31f));

		float[,] mountains = generateMountains (red, mountainHeight);
		float[,] baseMap = imageModules.perlin (emptyMap, baseHeight / 2, intensity, density); // iterations?
		float[,] riversAndBase = generateRiversIntoBase (blue, baseMap); //generate rivers into base perlin map

		float[,] finalMap = montainModules.finalizeMap (imageModules.add (riversAndBase, mountains), 5);
		mainTerrain.terrainData.SetHeights (0, 0, finalMap);

		LowPolySystems.LowPolyWorld lowPoly = GameObject.Find ("lowpoly").GetComponent<LowPolySystems.LowPolyWorld> ();
		lowPoly.sampleScale = 3.9f;
		lowPoly.forceSize = true;
		lowPoly.forceResolution = false;
		Light sun = GameObject.Find ("Sun").GetComponent<Light> ();
		lowPoly.sun = sun;
		lowPoly.Generate ();

		blobClassify.debug = true;

		Blob[] blobs = blobClassify.grassFire (imageModules.blackFrame (yellow));
		bool playerSpawned = false;

		for (int i = 0; i < blobs.Length; i++) {
			print (blobs [i].type);
			if (blobs [i].type == "Triangle" && !playerSpawned) {
				print ("spawning player at: " + blobs [i].CenterOfMass.y + ", " + blobs [i].CenterOfMass.x + " with the angle: " + blobs [i].angle);
				SpawnPrefab ((int)blobs [i].CenterOfMass.y, (int)blobs [i].CenterOfMass.x, mainTerrain, spawn);
				playerSpawned = true;
			}
			if (blobs [i].type == "Square") {
				print ("spawning key at: " + blobs [i].CenterOfMass.y + ", " + blobs [i].CenterOfMass.x + " with the angle: " + blobs [i].angle);
				SpawnPrefab ((int)blobs [i].CenterOfMass.y, (int)blobs [i].CenterOfMass.x, mainTerrain, Key);
			}
			if (blobs [i].type == "Circle") {
				print ("spawning Gate at: " + blobs [i].CenterOfMass.y + ", " + blobs [i].CenterOfMass.x + " with the angle: " + blobs [i].angle);
				SpawnPrefab ((int)blobs [i].CenterOfMass.y, (int)blobs [i].CenterOfMass.x, mainTerrain, gateObj, new Vector3 (90f, 0f, 0f));
			}
		}

		if (!playerSpawned)
		{
			SpawnPrefab (256, 256, mainTerrain, spawn);
			playerSpawned = true;
		}



		generateTrees (mainTerrain, green);

		//scanModules.printBinary (red);

		HUD = flipXAndY (scanModules.originalTexture);

		TimingModule.timer ("program", "end");



	}

	void Update ()
	{
		//bool[,] yellow = scanModules.colorDetection ( (scanModules.originalImage), hueMin, humMax, sat, val); // getting colors from input image
		//scanModules.printBinary (yellow);
	}


	void init ()
	{
		mainTerrain = GameObject.Find ("Terrain").GetComponent <Terrain> ();
		TimingModule.timer ("program", "start");
		imageModules = GetComponent<imageProcModules> ();
		scanModules = GameObject.Find ("colorScan").GetComponent<ColorDetection> ();
		montainModules = GetComponent<MountainGeneration> ();
		Debug.Log (scanModules.widthOfTex + ", " + scanModules.heightOfTex);
		emptyMap = new float[scanModules.widthOfTex, scanModules.heightOfTex]; // getting terrain data
	}

	Terrain mapSize (Terrain inputTerrain)
	{
		inputTerrain.terrainData.size = new Vector3 (512, heightOfMap, 512); /*setting size*/
		print (inputTerrain.terrainData.size.x + " :yo " + inputTerrain.terrainData.size.z);
		inputTerrain.terrainData.RefreshPrototypes ();	

		biggestDimension = (scanModules.heightOfTex > scanModules.widthOfTex) ? scanModules.heightOfTex : scanModules.widthOfTex; //Simple if statement 
		inputTerrain.terrainData.heightmapResolution = (biggestDimension + 1); 
		inputTerrain.terrainData.SetHeights (0, 0, new float[biggestDimension, biggestDimension]);
		return inputTerrain;
	}


	[Range (0.0f, 1.0f)]
	public float hueMin = 0.9f;
	[Range (0.0f, 1.0f)]
	public float humMax = 0.10f;
	[Range (0.0f, 1.0f)]
	public float sat = 0.5f;
	[Range (0.0f, 1.0f)]
	public float val = 0.3f;



	void generateTrees (Terrain mainTerrain, bool[,] binaryTreeArea)
	{
		float yScale = (float)scanModules.widthOfTex / (float)biggestDimension;
		float xScale = (float)scanModules.heightOfTex / (float)biggestDimension;

		print ("xscal: " + xScale);
		print ("yscal: " + yScale);

		//binaryTreeArea = imageModules.medianFilter (binaryTreeArea);
		binaryTreeArea = imageModules.dilation (imageModules.dilation (binaryTreeArea));
		binaryTreeArea = imageModules.floodFillQueue (binaryTreeArea);
		scanModules.printBinary (binaryTreeArea);

		float[,] treeArea = imageModules.boolToFloat (binaryTreeArea);
		float[,] treePositions = imageModules.generateTreePositions (treeArea, 6);


		TreeInstance[] reset = new TreeInstance[0]; 

		mainTerrain.terrainData.treeInstances = reset; 

		List <TreeInstance> treeList = new List<TreeInstance> (mainTerrain.terrainData.treeInstances); 
		for (int i = 0; i < treePositions.GetLength (1); i++) {


			if (mainTerrain.terrainData.GetSteepness (treePositions [1, i] * xScale, treePositions [0, i] * yScale) < 45f) { // 1 & 0 has been flipped to mirror trees.
				if (mainTerrain.terrainData.GetHeight ((int)(treePositions [1, i] * xScale), (int)(treePositions [0, i] * yScale)) > (int)(baseHeight)) {
					tree.position = new Vector3 (treePositions [1, i] * xScale, (mainTerrain.terrainData.GetHeight ((int)(treePositions [1, i] * xScale * 512), (int)(treePositions [0, i] * yScale * 512)) / 100), treePositions [0, i] * yScale); 
					tree.color = Color.yellow; 
					tree.lightmapColor = Color.yellow; 
					tree.prototypeIndex = 0; 
					float treeScale = UnityEngine.Random.Range (1.3f, 3.8f);
					tree.widthScale = treeScale; 
					tree.heightScale = treeScale;
					tree.rotation = UnityEngine.Random.Range (0f, Mathf.PI);
					treeList.Add (tree);
				}
			}
		}
		mainTerrain.terrainData.treeInstances = treeList.ToArray ();
	}

	
	/// <summary>
	/// Generates the rivers into base.
	/// </summary>
	/// <returns>The rivers into base.</returns>
	/// <param name="area">Area.</param>
	/// <param name="basemap">Basemap.</param>
	float[,] generateRiversIntoBase (bool[,] area, float[,] basemap)
	{
		//area = imageModules.medianFilter (area);
		area = imageModules.dilation (area);
		area = imageModules.dilation (area);
		area = imageModules.erosion (area); 
		area = imageModules.dilation (area);

		//scanModules.printBinary (area);

		area = imageModules.floodFillQueue (area);
		float[,] river = imageModules.boolToFloat (area);
		river = imageModules.gaussian (river, 10);
		river = imageModules.compressAndSubtract (basemap, river);

		return river;
	}



	/// <summary>
	/// Generates the mountains.
	/// </summary>
	/// <returns>The mountains.</returns>
	/// <param name="area">Area.</param>
	float[,] generateMountains (bool[,] area, float mountainHeight)
	{
		area = imageModules.medianFilter (area);
		area = imageModules.dilation (area); //area = where red is.
		area = imageModules.erosion (area); 
		area = imageModules.dilation (area); //area = where red is.
		area = imageModules.erosion (area);
//		area = imageModules.dilation (area); //area = where red is.
//		area = imageModules.erosion (area); 

		area = imageModules.floodFillQueue (area);
		//scanModules.printBinary (area);
		float[,] mountainArea = new float[area.GetLength (0), area.GetLength (1)];
		mountainArea = imageModules.boolToFloat (area);
		mountainArea = imageModules.gaussian (mountainArea, 10);

		float[,] randomMountains = new float[area.GetLength (0), area.GetLength (1)];
		randomMountains = montainModules.midpointDisplacement (3, randomMountains, 0.5f, 0);
		randomMountains = montainModules.midpointDisplacement (8, randomMountains, 0.5f, 0);
		randomMountains = montainModules.midpointDisplacement (16, randomMountains, 0.5f, 0);
		randomMountains = montainModules.midpointDisplacement (32, randomMountains, 0.3f, 0);
		randomMountains = montainModules.midpointDisplacement (64, randomMountains, 0.3f, 0);
//		randomMountains = montainModules.midpointDisplacement (128, randomMountains, 0.2f, 0);
		randomMountains = imageModules.gaussian (randomMountains, 0);

		return (montainModules.mountainRemove (randomMountains, mountainArea, mountainHeight));
	}

	void SpawnPrefab (int x, int y, Terrain heightmap, GameObject obj)
	{
		float currentHeight = heightmap.terrainData.GetHeight (x, y);
		GameObject.Instantiate (obj, new Vector3 (x, currentHeight + 2, y), Quaternion.identity);

	}

	void SpawnPrefab (int x, int y, Terrain heightmap, GameObject obj, Vector3 rot)
	{
		float currentHeight = heightmap.terrainData.GetHeight (x, y);
		GameObject.Instantiate (obj, new Vector3 (x, currentHeight, y), Quaternion.Euler (rot));

	}


	void OnGUI ()
	{

		GUI.DrawTexture (new Rect (1920 - 260, 10, 250, 250), HUD, ScaleMode.StretchToFill, true, 0f);
		int canvasX = 1920 - 260;
		int canvasY = 10;

		float canvasPlayerPosX = 250 - GameObject.Find ("FPSController(Clone)").transform.position.x / 512 * 250 + canvasX;
		float canvasPlayerPosY = GameObject.Find ("FPSController(Clone)").transform.position.z / 512 * 250 + canvasY;


		print (canvasPlayerPosX);
		GUI.DrawTexture (new Rect ((int)canvasPlayerPosX - 15, (int)canvasPlayerPosY - 15, 30, 30), playerIcon, ScaleMode.StretchToFill, true, 0f);
	}


	public Texture2D flipXAndY (Texture2D original)
	{
		TimingModule.timer ("flipX&Y", "start");
		Texture2D flipped = new Texture2D (original.height, original.height);

		int cropAreaOnEachSide = (original.width - original.height) / 2;

		int xN = original.width - 1 - cropAreaOnEachSide;
		int yN = original.height - 1;


		for (int i = cropAreaOnEachSide; i < xN; i++) {
			for (int j = 0; j < yN; j++) {
				flipped.SetPixel (yN - j, xN - i, original.GetPixel (i, j));

			}
		}
		flipped.Apply ();
		TimingModule.timer ("flipX&Y", "end");

		return flipped;
	}

}
