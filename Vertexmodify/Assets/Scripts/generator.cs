using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Specific modules.
/// </summary>
public class generator : MonoBehaviour
{
	imageProcModules imageModules;
	ColorDetection scanModules;
	MountainGeneration montainModules;

	void Start ()
	{
		imageModules = GetComponent<imageProcModules> ();
		scanModules = GameObject.Find ("colorScan").GetComponent<ColorDetection> ();
		montainModules = GetComponent<MountainGeneration> ();
	}

	public void generateTrees (Terrain mainTerrain, TreeInstance tree, bool[,] binaryTreeArea, int biggestDimension, float baseHeight)
	{
		float yScale = (float)scanModules.widthOfTex / (float)biggestDimension;
		float xScale = (float)scanModules.heightOfTex / (float)biggestDimension;

		print ("xscal: " + xScale);
		print ("yscal: " + yScale);

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



	public float[,] generateMountains (bool[,] area, float mountainHeight)
	{
		area = imageModules.medianFilter (area);
		area = imageModules.dilation (area); //area = where red is.
		area = imageModules.erosion (area); 
		area = imageModules.dilation (area); //area = where red is.
		area = imageModules.erosion (area);
		//		area = imageModules.dilation (area); //area = where red is.
		//		area = imageModules.erosion (area); 

		area = imageModules.floodFillQueue (area);
		scanModules.printBinary (area);
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


	public float[,] generateRiversIntoBase (bool[,] area, float[,] basemap)
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


	public void SpawnPrefab (int x, int y, Terrain heightmap, GameObject obj)
	{
		float currentHeight = heightmap.terrainData.GetHeight (x, y);
		GameObject.Instantiate (obj, new Vector3 (x, currentHeight + 2, y), Quaternion.identity);

	}

	public void SpawnPrefab (int x, int y, Terrain heightmap, GameObject obj, Vector3 rot)
	{
		float currentHeight = heightmap.terrainData.GetHeight (x, y);
		GameObject.Instantiate (obj, new Vector3 (x, currentHeight, y), Quaternion.Euler (rot));

	}

	public int mapSize (Terrain inputTerrain, float heightOfMap)
	{
		inputTerrain.terrainData.size = new Vector3 (512, heightOfMap, 512); /*setting size*/
		print (inputTerrain.terrainData.size.x + " :yo " + inputTerrain.terrainData.size.z);
		inputTerrain.terrainData.RefreshPrototypes ();	

		int biggestDimension = (scanModules.heightOfTex > scanModules.widthOfTex) ? scanModules.heightOfTex : scanModules.widthOfTex; //Simple if statement 
		inputTerrain.terrainData.heightmapResolution = (biggestDimension + 1); 
		inputTerrain.terrainData.SetHeights (0, 0, new float[biggestDimension, biggestDimension]);
		return biggestDimension;
	}

	public void generateObjects (Blob[] blobs, string figure, Terrain mainTerrain, GameObject Object, float angle, bool once)
	{
		bool objectSpawned = false;

		for (int i = 0; i < blobs.Length; i++) {
			print (blobs [i].type);
			if (blobs [i].type == figure && !objectSpawned) {
				print ("spawning player at: " + blobs [i].CenterOfMass.y + ", " + blobs [i].CenterOfMass.x + " with the angle: " + blobs [i].angle);
				SpawnPrefab ((int)blobs [i].CenterOfMass.y, (int)blobs [i].CenterOfMass.x, mainTerrain, Object, new Vector3 (angle, 0f, 0f));
				if (once)
					objectSpawned = true;
			}
		}
	}


}
