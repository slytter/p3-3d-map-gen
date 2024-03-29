﻿using UnityEngine;
using System.Collections;
using System;

public class MountainGeneration : MonoBehaviour
{
	imageProcModules modules;

	void Awake ()
	{
		modules = GetComponent<imageProcModules> ();
	}

	/// <summary>
	/// midpointDisplacement, taking the current recursion, heightmap..
	/// </summary>
	/// <returns>The recursion.</returns>
	/// <param name="recursion">Recursion.</param>
	/// <param name="heightMap">heightMap.</param>
	/// <param name="amount">Amount.</param>
	public float[,] midpointDisplacement (int recursion, float[,] heightMap, float amount, int gaussianAmount)
	{

		// modules = GetComponent<imageProcModules>();
		// float amount goes from 0 to 1.
		// Debug.Log("Millis for iteration " + recursion + ": "  + ((Time.realtimeSinceStartup-lastmillis)*1000));
		// lastmillis = Time.realtimeSinceStartup; // Calculate the number of milliseconds since midnight
		float[,] randomValues = modules.randomValGen (heightMap.GetLength (0), heightMap.GetLength (1));
		int splitLength;
		float currentTerrainHeight;
		splitLength = heightMap.GetLength (1) / recursion;

		int randomArrayPointerX = 0;
		int randomArrayPointerY = 0;

		for (int j = 0; j < heightMap.GetLength (1) - 1; j++) {
			if (j % splitLength == 0)
				randomArrayPointerY = j / splitLength; //same here
			for (int i = 0; i < heightMap.GetLength (0) - 1; i++) {
				if (i % splitLength == 0) {
					if (i == 0)
						randomArrayPointerX = 0;
					randomArrayPointerX = i / splitLength; 
				}
				currentTerrainHeight = randomValues [randomArrayPointerX, randomArrayPointerY];

				if (amount == 1) {
					heightMap [i, j] = Math.Abs (currentTerrainHeight);
				} else {
					heightMap [i, j] += Math.Abs (amount / 2 - currentTerrainHeight * amount);
				}
					
			}
		}
		heightMap = modules.gaussian (heightMap, gaussianAmount);
		return heightMap;
	}


	/// <summary>
	/// Removes moutains where they should not be.
	/// </summary>
	/// <returns>Clean heightmap.</returns>
	/// <param name="heightMap">Height map.</param>
	/// <param name="whereMoutainsShouldBe">Where moutains should be.</param>
	public float[,] mountainRemove (float[,] heightMap, float[,] mountainArea, float mountainHeight)
	{
		for (int y = 0; y < heightMap.GetLength (1); y++) {
			for (int x = 0; x < heightMap.GetLength (0); x++) {
				heightMap [x, y] = heightMap [x, y] * (mountainArea [x, y]) * mountainHeight;
			}
		}
		return heightMap;
	}


	/// <summary>
	/// Limits the map to go from 0-1, and applies master smoothing.
	/// </summary>
	/// <returns>Final heightmap.</returns>
	/// <param name="heightMap">Height map.</param>
	public float[,] finalizeMap (float[,] heightMap, int smoothing)
	{
		float max_val = 0f;
		for (int j = 0; j < heightMap.GetLength (1); j++) {
			for (int i = 0; i < heightMap.GetLength (0); i++) {
				if (heightMap [i, j] > max_val) {
					max_val = heightMap [i, j];
				}
			}
		}

		heightMap = modules.gaussian (heightMap, smoothing);
		for (int j = 0; j < heightMap.GetLength (1); j++) {
			for (int i = 0; i < heightMap.GetLength (0); i++) {
				heightMap [i, j] = heightMap [i, j] / max_val; //global tissemand
			}
		}
		return heightMap;
	}
	
}
