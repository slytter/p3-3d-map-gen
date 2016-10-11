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


	public float maxX; 
	public float minX; 
	public float maxZ; 
	public float minZ; 



	void Start () { 

		 

	}
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetKeyDown ("m")) {
			resetSpawnObject (); 

		}

	}


	void instantiatePrefab(GameObject objectToSpawn){
		// Trying to change the color of the material on each instantiate
		//objectToSpawn.GetComponent<Material>() = new Color (Random.Range (0, 2), Random.Range (0, 2), Random.Range (0, 2)); 
		spawnPosition = new Vector3 (Random.Range (maxX, minX), 1.0f, Random.Range (maxZ, minZ));
		Quaternion spawnRotation = Quaternion.identity; 
		Instantiate (objectToSpawn, spawnPosition, spawnRotation); 

	}
	void resetSpawnObject(){
		int determine; 
		determine = Random.Range (0,4); 
		Debug.Log (determine); 
		//prefabMat.color = new Color (Random.Range (0, 2), Random.Range (0, 2), Random.Range (0, 2)); 

		if (determine == 0) {
			instantiatePrefab (prefab); 
		} 
		else if (determine == 1) {
			instantiatePrefab (prefab2); 
		}
		else if (determine == 2) {
			instantiatePrefab (prefab3); 
		}
		else {
			instantiatePrefab (prefab4); 
		}
	}
}
