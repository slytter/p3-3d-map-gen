using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class cameraScript : MonoBehaviour {

	public GameObject targetPlane; 

    public RawImage rawimage;
    //public Color[] newImage;
	Texture2D tex1; 
	WebCamTexture webcamTexture; 



	void Start () {
		webcamTexture = new WebCamTexture();
		tex1 = new Texture2D (webcamTexture.width,webcamTexture.height);
        rawimage.texture = webcamTexture;
        rawimage.material.mainTexture = webcamTexture;
        webcamTexture.Play();

        
        
    }
	
	
	void Update () {
	
		if (Input.GetKeyDown ("s")) {
			Snapshot (); 
		}

	}

    void Snapshot()
{
		//newImage = webcamTexture.GetPixels(); 
		tex1.SetPixels (webcamTexture.GetPixels()); 
		tex1.Apply (); 
		targetPlane.GetComponent<Renderer> ().material.mainTexture = tex1; 
}



}