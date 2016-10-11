using UnityEngine;
using System.Collections;

public class ScaleController : MonoBehaviour {

	Vector3 scale; 
	void Start () {
		scale = new Vector3 (Random.Range (0.5f, 5f), Random.Range (0.5f, 5f), Random.Range (0.5f, 5f)); 
			transform.localScale = scale; 
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
