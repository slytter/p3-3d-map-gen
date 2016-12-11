using UnityEngine;
using System.Collections;

public class camScript : MonoBehaviour {
	GameObject cam;
	public Vector3 Transformer;
	// Use this for initialization
	void Start () {
		cam = this.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = cam.transform.position;
		cam.transform.position.Set(pos.x + 1f, pos.y, pos.z);
		transform.Translate (Transformer);
	}
}
