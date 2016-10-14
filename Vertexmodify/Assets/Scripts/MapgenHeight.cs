using UnityEngine;
using System.Collections;
using System;
public class MapgenHeight : MonoBehaviour {
	public int length;
	public int height;
	Terrain currentTerrain;
	float update = 0;
	int frame = 0;
	float[,] myHeightMap = new float[512, 512];
	float[,] drawMap = new float[512, 512];
	float rounder;
	float lastmillis;
	float max_val = 0;

	void Start () {
		
//		for (int i = 1; i < 8; i++) {
//			myHeightMap = mountainRecursion (i*i, myHeightMap, 1f / i);
//		}
		float lastmillis = Time.realtimeSinceStartup;
		float startit = Time.realtimeSinceStartup;
		//myHeightMap = mountainRecursion (1, myHeightMap, 0.9f, 15);
		myHeightMap = mountainRecursion (2, myHeightMap, 0.2f, 20);
		myHeightMap = mountainRecursion (4, myHeightMap, 0.8f, 10);
		myHeightMap = mountainRecursion (8, myHeightMap, 0.7f, 10);
		myHeightMap = mountainRecursion (16, myHeightMap, 0.5f, 5);
		myHeightMap = mountainRecursion (32, myHeightMap, 0.3f, 0);
		myHeightMap = mountainRecursion (64, myHeightMap, 0.2f, 0);
		myHeightMap = mountainRecursion (128, myHeightMap, 0.2f, 0);

		myHeightMap = finalMap (myHeightMap);
		Debug.Log("Total millis for all recursions: " + ((Time.realtimeSinceStartup - startit ) * 1000));

		currentTerrain = Terrain.activeTerrain;

	}


	/// <summary>
	/// Creates random value 2D array.
	/// </summary>
	/// <returns>The value gen.</returns>
	/// <param name="height">Height.</param>
	/// <param name="length">Length.</param>
	float[,] randomValGen(int height, int length){
		float[,] randomValues = new float[height, length];
		for (int i = 0; i < randomValues.GetLength (1); i++) {
			for (int j = 0; j < randomValues.GetLength (0); j++) {
				randomValues [i,j] = UnityEngine.Random.Range (0f, 1f);
			}
		}
		return randomValues;
	}
		
	/// <summary>
	/// Mountain generator, taking the current recursion, the 2 heightmap
	/// </summary>
	/// <returns>The recursion.</returns>
	/// <param name="recursion">Recursion.</param>
	/// <param name="heightMap">heightMap.</param>
	/// <param name="amount">Amount.</param>
	float[,] mountainRecursion(int recursion, float[,] heightMap, float amount, int gaussianAmount){
		// float amount goes from 0 to 1. and is  
		// Get current date and time 
		// Date
		Debug.Log("Millis for iteration " + recursion + ": "  + ((Time.realtimeSinceStartup-lastmillis)*1000));
		lastmillis = Time.realtimeSinceStartup;

		// Calculate the number of milliseconds since midnight

		float[,] randomValues = randomValGen (512, 512);
		int splitLength;
		float currentTerrainHeight;

		splitLength = heightMap.GetLength (1) / recursion;

		int randomArrayPointerX = 0;
		int randomArrayPointerY = 0;

		for (int i = 0; i < heightMap.GetLength(1)-1; i ++) {
			//if(i % 10 == 0) rounder = Mathf.Abs(Mathf.Sin (Mathf.PI * i / splitLength)) / 4;  	//rounder
			if (i % splitLength == 0) 	randomArrayPointerY = i / splitLength; //same here

			for (int j = 0; j < heightMap.GetLength (0)-1; j++) {
				
				if (j % splitLength == 0) {
					if (j == 0) randomArrayPointerX = 0;
					randomArrayPointerX = j / splitLength; 
				}

				currentTerrainHeight = randomValues [randomArrayPointerX, randomArrayPointerY];

				if (amount == 1) {
					heightMap [i, j] = Math.Abs(currentTerrainHeight + rounder);
				} else{
					heightMap [i, j] += Math.Abs(amount/2 - currentTerrainHeight * amount);
				}

				if (heightMap [i, j] > max_val) {
					max_val = heightMap [i, j];
				}

			}
		}
		heightMap = gaussian (heightMap, gaussianAmount);
		//Debug.Log ("maimum: " + max_val);
		return heightMap;
	}


	float[,] finalMap(float[,] heightMap) {
		heightMap = gaussian (heightMap, 5);

		for (int i = 0; i < heightMap.GetLength (1); i++) {
			for (int j = 0; j < heightMap.GetLength (0); j++) {
				heightMap [i, j] = heightMap [i, j] / max_val;
			}
		}
		return heightMap;
	}
		
	/// <summary>
	/// Gaussian the specified 2dArray by using smoothing.
	/// </summary>
	/// <param name="heightMap">2D float height map.</param>
	/// <param name="smoothing">Number of blob iterations.</param>
	float[,] gaussian(float[,] heightMap, int smoothing){
		for (int k = 0; k < smoothing; k++) {
			for (int i = 1; i < heightMap.GetLength (1)-1; i++) {
				for (int j = 1; j < heightMap.GetLength (0)-1; j++) {
					float blur = (
						heightMap [i, j] 

						+ heightMap [i + 1, j + 1] 		* 2 
						+ heightMap [i + 1, j] 			* 2
						+ heightMap [i, j + 1] 			* 2

						+ heightMap [i - 1, j - 1] 		* 2  
						+ heightMap [i - 1, j] 			* 2
						+ heightMap [i, j - 1] 			* 2

						+ heightMap [i + 1, j - 1] 		* 2
						+ heightMap [i - 1, j + 1] 		* 2
					) 
						/ (9 + 8);
					heightMap [i, j] = blur;	
				}
			}
		}
		return heightMap;
	}

	//---------UPDATE---------//

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
