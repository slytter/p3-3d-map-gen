using UnityEngine;
using System.Collections;
using System;

public class MapCreator : MonoBehaviour
{
    imageProcModules modules;
    ColorDetection colorScanScript;
    MountainGeneration mg;
    int length = 512;
    int height = 512;
    Terrain currentTerrain;
    float update = 0;
    int frame = 0;
    float[,] drawMap = new float[512, 512];
    float[,] myHeightMap = new float[512, 512];
    float lastmillis;


    void Start() {


        modules = GetComponent<imageProcModules>();
        colorScanScript = GameObject.Find("colorScan").GetComponent<ColorDetection>();
        mg = GetComponent<MountainGeneration>();

        float lastmillis = Time.realtimeSinceStartup;
        float startit = Time.realtimeSinceStartup;

        //bool [,] inputColorImage = colorScanScript.colorDetection (colorScanScript.originalImage, 0.23f, 0.15f, 0.25f, 0.5f);

		bool[,] inputColorImage = colorScanScript.colorDetection(colorScanScript.originalImage, 0.20f, 0.15f, 0.57f, 0.5f); // getting colors from input image
        currentTerrain = Terrain.activeTerrain; // getting terrain data


        inputColorImage = modules.dilation(inputColorImage);
        inputColorImage = modules.floodFill(inputColorImage);
        colorScanScript.printBinary(inputColorImage);
        //float convertion
        myHeightMap = modules.boolToFloat(inputColorImage); // my heightmap = float
        myHeightMap = modules.gaussian(myHeightMap, 4); //gauss

        myHeightMap = mg.midpointDisplacement(3, myHeightMap, 1.8f, 10);
        myHeightMap = mg.midpointDisplacement(8, myHeightMap, 1.8f, 10);
        myHeightMap = mg.midpointDisplacement(16, myHeightMap, 1.6f, 0);
        myHeightMap = mg.midpointDisplacement(32, myHeightMap, 1.6f, 0);
        myHeightMap = mg.midpointDisplacement(64, myHeightMap, 1.6f, 0);
        myHeightMap = mg.midpointDisplacement(128, myHeightMap, 0.5f, 0);


        myHeightMap = mg.finalMap(mg.mountainRemove(myHeightMap, modules.boolToFloat(inputColorImage)), 5);

        Debug.Log("Total millis for all recursions: " + ((Time.realtimeSinceStartup - startit) * 1000));


        currentTerrain.terrainData.SetHeights(0, 0, myHeightMap);

    }




    bool stop = false;

    /// <summary>
    /// Update this instance.
    /// </summary>
    void Update()
    {
        if (!stop)
        {
            if (frame % 2 == 0)
            {	
                for (int i = 0; i < drawMap.GetLength(1); i++)
                {
                    for (int j = 0; j < drawMap.GetLength(0); j++)
                    {
                        drawMap[i, j] = myHeightMap[i, j] * update * 0.5f;
                    }
                }
                currentTerrain.terrainData.SetHeights(0, 0, drawMap);
            }
            frame++;
            if (update < 1f)
                update += 0.01f;
            else
                stop = true;
        }
    }

}
