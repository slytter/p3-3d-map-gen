using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;



public class MapCreator : MonoBehaviour
{
	TreeInstance tree;
	imageProcModules modules;
	ColorDetection colorScanScript;
	MountainGeneration mg;
	Terrain currentTerrain;
	float update;
	int frame = 0;
	float[,] drawMap, emptyMap;
	public int heightOfMap = 100;
	public float[,] newHeightMap;
	public float[,] river;
	public float riverButtom = 0.2f;

	public float baseHeight, intensity, density, mountainHeight;


	void Start ()
	{

		TimingModule.timer ("program", "start");


		//INITIALIZING: 	////////////
		modules = GetComponent<imageProcModules> ();
		colorScanScript = GameObject.Find ("colorScan").GetComponent<ColorDetection> ();
		mg = GetComponent<MountainGeneration> ();

		Debug.Log (colorScanScript.widthOfTex + ", " + colorScanScript.heightOfTex);

		//fixing texture scale issue:
		emptyMap = new float[colorScanScript.widthOfTex, colorScanScript.heightOfTex];
		//drawMap = new float[colorScanScript.widthOfTex, colorScanScript.heightOfTex];


		currentTerrain = Terrain.activeTerrain; // getting terrain data

		float res = 0.5f;

		int biggestDimension = (colorScanScript.heightOfTex > colorScanScript.widthOfTex) ? colorScanScript.heightOfTex : colorScanScript.widthOfTex; //Simple if statement ¨
		currentTerrain.terrainData.size = new Vector3 (biggestDimension, heightOfMap, biggestDimension); //setting size
		currentTerrain.terrainData.RefreshPrototypes();	

		print(currentTerrain.terrainData.size.x + " :yo " + currentTerrain.terrainData.size.z);

		currentTerrain.terrainData.heightmapResolution = (biggestDimension + 1); 
		currentTerrain.terrainData.SetHeights(0,0, new float[biggestDimension, biggestDimension]);







		//GENERATION: 		////////////
		bool[,] yellow = colorScanScript.colorDetection (colorScanScript.originalImage, 0.15f, 0.23f, 0.20f, 0.5f); // getting colors from input image
		bool[,] red = colorScanScript.colorDetection (colorScanScript.originalImage, 0.94f, 0.05f, 0.25f, 0.5f); // getting colors from input image
		bool[,] green = colorScanScript.colorDetection (colorScanScript.originalImage, 0.24f, 0.42f, 0.20f, 0.5f); // getting colors from input image
		bool[,] blue = colorScanScript.colorDetection (colorScanScript.originalImage, 0.40f, 0.69f, 0.1f, 0.5f); // getting colors from input image


		float[,] perlin = modules.perlin (emptyMap, baseHeight / 2, intensity, density); // iterations?
		perlin = modules.perlin (perlin, baseHeight / 2, intensity * 6, density / 4);//two perlin noises to create more 'real' density

//		float[,] mountains = generateMountains (red, mountainHeight);
//		float[,] rivers = generateRivers (blue, perlin); //generate rivers into base perlin map
//		float[,] finalMap = modules.flip (mg.finalizeMap (modules.add (rivers, mountains), 5));
//		generateTrees (green);

		modules.grassFire (modules.floodFillQueue(modules.dilation(modules.dilation (yellow))));

		//currentTerrain.terrainData.SetHeights (0, 0, finalMap);


		TimingModule.timer ("program", "end");

	}

	void generateTrees (bool[,] binaryTreeArea)
	{
		
		binaryTreeArea = modules.dilation (modules.dilation (binaryTreeArea));
		binaryTreeArea = modules.floodFillQueue (binaryTreeArea);
		float[,] treeArea = modules.boolToFloat (binaryTreeArea);

		float[,] treePositions = modules.generateTrees (treeArea, 6);


		TreeInstance[] reset = new TreeInstance[0]; 

		currentTerrain.terrainData.treeInstances = reset; 

		List <TreeInstance> treeList = new List<TreeInstance> (currentTerrain.terrainData.treeInstances); 
		for (int x = 0; x < treePositions.GetLength (1); x++)
		{

			//if (treePositions [0, x] != null || treePositions [1, x] != null)
			//{
				if (currentTerrain.terrainData.GetSteepness (treePositions [0, x], treePositions [1, x]) < 45f)
				{
					tree.position = new Vector3 (treePositions [0, x], 0f, treePositions [1, x]); 
					tree.color = Color.yellow; 
					tree.lightmapColor = Color.yellow; 
					tree.prototypeIndex = 0; 
					tree.widthScale = 1; 
					tree.heightScale = 1; 
					treeList.Add (tree);
				}
			//}
		}
		currentTerrain.terrainData.treeInstances = treeList.ToArray (); 
	}

	
	/// <summary>
	/// Generates the rivers.
	/// </summary>
	/// <returns>The rivers.</returns>
	/// <param name="area">Area.</param>
	/// <param name="basemap">Basemap.</param>
	float[,] generateRivers (bool[,] area, float[,] basemap){
		area = modules.floodFillQueue (area);
		float[,] river = modules.boolToFloat (area);
		river = modules.gaussian (river, 10);
		river = modules.compressAndSubtract (basemap, river);
		return river;
	}



	/// <summary>
	/// Generates the mountains.
	/// </summary>
	/// <returns>The mountains.</returns>
	/// <param name="area">Area.</param>
	float[,] generateMountains (bool[,] area, float mountainHeight)
	{
		area = modules.dilation (area); //area = where red is.
		area = modules.floodFillQueue (area);

		colorScanScript.printBinary (area);

		float[,] mountainArea = new float[area.GetLength (0), area.GetLength (1)];
		mountainArea = modules.boolToFloat (area);
		mountainArea = modules.gaussian (mountainArea, 10);

		float[,] randomMountains = new float[area.GetLength (0), area.GetLength (1)];
		randomMountains = mg.midpointDisplacement (3, randomMountains, 0.5f, 0);
		randomMountains = mg.midpointDisplacement (8, randomMountains, 0.5f, 0);
		randomMountains = mg.midpointDisplacement (16, randomMountains, 0.4f, 0);
		randomMountains = mg.midpointDisplacement (32, randomMountains, 0.3f, 0);
		randomMountains = mg.midpointDisplacement (64, randomMountains, 0.3f, 0);
		randomMountains = mg.midpointDisplacement (128, randomMountains, 0.2f, 0);
		randomMountains = modules.gaussian (randomMountains, 5);

		return (mg.mountainRemove (randomMountains, mountainArea, mountainHeight));
	}





	bool stop = false;

	void Update ()
	{
		if (!stop && false)
		{
			if (frame % 2 == 0)
			{
				for (int j = 0; j < drawMap.GetLength (1); j++)
				{
					for (int i = 0; i < drawMap.GetLength (0); i++)
					{
						drawMap [i, j] = emptyMap [i, j] * update * 0.5f;
					}
				}
				currentTerrain.terrainData.SetHeights (0, 0, drawMap);
			}
			frame++;
			if (update < 1f)
			{
				update += 0.01f;
			} else
			{
				print (drawMap.GetLength (0) + ", " + drawMap.GetLength (0));
				stop = true;
			}
		}
	}

}
