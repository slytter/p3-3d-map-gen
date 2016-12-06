using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


static public class blobClassify
{
	public static bool debug;

	/// <summary>
	/// Grasses the fire.
	/// </summary>
	/// <returns>The fire.</returns>
	/// <param name="inputPicture">Input picture.</param>
	public static Blob[] grassFire (bool[,] inputPicture)
	{
		float[,] blobsGradient = new float[inputPicture.GetLength (0), inputPicture.GetLength (1)];
		float[,] blobsEdges = new float[inputPicture.GetLength (0), inputPicture.GetLength (1)];

		int blobTag = 1; // 0 is background


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
		Blob[] blobs = classifyBlobs (blobsGradient, blobsEdges, blobTag);
		return blobs;
	}




	/// <summary>
	/// Classifies the blobs.
	/// </summary>
	/// <returns>The blobs.</returns>
	/// <param name="blobsGradient">Blobs gradient.</param>
	/// <param name="blobsEdges">Blobs edges.</param>
	/// <param name="blobTag">BLOB tag.</param>
	private static Blob[] classifyBlobs (float[,] blobsGradient, float[,] blobsEdges, int blobTag)
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

				if (blobsEdges [x, y] != 0f)
				{
					blobs [(int)blobsEdges [x, y]].edgeArea++;
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
		if (debug)
			colorScanScript.printBinary (blobsEdges);

		for (int i = 1; i < blobs.Length; i++) {
			blobs [i].number = i;
			blobs [i] = calculateCorners (blobsEdges, blobs, i);
		}

		return blobs;
	}



	/// <summary>
	/// Calculates the corners.
	/// </summary>
	/// <returns>The corners.</returns>
	private static Blob calculateCorners (float[,] blobsEdges, Blob[] blobs, int index)
	{
		int incr = 0;
		float[] lengths = new float[blobs [index].edgeArea]; //set to edge area!!
		float[] angles = new float[blobs [index].edgeArea];

		for (int y = 0; y < blobsEdges.GetLength (1); y++) {
			for (int x = 0; x < blobsEdges.GetLength (0); x++) {
				if (blobsEdges [x, y] == index) {
					Vector2 edge = new Vector2 (x, y);
					int tag = (int)blobsEdges [x, y];
					Vector2 cm = new Vector2 (blobs [tag].CenterOfMass.x, blobs [tag].CenterOfMass.y);
					lengths [incr] = Vector2.Distance (cm, edge);
					angles [incr] = (float)(Mathf.Atan2 (cm.y - edge.y, cm.x - edge.x)) * Mathf.Rad2Deg + 180f;
					incr++;
					if (debug)
						Debug.DrawLine (new Vector3 (edge.x, 60f, edge.y), new Vector3 (cm.x, 60f, cm.y), Color.red, 10000f);
				}
			}
		}
		float meanLength = 0f;
		float[] sortedAngles = new float[incr];
		Array.Copy (angles, sortedAngles, incr);
		Array.Sort (sortedAngles);

		float[] sortedLengths = new float[incr];


		for (int i = 0; i < sortedAngles.Length; i++) {
			int unsortedIndex = findIndex (angles, sortedAngles [i]);

			sortedLengths [i] = lengths [unsortedIndex];
			meanLength += sortedLengths [i];
		}
		meanLength /= sortedLengths.Length;

		bool under = true;
		int crossingTheMean = 0;
		if (sortedLengths [0] > meanLength) {
			under = false;
		}

		if (debug)
			Debug.DrawLine (new Vector3 (0, 60f, meanLength), new Vector3 (sortedAngles.Length, 60f, meanLength), Color.green, 10000f);

		int peak = 0;
		float peakAngle = 0;

		for (int x = 0; x < sortedAngles.Length; x++) {
			if (x > peak) {
				peak = x;
				peakAngle = sortedAngles [x];
			}
			if ((under && sortedLengths [x] > meanLength + 2f) || (!under && sortedLengths [x] < meanLength - 2f)) {
				under = !under;
				crossingTheMean++;
			}
			if (debug)
				Debug.DrawLine (new Vector3 (x, 60f, 0), new Vector3 (x, 60f, sortedLengths [x]), Color.blue, 10000f);
		}

		if (debug)
			Debug.Log ("Number of sides: " + (crossingTheMean / 2));
		if ((int)crossingTheMean % 2 == 1)
			Debug.LogError ("Blob has uneven cornercrossings, which resolves in the blob having " + (float)crossingTheMean / 2f + " corners");
		blobs [index].corners = crossingTheMean / 2;
		blobs [index].type = blobNamer (blobs [index].corners, false);
		blobs [index].angle = peakAngle;
		return blobs [index];
	}


	/// <summary>
	/// Finds the index.
	/// </summary>
	/// <returns>The index.</returns>
	/// <param name="inp">Inp.</param>
	/// <param name="search">Search.</param>
	public static int findIndex (float[] inp, float search)
	{
		for (int i = 0; i < inp.Length; i++) {
			if (inp [i] == search) {
				return i;
			}
		}
		return 0;
	}


	private static string blobNamer (int corners, bool star)
	{
		switch (corners) {
		case 0:
			return "Circle";
		case 3:
			return "Triangle";
		case 4:
			return "Square";
		case 5:
			return "Pentagon";
		case 6:
			return "Sixangle";

//		case (3):
//		return "Triangle Star";
//		case 4 && star:
//		return "Squared Triangle";
//		case 5 && star:
//		return "Pentagonic Triangle";
//		case 6 && star:
//		return "Sixangluar Triangle";
		


		default:
			return "Unclassified";
		}
	}
}

public class Blob
{
	public int number;
	public int area;
	public Vector2 CenterOfMass = new Vector2 (0, 0);
	public string type;
	public int corners;
	public float angle;
	public int edgeArea;
}