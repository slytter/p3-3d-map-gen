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
		Blob[] blobs = BlobFeatures (blobsGradient, blobsEdges, blobTag);
		return blobs;
	}

	/// <summary>
	/// Classifies the blobs.
	/// </summary>
	/// <returns>The blobs.</returns>
	/// <param name="blobsGradient">Blobs gradient.</param>
	/// <param name="blobsEdges">Blobs edges.</param>
	/// <param name="blobTag">BLOB tag.</param>
	private static Blob[] BlobFeatures (float[,] blobsGradient, float[,] blobsEdges, int blobTag)
	{
		//INITIALIZING:
		Blob[] blobs = new Blob[blobTag];
		for (int i = 0; i < blobTag; i++) {
			blobs [i] = new Blob ();
		}
		//FINDING CENTER OF MASS & AREA:
		for (int y = 2; y < blobsGradient.GetLength (1) - 2; y++) {
			for (int x = 2; x < blobsGradient.GetLength (0) - 2; x++) {
				if (blobsGradient [x, y] != 0f) {
					blobs [(int)blobsGradient [x, y]].CenterOfMass.y += y;
					blobs [(int)blobsGradient [x, y]].CenterOfMass.x += x;
					blobs [(int)blobsGradient [x, y]].area++;
				}
				if (blobsEdges [x, y] != 0f) {
					blobs [(int)blobsEdges [x, y]].edgeArea++;
				}
			}
		}
		for (int i = 0; i < blobs.Length; i++) {
			blobs [i].CenterOfMass.x = blobs [i].CenterOfMass.x / blobs [i].area;
			blobs [i].CenterOfMass.y = blobs [i].CenterOfMass.y / blobs [i].area;
			if (i >= 1) {
				blobs [i].tagNumber = i;
				blobs [i] = BlobClassification (blobsEdges, blobs, i);
			}
		}

		return blobs;
	}

	/// <summary>
	/// BLOB classification.
	/// </summary>
	/// <returns>The classification.</returns>
	/// <param name="blobsEdges">Blobs edges.</param>
	/// <param name="blobs">Blobs.</param>
	/// <param name="index">Index.</param>
	private static Blob BlobClassification (float[,] blobsEdges, Blob[] blobs, int index)
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
						Debug.DrawLine (new Vector3 (edge.y, 60f, edge.x), new Vector3 (cm.y, 60f, cm.x), Color.red, 10000f);
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
		for (int x = 0; x < sortedAngles.Length; x++) {
			if (debug)
				Debug.DrawLine (new Vector3 (x + (100 * index), 60f, 0), new Vector3 (x + (100 * index), 60f, sortedLengths [x]), Color.green, 10000f);
		}
		sortedLengths = oneDMeanFilter (sortedLengths);
		for (int x = 0; x < sortedAngles.Length; x++) {
			if ((under && sortedLengths [x] > meanLength + 1f) || (!under && sortedLengths [x] < meanLength - 1f)) {
				under = !under;
				crossingTheMean++;
			}
			if (debug)
				Debug.DrawLine (new Vector3 (x + (100 * index), 60f, 0), new Vector3 (x + (100 * index), 60f, sortedLengths [x]), Color.blue, 10000f);
		}
		if (debug)
			Debug.Log ("Number of sides: " + (crossingTheMean / 2));
		if (crossingTheMean % 2 == 1) {
			Debug.LogError ("Blob has uneven cornercrossings, which resolves in the blob having " + (float)crossingTheMean / 2f + " corners");
			crossingTheMean++;
		}
		blobs [index].corners = crossingTheMean / 2;
		blobs [index].type = blobNamer (blobs [index].corners, false);
		return blobs [index];
	}

	public static float[] oneDMeanFilter (float[] inp)
	{
		for (int i = 1; i < inp.Length - 1; i++) {
			inp [i] = (inp [i - 1] + inp [i] + inp [i + 1]) / 3;
		}
		return inp; 
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
	public int tagNumber;
	public int area;
	public Vector2 CenterOfMass = new Vector2 (0, 0);
	public string type;
	public int corners;
	public int edgeArea;
}