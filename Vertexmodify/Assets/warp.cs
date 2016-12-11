using UnityEngine;
using System.Collections;

public class warp : MonoBehaviour {
	GameObject cam;
	public Vector3 Transformer;
	// Use this for initialization
	void Start () {
		transform.localScale += Transformer;
		cam = this.gameObject;
	}

	// Update is called once per frame
	void Update () {
		Vector3 pos = cam.transform.localScale;
		//cam.transform.localScale.Set (pos.x + 1, pos.y, pos.z);
		transform.localScale += Transformer;
	//	transform.position.Set(pos.x + 1f, pos.y, pos.z);
	}
}
