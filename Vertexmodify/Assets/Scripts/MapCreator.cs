using UnityEngine;
using System.Collections;
using System;



public class MapCreator : MonoBehaviour
{
	public GameObject tree1; 
    imageProcModules modules;
    ColorDetection colorScanScript;
    MountainGeneration mg;
    Terrain currentTerrain;
	float update;
    int frame = 0;
	float[,] drawMap, myHeightMap; 
	public int heightOfMap = 100;

	public TreeInstance tree;
	public TreePrototype TheTree; 

    void Start() {

		//INITIALIZING: 	////////////
		modules = GetComponent<imageProcModules> ();
		colorScanScript = GameObject.Find ("colorScan").GetComponent<ColorDetection> ();
		mg = GetComponent<MountainGeneration> ();

		Debug.Log (colorScanScript.widthOfTex + ", " + colorScanScript.heightOfTex);

		myHeightMap = new float[colorScanScript.widthOfTex, colorScanScript.heightOfTex];
		drawMap = new float[colorScanScript.widthOfTex, colorScanScript.heightOfTex];
		float startit = Time.realtimeSinceStartup; //starting milli counter:




		//GENERATION: 		////////////
		bool[,] inputColorImage = colorScanScript.colorDetection(colorScanScript.originalImage, 0.20f, 0.15f, 0.57f, 0.5f); // getting colors from input image

		currentTerrain = Terrain.activeTerrain; // getting terrain data
		//fixing texturescale issue:
		int biggestDimention = (colorScanScript.heightOfTex > colorScanScript.widthOfTex) ? colorScanScript.heightOfTex : colorScanScript.widthOfTex; //Simple if statement 
		currentTerrain.terrainData.size = new Vector3(biggestDimention, heightOfMap, biggestDimention); //setting size

        inputColorImage = modules.dilation(inputColorImage);
        inputColorImage = modules.floodFill(inputColorImage);
		colorScanScript.printBinary(inputColorImage); //printing to plane
		myHeightMap = modules.boolToFloat(inputColorImage);//float convertion


		int[,] treePositions1 = modules.generateTrees (myHeightMap);


		generateTrees(treePositions1); 



		//MOUNTAINS: 		////////////
		myHeightMap = modules.gaussian(myHeightMap, 200); //gauss
		colorScanScript.printBinary(myHeightMap); //printing to plane
        myHeightMap = mg.midpointDisplacement(3, myHeightMap, 1.0f, 0);
        myHeightMap = mg.midpointDisplacement(8, myHeightMap, 1.0f, 0);
        myHeightMap = mg.midpointDisplacement(16, myHeightMap, 1.0f, 0);
        myHeightMap = mg.midpointDisplacement(32, myHeightMap, 0.5f, 0);
        myHeightMap = mg.midpointDisplacement(64, myHeightMap, 0.5f, 0);
        myHeightMap = mg.midpointDisplacement(128, myHeightMap, 0.5f, 0);

        myHeightMap = mg.finalMap(mg.mountainRemove(myHeightMap, modules.boolToFloat(inputColorImage)), 5);
		myHeightMap = modules.perlin(myHeightMap); 

        Debug.Log("Total millis for all recursions: " + ((Time.realtimeSinceStartup - startit) * 1000));

		currentTerrain.terrainData.SetHeights(0, 0, myHeightMap);


	
    }

	void generateTrees(int[,] treePositions){
		for (int x = 0; x < treePositions.GetLength(1); x++) {

			RaycastHit rcHit = new RaycastHit (); 
			Ray theRay = new Ray (new Vector3(treePositions[0,x], 100, treePositions[1,x]), Vector3.down);

			if (Physics.Raycast (theRay, out rcHit, 10000)) {

				float groundDist = rcHit.distance; 
				GameObject.Instantiate (tree1, new Vector3 (treePositions [0, x], (100 - groundDist), treePositions [1, x]), Quaternion.identity); 
			}
		}
	}


    bool stop = false;

    void Update() {
		if (!stop && false){
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
