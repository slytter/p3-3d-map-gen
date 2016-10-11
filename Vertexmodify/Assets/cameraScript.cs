using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class cameraScript : MonoBehaviour {

    public RawImage rawimage;
    public Color[,] newImage;
    public texture

}

    void Start () {
        WebCamTexture webcamTexture = new WebCamTexture();
        rawimage.texture = webcamTexture;
        rawimage.material.mainTexture = webcamTexture;
        webcamTexture.Play();
        
        
    }
	
	
	void Update () {
	
	}

    void Snapshot()
{
        new

}


}
