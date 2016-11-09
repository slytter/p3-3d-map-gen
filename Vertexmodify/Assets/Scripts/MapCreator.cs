using UnityEngine;
using System.Collections;
using System;

public class MapCreator : MonoBehaviour
{
    imageProcModules modules;
    ColorDetection colorScanScript;
    MountainGeneration mg;
    Terrain currentTerrain;
	float update;
    int frame = 0;
	float[,] drawMap, myHeightMap; 
	public int heightOfMap = 100;
	public float [,] newHeightMap;
	public float[,] river;

	public TreeInstance tree;

    void Start() {

		//INITIALIZING: 	////////////
		modules = GetComponent<imageProcModules> ();
		colorScanScript = GameObject.Find ("colorScan").GetComponent<ColorDetection> ();
		mg = GetComponent<MountainGeneration> ();

		Debug.Log (colorScanScript.widthOfTex + ", " + colorScanScript.heightOfTex);

		//fixing texture scale issue:
		myHeightMap = new float[colorScanScript.widthOfTex, colorScanScript.heightOfTex];
		drawMap = new float[colorScanScript.widthOfTex, colorScanScript.heightOfTex];

		float startit = Time.realtimeSinceStartup; //starting milli counter

		currentTerrain = Terrain.activeTerrain; // getting terrain data
		int biggestDimention = (colorScanScript.heightOfTex > colorScanScript.widthOfTex) ? colorScanScript.heightOfTex : colorScanScript.widthOfTex; //Simple if statement 
		currentTerrain.terrainData.size = new Vector3(biggestDimention, heightOfMap, biggestDimention); //setting size



		bool[,] yellow = colorScanScript.colorDetection(colorScanScript.originalImage, 0.15f, 0.23f, 0.20f, 0.5f); // getting colors from input image
		bool[,] red = colorScanScript.colorDetection(colorScanScript.originalImage, 0.94f, 0.02f, 0.25f, 0.5f); // getting colors from input image
		bool[,] green = colorScanScript.colorDetection(colorScanScript.originalImage, 0.24f, 0.42f, 0.20f, 0.5f); // getting colors from input image



		float[,] mountains = generateMountains (yellow);
		colorScanScript.printBinary (mountains);
		currentTerrain.terrainData.SetHeights (0, 0, mountains);

        Debug.Log("Total millis for all recursions: " + ((Time.realtimeSinceStartup - startit) * 1000));
    }



	float[,] generateMountains(bool[,] area){
		area = modules.dilation (area);
		area = modules.floodFill (area);


		float[,] mountainArea = new float[area.GetLength(0),area.GetLength(1)];
		mountainArea = modules.boolToFloat (area);
		mountainArea = modules.gaussian (mountainArea, 50);


		float[,] randomMountains = new float[area.GetLength(0),area.GetLength(1)];
        randomMountains = mg.midpointDisplacement(3, randomMountains, 0.5f, 0);
        randomMountains = mg.midpointDisplacement(8, randomMountains, 0.5f, 0);
        randomMountains = mg.midpointDisplacement(16, randomMountains, 0.4f, 0);
        randomMountains = mg.midpointDisplacement(32, randomMountains, 0.3f, 0);
        randomMountains = mg.midpointDisplacement(64, randomMountains, 0.3f, 0);
		randomMountains = mg.midpointDisplacement(128, randomMountains, 0.2f, 0);
		randomMountains = modules.gaussian (randomMountains, 10);


		return mg.mountainRemove(randomMountains, mountainArea);
	}










    bool stop = false;

    void Update() {
        if (!stop){
            if (frame % 2 == 0) {
                for (int i = 0; i < drawMap.GetLength(0); i++) {
                    for (int j = 0; j < drawMap.GetLength(1); j++) {
                        drawMap[i, j] = myHeightMap[i, j] * update * 0.5f;
                    }
                }
                currentTerrain.terrainData.SetHeights(0, 0, drawMap);
            }
            frame++;
			if (update < 1f) {
				update += 0.01f;
			} else {
				print(drawMap.GetLength (0) + ", " + drawMap.GetLength (0));
				stop = true;
			}
        }
    }

}
