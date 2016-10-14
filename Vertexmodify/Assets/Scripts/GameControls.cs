using UnityEngine;
using System.Collections;


public class GameControls : MonoBehaviour {


	Vector3 spawnPosition; 

	[SerializeField]
	GameObject prefab;
	[SerializeField]
	GameObject prefab2; 
	[SerializeField]
	GameObject prefab3; 
	[SerializeField]
	GameObject prefab4; 


	GameObject spawnObject; 

	public Material prefabMat; 



    public int [,] treePosition; 




	void Start () {

        treePosition =  new int [,] { { 0, 512 }, { 0  , 512 } } ;
        resetSpawnObject(treePosition);
    }
	
	// Update is called once per frame
	void Update () {

	}


	void instantiatePrefab(GameObject objectToSpawn, int xPos, int zPos){
		// Trying to change the color of the material on each instantiate
		//objectToSpawn.GetComponent<Material>() = new Color (Random.Range (0, 2), Random.Range (0, 2), Random.Range (0, 2)); 
		//spawnPosition = new Vector3 (Random.Range (maxX, minX), 1.0f, Random.Range (maxZ, minZ));
        spawnPosition = new Vector3(xPos, 0.0f, zPos);

        Quaternion spawnRotation = Quaternion.identity; 
		Instantiate (objectToSpawn, spawnPosition, spawnRotation); 

	}
	void resetSpawnObject(int [,] treePositions){


        //prefabMat.color = new Color (Random.Range (0, 2), Random.Range (0, 2), Random.Range (0, 2)); 


        for (int i = 0; i < treePositions.GetLength(1); i++)
        {

            //int determine;
            //determine = 0;
            //Debug.Log(determine);

            instantiatePrefab(prefab, treePositions[i, 0], treePositions[i,1]);

           // if (determine == 0) {
		   // instantiatePrefab (prefab); 
		

		/*else if (determine == 1) {
			instantiatePrefab (prefab2); 
		}
		else if (determine == 2) {
			instantiatePrefab (prefab3); 
		}
		else {
			instantiatePrefab (prefab4); 
		}*/

        }
	}
}
