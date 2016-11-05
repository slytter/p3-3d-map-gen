using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class imageProcModules : MonoBehaviour {



	/// <summary>
	/// Flood fills.
	/// </summary>
	/// <returns>The fill.</returns>
	/// <param name="inputPicture">Input picture.</param>
	public bool [,] floodFill (bool[,] inputPicture) {
		inputPicture = blackFrame (inputPicture);
		bool[,] inputPictureEdge = new bool[inputPicture.GetLength(0), inputPicture.GetLength(1)];
		Buffer.BlockCopy (inputPicture, 0, inputPictureEdge, 0, inputPicture.Length * sizeof(bool));
		Debug.Log ("længden af inputPicture: " + inputPicture.Length + "længden af edge array: " + inputPictureEdge.Length);


		List<int> xPos = new List<int> ();
		List<int> yPos = new List<int> ();

		xPos.Add (1);
		yPos.Add (1);

		int listIndex = 0;
		int[] kernelX = { 0, 1, 0, -1 };
		int[] kernelY = { -1, 0, 1, 0 };

		while (listIndex != xPos.Count && listIndex != yPos.Count){
			for (int i = 0; i < 4; i++){
				if (xPos.ElementAt (listIndex) < inputPicture.GetLength(0) - 2 && xPos.ElementAt (listIndex) > 0 && yPos.ElementAt (listIndex) < inputPicture.GetLength(1) - 2 && yPos.ElementAt (listIndex) > 0) {
					if (inputPicture [xPos.ElementAt (listIndex) + kernelX [i], yPos.ElementAt (listIndex) + kernelY [i]] == false) {
						xPos.Add (xPos.ElementAt (listIndex) + kernelX [i]);
						yPos.Add (yPos.ElementAt (listIndex) + kernelY [i]);
						inputPicture [xPos.ElementAt (listIndex) + kernelX [i], yPos.ElementAt (listIndex) + kernelY [i]] = true;
					}
				}
			}
			listIndex++;
		}

		//adds the edges
		for (int y = 2; y < inputPicture.GetLength(1) - 2; y++)
		{
			for (int x = 2; x < inputPicture.GetLength(0) - 2; x++)
			{
				if (inputPictureEdge [x, y] == true && inputPicture [x, y] == true)
				{
					inputPicture [x, y] = false;
				}
			}	
		}

		return invert(inputPicture);
	}



	public bool[,] invert (bool [,] boolArray){
		for (int x = 0; x < boolArray.GetLength(0); x++) {
			for (int y = 0; y < boolArray.GetLength(1); y++) {
				boolArray [x, y] = !boolArray [x, y];
			}
		}
		return boolArray;
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



	/// <summary>
	/// Dilate the specified boolean array.
	/// </summary>
	/// <param name="bools">Bools.</param>
	public bool[,] dilation (bool[,] bools){
		bool[,] returnedBools = new bool[bools.GetLength(0),bools.GetLength(1)];

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



	/// <summary>
	/// Converts boolean array to float array.
	/// </summary>
	/// <returns>The to float.</returns>
	/// <param name="toBeConverted">To be converted.</param>
	public float [,] boolToFloat(bool[,] toBeConverted){
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



	/// <summary>
	/// Converts float array to boolean array.
	/// </summary>
	/// <returns>The to bool.</returns>
	/// <param name="toBeConverted">To be converted.</param>
	public bool [,] floatToBool(float [,] toBeConverted){
		bool[,] outputBoolArray = new bool[toBeConverted.GetLength (1), toBeConverted.GetLength (0)];
		for (int y = 0; y < toBeConverted.GetLength (1); y++) {
			for (int x = 0; x < toBeConverted.GetLength (0); x++) {

				if (toBeConverted [x, y] > 0.5f) {
					outputBoolArray [x, y] = true; 
				}
			}
		}

		return outputBoolArray; 
	}



	/// <summary>
	/// Adds true border, adds false inline border.
	/// </summary>
	/// <returns>The frame.</returns>
	/// <param name="boolArrayToBeFramed">Bool array to be framed.</param>
	private bool [,] blackFrame(bool[,] boolArrayToBeFramed) {
		for (int y = 0; y < boolArrayToBeFramed.GetLength(1); y++) {
			for (int x = 0; x < boolArrayToBeFramed.GetLength(0); x++) {

				if (y == 0)
					boolArrayToBeFramed [x, y] = true; 
				if (y == 1) 
					boolArrayToBeFramed [x, y] = false; 
				
				if( y == boolArrayToBeFramed.GetLength (1) - 1 )
					boolArrayToBeFramed [x, y] = true; 
				if( y == boolArrayToBeFramed.GetLength (1) - 2 ) 
					boolArrayToBeFramed [x, y] = false; 
				
				if (x == 0)
					boolArrayToBeFramed [x, y] = true;
				if (x == 1)
					boolArrayToBeFramed [x, y] = false;
				
				if (x == boolArrayToBeFramed.GetLength (0) - 1 ) 
					boolArrayToBeFramed [x, y] = true;
				if(x == boolArrayToBeFramed.GetLength (0) - 2) 
					boolArrayToBeFramed [x, y] = false;

			}
		}
		return boolArrayToBeFramed; 
	}
}
