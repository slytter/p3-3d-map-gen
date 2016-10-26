using UnityEngine;
using System.Collections;
using System;

public class MapgenHeight : MonoBehaviour {
	imageProcModules modules; 
	public int length;
	public int height;
	Terrain currentTerrain;
	float update = 0;
	int frame = 0;
	float[,] drawMap = new float[512, 512];
	float[,] myHeightMap = new float[512, 512];
	float lastmillis;
	float max_val = 0;
	ColorDetection colorScanScript;
	void Start () {

		modules = GetComponent<imageProcModules>();
		colorScanScript = GameObject.Find ("colorScan").GetComponent<ColorDetection>();

		bool [,] inputColorImage = colorScanScript.colorDetection (colorScanScript.originalImage, 0.23f, 0.15f, 0.25f, 0.5f);

		float lastmillis = Time.realtimeSinceStartup;
		float startit = Time.realtimeSinceStartup;

		float[,] moutainArea = 
			modules.gaussian (
				modules.boolToFloat (
					modules.floodFill (
						modules.dilation (
							inputColorImage
						)
					)
				),10
			);
		
		myHeightMap = 
			modules.gaussian(
				modules.boolToFloat(
					modules.floodFill(
						modules.dilation(
							inputColorImage
						)
					)
				)
			,2);
		colorScanScript.printBinary (myHeightMap);

//		myHeightMap = mountainRecursion (1, myHeightMap, 0.9f, 15);
//		myHeightMap = mountainRecursion (2, myHeightMap, 0.2f, 20);
		myHeightMap = midpointDisplacement (3, myHeightMap, 0.8f, 10);
		myHeightMap = midpointDisplacement (8, myHeightMap, 0.7f, 10);
		myHeightMap = midpointDisplacement (16, myHeightMap, 0.5f, 0);
		myHeightMap = midpointDisplacement (32, myHeightMap, 0.5f, 0);
		myHeightMap = midpointDisplacement (64, myHeightMap, 0.5f, 0);
		myHeightMap = midpointDisplacement (128, myHeightMap, 0.5f, 0);

		myHeightMap = finalMap (mountainRemove(myHeightMap,moutainArea),1);
		//myHeightMap = inputColorImage;
		colorScanScript.printBinary (myHeightMap);
		Debug.Log("Total millis for all recursions: " + ((Time.realtimeSinceStartup - startit ) * 1000));

		currentTerrain = Terrain.activeTerrain;
//		currentTerrain.terrainData.SetHeights (0, 0, myHeightMap);

	}


	/// <summary>
	/// midpointDisplacement, taking the current recursion, heightmap..
	/// </summary>
	/// <returns>The recursion.</returns>
	/// <param name="recursion">Recursion.</param>
	/// <param name="heightMap">heightMap.</param>
	/// <param name="amount">Amount.</param>
	float[,] midpointDisplacement(int recursion, float[,] heightMap, float amount, int gaussianAmount) {
		// float amount goes from 0 to 1.
		// Debug.Log("Millis for iteration " + recursion + ": "  + ((Time.realtimeSinceStartup-lastmillis)*1000));
		// lastmillis = Time.realtimeSinceStartup; // Calculate the number of milliseconds since midnight

		float[,] randomValues = modules.randomValGen (512, 512);
		int splitLength;
		float currentTerrainHeight;

		splitLength = heightMap.GetLength (1) / recursion;

		int randomArrayPointerX = 0;
		int randomArrayPointerY = 0;

		for (int i = 0; i < heightMap.GetLength(1)-1; i ++) {
			if (i % splitLength == 0) 	
				randomArrayPointerY = i / splitLength; //same here

			for (int j = 0; j < heightMap.GetLength (0)-1; j++) {
				if (j % splitLength == 0) {
					if (j == 0) randomArrayPointerX = 0;
					randomArrayPointerX = j / splitLength; 
				}
				currentTerrainHeight = randomValues [randomArrayPointerX, randomArrayPointerY];

				if (amount == 1) {
					heightMap [i, j] = Math.Abs(currentTerrainHeight);
				} else{
					heightMap [i, j] += Math.Abs(amount/2 - currentTerrainHeight * amount);
				}

				if (heightMap [i, j] > max_val) {
					max_val = heightMap [i, j];
				}
			}
		}
		heightMap = modules.gaussian (heightMap, gaussianAmount);
		return heightMap;
	}


	/// <summary>
	/// Removes moutains where they should not be
	/// 
	/// </summary>
	/// <returns>The remove.</returns>
	/// <param name="heightMap">Height map.</param>
	/// <param name="whereMoutainsShouldBe">Where moutains should be.</param>
	float[,] mountainRemove(float[,] heightMap, float[,] moutainArea) {
		for (int x = 0; x < heightMap.GetLength (0); x++) {
			for (int y = 0; y < heightMap.GetLength (1); y++) {
				heightMap[x,y] = heightMap [x, y] * moutainArea [x, y];
			}
		}

		return heightMap;
	}


	/// <summary>
	/// Finals the map.
	/// </summary>
	/// <returns>The map.</returns>
	/// <param name="heightMap">Height map.</param>
	float[,] finalMap(float[,] heightMap, int smoothing) {
		heightMap = modules.gaussian (heightMap, smoothing);
		for (int i = 0; i < heightMap.GetLength (1); i++) {
			for (int j = 0; j < heightMap.GetLength (0); j++) {
				heightMap [i, j] = heightMap [i, j] / max_val;
			}
		}
		return heightMap;
	}


	void Update(){
		if(frame % 1 == 0){	
			for (int i = 0; i < drawMap.GetLength (1); i++) {
				for (int j = 0; j < drawMap.GetLength (0); j++) {
					drawMap [i, j] = myHeightMap [i, j] * update;
				}
			}
			currentTerrain.terrainData.SetHeights (0, 0, drawMap);
		}
		frame ++;
		if(update < 1f)
			update += 0.006f;
	}

}
