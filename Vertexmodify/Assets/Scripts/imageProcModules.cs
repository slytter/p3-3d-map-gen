﻿using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class imageProcModules : MonoBehaviour
{

	/// <summary>
	/// Perlin the specified inp.
	/// </summary>
	/// <param name="inp">Inp.</param>
	public float [,] perlin (float[,] inp, float baseHeight, float intensity, float density)
	{
		TimingModule.timer ("perlinModule", "start");
		float px, py;
		for (int y = 0; y < inp.GetLength (1); y++) {
			for (int x = 0; x < inp.GetLength (0); x++) {
				px = (float)x / density;
				py = (float)y / density;
				inp [x, y] += Mathf.PerlinNoise (px, py) / intensity + baseHeight;
			}
		}
		TimingModule.timer ("perlinModule", "end");

		return inp;
	}


	public bool [,] floodFillQueue (bool[,] inputPicture)
	{
		TimingModule.timer ("floodFillModule", "start");
		inputPicture = blackFrame (inputPicture);
		bool[,] inputPictureEdge = new bool[inputPicture.GetLength (0), inputPicture.GetLength (1)];
		Buffer.BlockCopy (inputPicture, 0, inputPictureEdge, 0, inputPicture.Length * sizeof(bool));

		Queue<int> qX = new Queue<int> ();
		Queue<int> qY = new Queue<int> ();

		qX.Enqueue (1);
		qY.Enqueue (1);

		int[] kernelX = { 0, 1, 0, -1 };
		int[] kernelY = { -1, 0, 1, 0 };

		while (qX.Count != 0 && qY.Count != 0) {
			for (int i = 0; i < 4; i++) {
				if (qX.Peek () < inputPicture.GetLength (0) - 2 && qX.Peek () > 0 && qY.Peek () < inputPicture.GetLength (1) - 2 && qY.Peek () > 0) {
					if (inputPicture [qX.Peek () + kernelX [i], qY.Peek () + kernelY [i]] == false) {
						qX.Enqueue (qX.Peek () + kernelX [i]);
						qY.Enqueue (qY.Peek () + kernelY [i]);
						inputPicture [qX.Peek () + kernelX [i], qY.Peek () + kernelY [i]] = true;
					}
				}

			}
			qX.Dequeue ();
			qY.Dequeue ();
		}

		for (int y = 2; y < inputPicture.GetLength (1) - 2; y++) {
			for (int x = 2; x < inputPicture.GetLength (0) - 2; x++) {
				if (inputPictureEdge [x, y] == true && inputPicture [x, y] == true) {
					inputPicture [x, y] = false;
				}
				
			}
		}
		TimingModule.timer ("floodFillModule", "end");
		return invert (inputPicture);
	}



	/// <summary>
	/// Invert the specified boolArray.
	/// </summary>
	/// <param name="boolArray">Bool array.</param>
	public bool[,] invert (bool[,] boolArray)
	{
		TimingModule.timer ("invertModule", "start");
		for (int y = 0; y < boolArray.GetLength (1); y++) {
			for (int x = 0; x < boolArray.GetLength (0); x++) {
				boolArray [x, y] = !boolArray [x, y];
			}
		}
		TimingModule.timer ("invertModule", "end");

		return boolArray;
	}


	/// <summary>
	/// Creates random value 2D array.
	/// </summary>
	/// <returns>The value gen.</returns>
	/// <param name="height">Height.</param>
	/// <param name="length">Length.</param>
	public float[,] randomValGen (int height, int length)
	{
		TimingModule.timer ("randomValueGenerator", "start");
		float[,] randomValues = new float[height, length];
		for (int y = 0; y < randomValues.GetLength (1); y++) {
			for (int x = 0; x < randomValues.GetLength (0); x++) {
				randomValues [x, y] = UnityEngine.Random.Range (0f, 1f);
			}
		}
		TimingModule.timer ("randomValueGenerator", "end");

		return randomValues;
	}



	/// <summary>
	/// Gaussian the specified 2D Array by using smoothing.
	/// </summary>
	/// <param name="heightMap">2D float height map.</param>
	/// <param name="smoothing">Number of iterations.</param>
	public float[,] gaussian (float[,] heightMap, int smoothing)
	{
		TimingModule.timer ("gaussianModule", "start");
		for (int k = 0; k < smoothing; k++) {
			for (int i = 1; i < heightMap.GetLength (0) - 1; i++) {
				for (int j = 1; j < heightMap.GetLength (1) - 1; j++) {
					float blur = (
					                 heightMap [i, j]

					                 + heightMap [i + 1, j + 1] * 2
					                 + heightMap [i + 1, j] * 2
					                 + heightMap [i, j + 1] * 2

					                 + heightMap [i - 1, j - 1] * 2
					                 + heightMap [i - 1, j] * 2
					                 + heightMap [i, j - 1] * 2

					                 + heightMap [i + 1, j - 1] * 2
					                 + heightMap [i - 1, j + 1] * 2
					             )
					             / (9 + 8);
					heightMap [i, j] = blur;	
				}
			}
		}
		TimingModule.timer ("gaussianModule", "end");

		return heightMap;
	}



	/// <summary>
	/// Dilate the specified boolean array.
	/// </summary>
	/// <param name="bools">Bools.</param>
	public bool[,] dilation (bool[,] bools)
	{
		TimingModule.timer ("dilationModule", "start");

		bool[,] returnedBools = new bool[bools.GetLength (0), bools.GetLength (1)];

		for (int y = 1; y < bools.GetLength (1) - 2; y++) {
			for (int x = 1; x < bools.GetLength (0) - 2; x++) {
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
		TimingModule.timer ("dilationModule", "end");

		return returnedBools;
	}


	bool[,] erosion (bool[,] bools)
	{
		bool[,] returnedBools = new bool[bools.GetLength (0), bools.GetLength (1)];
		Array.Copy (bools, returnedBools, 0);
		for (int y = 1; y < bools.GetLength (1) - 1; y++) {
			for (int x = 1; x < bools.GetLength (0) - 1; x++) {
				if (bools [x - 1, y - 1] && bools [x - 1, y] && bools [x, y - 1] && bools [x + 1, y - 1] && bools [x - 1, y + 1] && bools [x + 1, y + 1] && bools [x + 1, y] && bools [x, y + 1] && bools [x, y]) {
					returnedBools [x, y] = true;
				} else {
					returnedBools [x, y] = false;
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
	public float [,] boolToFloat (bool[,] toBeConverted)
	{
		TimingModule.timer ("boolToFloatModule", "start");

		float[,] outputFloatArray = new float[toBeConverted.GetLength (0), toBeConverted.GetLength (1)]; 
		for (int y = 0; y < toBeConverted.GetLength (1); y++) {
			for (int x = 0; x < toBeConverted.GetLength (0); x++) {
				if (toBeConverted [x, y] == true) {
					outputFloatArray [x, y] = 1; 
				}
			}
		}
		TimingModule.timer ("boolToFloatModule", "end");

		return outputFloatArray; 
	}



	/// <summary>
	/// Converts float array to boolean array.
	/// </summary>
	/// <returns>The to bool.</returns>
	/// <param name="toBeConverted">To be converted.</param>
	public bool [,] floatToBool (float[,] toBeConverted)
	{
		TimingModule.timer ("floatToBoolModule", "start");


		bool[,] outputBoolArray = new bool[toBeConverted.GetLength (1), toBeConverted.GetLength (0)];
		for (int y = 0; y < toBeConverted.GetLength (1); y++) {
			for (int x = 0; x < toBeConverted.GetLength (0); x++) {

				if (toBeConverted [x, y] > 0.5f) {
					outputBoolArray [x, y] = true; 
				}
			}
		}
		TimingModule.timer ("floatToBoolModule", "end");

		return outputBoolArray; 
	}

	public float[,] generateTrees (float[,] inputArea, int treeSpace)
	{
		// when treePositions[0,something], a tree's x position is accessed
		// when treePositions[1,something], a tree's y position is accessed
		// the y index is set to 10000 to ensure that there are space for enough trees to be spawned
		float[,] treePositions = new float[2, 10000];
		// Ensures that trees look randomly placed
		float nextXPosition = UnityEngine.Random.Range (2f, 5f); 
		float nextYPosition = UnityEngine.Random.Range (2f, 5f);
		// ensures that the trees are not too close to each other
   
		// keeps track of how many trees are spawned
		int index = 0; 

		for (int y = 0; y < inputArea.GetLength (1); y += treeSpace) {
			for (int x = 0; x < inputArea.GetLength (0); x += treeSpace) {
				// Checks if the pixel scanned is white, and if the x and y positions are within the map
				if (inputArea [x, y] == 1f && x + nextXPosition <= inputArea.GetLength (0) && y + nextYPosition <= inputArea.GetLength (1)) {
					treePositions [0, index] = ((float)x + nextXPosition) / inputArea.GetLength (0); 
					treePositions [1, index] = ((float)y + nextYPosition) / inputArea.GetLength (1);
					index++; 

					nextXPosition = UnityEngine.Random.Range (2f, 5f); 
					nextYPosition = UnityEngine.Random.Range (2f, 5f);
				} 
			}
		}
		// The array is resized to the number of tree's that are spawned, so 10000 trees are not spawned everytime.w
		float[,] output = new float[2, index];
		for (int i = 0; i < output.GetLength (1); i++) {
			output [0, i] = treePositions [0, i];
			output [1, i] = treePositions [1, i];
		}
		return output;
	}



	/// <summary>
	/// Adds true border, adds false inline border.
	/// </summary>
	/// <returns>The frame.</returns>
	/// <param name="boolArrayToBeFramed">Bool array to be framed.</param>
	private bool [,] blackFrame (bool[,] boolArrayToBeFramed)
	{
		TimingModule.timer ("blackframeModule", "start");

		for (int y = 0; y < boolArrayToBeFramed.GetLength (1); y++) {
			for (int x = 0; x < boolArrayToBeFramed.GetLength (0); x++) {

				if (y == 0)
					boolArrayToBeFramed [x, y] = true; 
				if (y == 1)
					boolArrayToBeFramed [x, y] = false; 
				
				if (y == boolArrayToBeFramed.GetLength (1) - 1)
					boolArrayToBeFramed [x, y] = true; 
				if (y == boolArrayToBeFramed.GetLength (1) - 2)
					boolArrayToBeFramed [x, y] = false; 
				
				if (x == 0)
					boolArrayToBeFramed [x, y] = true;
				if (x == 1)
					boolArrayToBeFramed [x, y] = false;
				
				if (x == boolArrayToBeFramed.GetLength (0) - 1)
					boolArrayToBeFramed [x, y] = true;
				if (x == boolArrayToBeFramed.GetLength (0) - 2)
					boolArrayToBeFramed [x, y] = false;

			}
		}
		TimingModule.timer ("blackframeModule", "end");

		return boolArrayToBeFramed; 
	}




	/// <summary>
	/// Subtract the specified .
	/// </summary>
	/// <param name="Base">Base.</param>
	/// <param name="subtract">Subtract.</param>
	public float [,] subtract (float[,] Base, float[,] subtract)
	{
		TimingModule.timer ("subtractModule", "start");

		for (int y = 0; y < Base.GetLength (1); y++) {
			for (int x = 0; x < Base.GetLength (0); x++) {

				Base [x, y] -= subtract [x, y];

			}
		}
		TimingModule.timer ("subtractModule", "end");


		return Base;
	}




	/// <summary>
	/// Add the specified Base and add.
	/// </summary>
	/// <param name="Base">Base.</param>
	/// <param name="add">Add.</param>
	public float [,] add (float[,] Base, float[,] add)
	{
		TimingModule.timer ("addModule", "start");

		for (int y = 0; y < Base.GetLength (1); y++) {
			for (int x = 0; x < Base.GetLength (0); x++) {

				Base [x, y] += add [x, y];

			}
		}
		TimingModule.timer ("addModule", "end");

		return Base;
	}


	/// <summary>
	/// Compressing and subtracting subtractor from base.
	/// </summary>
	/// <returns>The and subtract.</returns>
	/// <param name="Base">Base.</param>
	/// <param name="subtractor">Subtractor.</param>
	public float [,] compressAndSubtract (float[,] Base, float[,] subtractor)
	{
		TimingModule.timer ("riverGenerateModule", "start");

		for (int y = 0; y < Base.GetLength (1); y++) {
			for (int x = 0; x < Base.GetLength (0); x++) {
				Base [x, y] -= (subtractor [x, y]) * Base [x, y];
			}
		}
		TimingModule.timer ("riverGenerateModule", "end");

		return Base;
	}


	public float[,] flip (float[,] inp)
	{
		return inp;
		for (int y = 0; y < inp.GetLength (1); y++) {
			for (int x = 0; x < inp.GetLength (0); x++) {
				inp [x, y] = inp [y, x];
			}
		}
		return inp;
	}







	/// <summary>
	/// Grasses the fire.
	/// </summary>
	/// <returns>The fire.</returns>
	/// <param name="inputPicture">Input picture.</param>
	public Blob[] grassFire (bool[,] inputPicture)
	{

		float[,] blobsGradient = new float[inputPicture.GetLength (0), inputPicture.GetLength (1)];
		float[,] blobsEdges = new float[inputPicture.GetLength (0), inputPicture.GetLength (1)];

		int blobTag = 1; //0 is background

		Queue<int> qX = new Queue<int> ();
		Queue<int> qY = new Queue<int> ();
		
		int[] kernelX = { 0, 1, 0, -1 };
		int[] kernelY = { -1, 0, 1, 0 };
		
		for (int y = 2; y < inputPicture.GetLength (1) - 2; y++) {
			for (int x = 2; x < inputPicture.GetLength (0) - 2; x++) {
				if (inputPicture [x, y] == true) {
					
					qX.Enqueue (x);
					qY.Enqueue (y);
					inputPicture [x, y] = false;
					blobsGradient [x, y] = blobTag;

					while (qX.Count != 0 && qY.Count != 0) {
						for (int i = 0; i < 4; i++) {
							if (inputPicture [qX.Peek () + kernelX [i], qY.Peek () + kernelY [i]]) {

								qX.Enqueue (qX.Peek () + kernelX [i]);
								qY.Enqueue (qY.Peek () + kernelY [i]);

								inputPicture [qX.Peek () + kernelX [i], qY.Peek () + kernelY [i]] = false;
								blobsGradient [qX.Peek () + kernelX [i], qY.Peek () + kernelY [i]] = (float)blobTag;
							
							} else if (blobsGradient [qX.Peek () + kernelX [i], qY.Peek () + kernelY [i]] == 0) {
								blobsEdges [qX.Peek (), qY.Peek ()] = (float)blobTag;
							}

		
						}

						qX.Dequeue ();
						qY.Dequeue ();
					}
					blobTag += 1;
				}
			}
		}

		//	float[,] blobsGradient = new float[inputPicture.GetLength (0), inputPicture.GetLength (1)];
		// 	float[,] blobsEdges = new float[inputPicture.GetLength (0), inputPicture.GetLength (1)];


		Blob[] blobs = classifyBlobs (blobsGradient, blobsEdges, blobTag);


		return blobs;
	}



	private Blob[] classifyBlobs (float[,] blobsGradient, float[,] blobsEdges, int blobTag)
	{
		
		//INITIALIZING:
		ColorDetection colorScanScript = GameObject.Find ("colorScan").GetComponent<ColorDetection> ();
		Blob[] blobs = new Blob[blobTag];
		for (int i = 0; i < blobTag; i++) {
			blobs [i] = new Blob ();
		}


		//FINDING CENTER OF MASS & AREA:
		for (int y = 2; y < blobsGradient.GetLength (1) - 2; y++) {
			for (int x = 2; x < blobsGradient.GetLength (0) - 2; x++) {
				if (blobsGradient [x, y] != 0f) {
					//print ("COM tag: " + blobsGradient [x, y]);
					blobs [(int)blobsGradient [x, y]].CenterOfMass.y += y;
					blobs [(int)blobsGradient [x, y]].CenterOfMass.x += x;
					blobs [(int)blobsGradient [x, y]].area++;
				}
			}
		}

		for (int i = 0; i < blobs.GetLength (0); i++) {
			blobs [i].CenterOfMass.x = blobs [i].CenterOfMass.x / blobs [i].area;
			blobs [i].CenterOfMass.y = blobs [i].CenterOfMass.y / blobs [i].area;
			if ((int)blobs [i].CenterOfMass.x < 512 && (int)blobs [i].CenterOfMass.y < 512) {
				if ((int)blobs [i].CenterOfMass.x > 0 && (int)blobs [i].CenterOfMass.y > 0) {
					//	blobsEdges [(int)blobs [i].CenterOfMass.x, (int)blobs [i].CenterOfMass.y] = 1f;
				}
			} 
		}





		int incr = 0;
		float[] lengths = new float[10000]; //set to edge area!!
		float[] angles = new float[10000];

		for (int y = 0; y < blobsEdges.GetLength (1); y++) {
			for (int x = 0; x < blobsEdges.GetLength (0); x++) {
				if (blobsEdges [x, y] == 3) {
					Vector2 edge = new Vector2 (x, y);
					int tag = (int)blobsEdges [x, y];
					Vector2 cm = new Vector2 (blobs [tag].CenterOfMass.x, blobs [tag].CenterOfMass.y);
					lengths [incr] = Vector2.Distance (cm, edge);
					angles [incr] = (float)(Mathf.Atan2 (cm.y - edge.y, cm.x - edge.x)) * Mathf.Rad2Deg + 180f;
					incr++;

					Debug.DrawLine (new Vector3 (edge.x, 60f, edge.y), new Vector3 (cm.x, 60f, cm.y), Color.red, 10000f);
				}
			}
		}

		Array.Resize<float> (ref angles, incr);
		Array.Resize<float> (ref lengths, incr);

		float[] meanLength = 0f;

		float[] sortedAngles = new float[incr];
		Array.Copy (angles, sortedAngles, incr);
		Array.Sort (sortedAngles);

		float[] sortedLengths = new float[incr];


		for (int i = 0; i < sortedAngles.Length; i++) {
			print ("angle: " + sortedAngles [i]);
			int unsortedIndex = findIndex (angles, sortedAngles [i]);

			sortedLengths [i] = lengths [unsortedIndex];


			print (lengths [unsortedIndex]);
		}

		for (int x = 0; x < sortedAngles.Length; x++) {
			Debug.DrawLine (new Vector3 (x, 60f, 0), new Vector3 (x, 60f, sortedLengths [x]), Color.blue, 10000f);
		}


		colorScanScript.printBinary (blobsEdges);
		return blobs;
	}

	public int findIndex (float[] inp, float search)
	{
		for (int i = 0; i < inp.Length; i++) {
			if (inp [i] == search) {
				return i;
			}
		}
		return 0;
	}


}







public static class lineDrawVars
{
	public static Vector3[] fromPoint = new Vector3[3000];
	public static Vector3[] toPoint = new Vector3[3000];
}






public class TimingModule
{
	static float startTimeOfModule;
	static float endTimeOfModule;

	static float startTimeOfProgram;
	static float endTimeOfProgram;

	/// <summary>
	/// Timer the specified name and status.
	/// </summary>
	/// <param name="name">Name.</param>
	/// <param name="status">Status.</param>
	public static void timer (string name, string status)
	{
		if (name == "program" && status == "start") {
			startTimeOfProgram = Time.realtimeSinceStartup;
		}

		if (name == "program" && status == "end") {
			endTimeOfProgram = Time.realtimeSinceStartup;
//			Debug.Log (name + " took " + (endTimeOfProgram - startTimeOfProgram) * 1000 + " miliseconds");
		}

		if (name != "program" && status == "start") {
			startTimeOfModule = Time.realtimeSinceStartup;
		}

		if (name != "program" && status == "end") {
			endTimeOfModule = Time.realtimeSinceStartup;
//			Debug.Log (name + " took " + (endTimeOfModule - startTimeOfModule) * 1000 + " miliseconds");
		}


	}


}


public class Blob
{
	public int number;
	public int area;
	public Vector2 CenterOfMass = new Vector2 (0, 0);
}

