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
	public GameObject givenObj;


	void Start ()
	{
		init ();
		mainTerrain = mapSize (mainTerrain);

		//GENERATION: 		////////////
		bool[,] yellow	= scanModules.colorDetection ((scanModules.originalImage), 0.1f, 0.19f, 0.5f, 0.3f); // getting colors from input image
		bool[,] red = scanModules.colorDetection ((scanModules.originalImage), 0.97f, 0.1f, 0.20f, 0.55f); // getting colors from input image
		bool[,] green = scanModules.colorDetection ((scanModules.originalImage), 0.23f, 0.52f, 0.17f, 0.2f); // getting colors from input image
		bool[,] blue = scanModules.colorDetection ((scanModules.originalImage), 0.60f, 0.69f, 0.1f, 0.1f); // getting colors from input image

		float[,] mountains = generateMountains (red, mountainHeight);
		float[,] baseMap = imageModules.perlin (emptyMap, baseHeight / 2, intensity, density); // iterations?
		float[,] riversAndBase = generateRiversIntoBase (blue, baseMap); //generate rivers into base perlin map

		float[,] finalMap = montainModules.finalizeMap (imageModules.add (riversAndBase, mountains), 5);
		mainTerrain.terrainData.SetHeights (0, 0, finalMap);

		blobClassify.debug = true;

		Blob[] blobs = blobClassify.grassFire (imageModules.blackFrame (blue));

		for (int i = 0; i < blobs.Length; i++) {
			print (blobs [i].type);
			if (blobs [i].type == "Triangle") {
				print ("spawning player at: " + blobs [i].CenterOfMass.x + ", " + blobs [i].CenterOfMass.y + " with the angle: " + blobs [i].angle);
				Instantiate (givenObj, new Vector3 (blobs [i].CenterOfMass.y, 10f, blobs [i].CenterOfMass.x), Quaternion.identity);
			}
		}

		SpawnPrefab (255, 255, mainTerrain);

		generateTrees (mainTerrain, yellow);

		scanModules.printBinary (blue);

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
		mainTerrain.terrainData.RefreshPrototypes ();	
		float yScale = (float)scanModules.widthOfTex / (float)biggestDimension;
		float xScale = (float)scanModules.heightOfTex / (float)biggestDimension;

		print ("xscal:" + xScale);
		print ("ys" + yScale);

		binaryTreeArea = imageModules.dilation (imageModules.dilation (binaryTreeArea));
		binaryTreeArea = imageModules.floodFillQueue (binaryTreeArea);
		scanModules.printBinary (binaryTreeArea);

		float[,] treeArea = imageModules.boolToFloat (binaryTreeArea);
		float[,] treePositions = imageModules.generateTreePositions (treeArea, 6);


		TreeInstance[] reset = new TreeInstance[0]; 

		mainTerrain.terrainData.treeInstances = reset; 
		mainTerrain.terrainData.RefreshPrototypes ();	

		List <TreeInstance> treeList = new List<TreeInstance> (mainTerrain.terrainData.treeInstances); 
		for (int i = 0; i < treePositions.GetLength (1); i++) {

			//if (treePositions [0, x] != null || treePositions [1, x] != null)
			//{
			if (mainTerrain.terrainData.GetSteepness (treePositions [1, i] * xScale, treePositions [0, i] * yScale) < 45f) { // 1 & 0 has been flipped to mirror trees.
				if (mainTerrain.terrainData.GetHeight ((int)(treePositions [1, i] * xScale), (int)(treePositions [0, i] * yScale)) > (int)(baseHeight)) {
					tree.position = new Vector3 (treePositions [1, i] * xScale, 0f, treePositions [0, i] * yScale); 
					tree.color = Color.yellow; 
					tree.lightmapColor = Color.yellow; 
					tree.prototypeIndex = 0; 
					tree.widthScale = 1; 
					tree.heightScale = 1;
					tree.rotation = UnityEngine.Random.Range (0f, Mathf.PI);
					treeList.Add (tree);
				}
			}
			//}
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
		area = imageModules.dilation (area);
		area = imageModules.dilation (area);
		area = imageModules.erosion (area); 
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
		area = imageModules.dilation (area); //area = where red is.
		area = imageModules.dilation (area); //area = where red is.
		area = imageModules.dilation (area); //area = where red is.
		area = imageModules.erosion (area); 
		area = imageModules.erosion (area); 
		area = imageModules.erosion (area); 

		area = imageModules.floodFillQueue (area);
		scanModules.printBinary (area);
		float[,] mountainArea = new float[area.GetLength (0), area.GetLength (1)];
		mountainArea = imageModules.boolToFloat (area);
		mountainArea = imageModules.gaussian (mountainArea, 10);

		float[,] randomMountains = new float[area.GetLength (0), area.GetLength (1)];
		randomMountains = montainModules.midpointDisplacement (3, randomMountains, 0.5f, 0);
		randomMountains = montainModules.midpointDisplacement (8, randomMountains, 0.5f, 0);
		randomMountains = montainModules.midpointDisplacement (16, randomMountains, 0.4f, 0);
		randomMountains = montainModules.midpointDisplacement (32, randomMountains, 0.3f, 0);
		randomMountains = montainModules.midpointDisplacement (64, randomMountains, 0.3f, 0);
		randomMountains = montainModules.midpointDisplacement (128, randomMountains, 0.2f, 0);
		randomMountains = imageModules.gaussian (randomMountains, 5);

		return (montainModules.mountainRemove (randomMountains, mountainArea, mountainHeight));
	}

	void SpawnPrefab (int x, int y, Terrain heightmap)
	{
		float currentHeight = heightmap.terrainData.GetHeight (x, y);
		GameObject.Instantiate (Key, new Vector3 (x, currentHeight + 2, y), Quaternion.identity);

	}





	//	bool stop = false;
	//
	//	void Update ()
	//	{
	//		if (!stop && false) {
	//			if (frame % 2 == 0) {
	//				for (int j = 0; j < drawMap.GetLength (1); j++) {
	//					for (int i = 0; i < drawMap.GetLength (0); i++) {
	//						drawMap [i, j] = emptyMap [i, j] * update * 0.5f;
	//					}
	//				}
	//				currentTerrain.terrainData.SetHeights (0, 0, drawMap);
	//			}
	//			frame++;
	//			if (update < 1f) {
	//				update += 0.01f;
	//			} else {
	//				print (drawMap.GetLength (0) + ", " + drawMap.GetLength (0));
	//				stop = true;
	//			}
	//		}
	//	}

}
