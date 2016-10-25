using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class imageProcModules : MonoBehaviour {


	/// <summary>
	/// Grasses the fire.
	/// </summary>
	/// <returns>The fire.</returns>
	/// <param name="testArray">Test array.</param>
	/// <param name="nodeX">Node x.</param>
	/// <param name="nodeY">Node y.</param>
	public bool [,] floodFill(bool[,]inputPicture)
	{
		List<bool> list = new List<bool> ();


		for (int y = 1; y < inputPicture.GetLength(1) - 1; y++)
		{
			for (int x = 1; x < inputPicture.GetLength(0) - 1; x++)
			{
				if (inputPicture[x, y] == true)
				{
					
					
				}
			}
		}
	}



	/// <summary>
	/// Creates random value 2D array.
	/// </summary>
	/// <returns>The value gen.</returns>
	/// <param name="height">Height.</param>
	/// <param name="length">Length.</param>
	public float[,] randomValGen(int height, int length){
		float[,] randomValues = new float[height, length];
		for (int i = 0; i < randomValues.GetLength (1); i++) {
			for (int j = 0; j < randomValues.GetLength (0); j++) {
				randomValues [i,j] = UnityEngine.Random.Range (0f, 1f);
			}
		}
		return randomValues;
	}



	/// <summary>
	/// Gaussian the specified 2D Array by using smoothing.
	/// </summary>
	/// <param name="heightMap">2D float height map.</param>
	/// <param name="smoothing">Number of iterations.</param>
	public float[,] gaussian(float[,] heightMap, int smoothing){
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

	bool[,] dilation (bool[,] bools){
		bool[,] returnedBools = new bool[512,512];
		Array.Copy (bools, returnedBools,0);
		for (int y = 1; y < bools.GetLength(1)-2; y++) {
			for (int x = 1; x < bools.GetLength(0)-2; x++) {
				bool hit = false;
				for (int i = -1; i < 2; i++) {
					for (int j = -1; j < 2; j++) {
						if (hit) {
							returnedBools [x - j, y - i] = true;
						} else if (bools [x - j, y - i]) {
							hit = true;
							i = -1;
							j = -1;
						}
					}
				}
			}
		}
		return returnedBools;
	}

	float [,] boolToFloat(bool[,] toBeConverted){
	
		float[,] outputFloatArray = new float[toBeConverted.GetLength (1), toBeConverted.GetLength (0)]; 

		for (int y = 0; y < toBeConverted.GetLength(1); y++) {
			for (int x = 0; x < toBeConverted.GetLength(0); x++) {
				if (toBeConverted [y, x] == true) {
					outputFloatArray [y, x] = 1; 
				}
			}
		}
		return outputFloatArray; 
	}

	bool [,] floatToBool(float [,] toBeConverted){
	
		bool[,] outputBoolArray = new bool[toBeConverted.GetLength (1), toBeConverted.GetLength (0)];

		for (int y = 0; y < toBeConverted.GetLength (1); y++) {
			for (int x = 0; x < toBeConverted.GetLength (0); x++) {

				if (toBeConverted [x, y] > 1) {
					outputBoolArray [x, y] = true; 
				}
			}
		}

		return outputBoolArray; 
	}

	bool [,] blackFrame(bool[,] boolArrayToBeFramed){

		for (int y = 0; y < boolArrayToBeFramed.GetLength(1); y++) {
			for (int x = 0; x < boolArrayToBeFramed.GetLength(0); x++) {

				if (y == 0 || y == 1) {
					boolArrayToBeFramed [x, y] = false; 
				} else if (y == boolArrayToBeFramed.GetLength (1) || y == boolArrayToBeFramed.GetLength (1) - 1) {
					boolArrayToBeFramed [x, y] = false; 
				} else if (x == 0 || x == 1) {
					boolArrayToBeFramed [x, y] = false;
				} else if (x == boolArrayToBeFramed.GetLength (0) || x == boolArrayToBeFramed.GetLength (0) - 1) {
					boolArrayToBeFramed [x, y] = false;
				}	
			}
		}
		return boolArrayToBeFramed; 

	}

}
