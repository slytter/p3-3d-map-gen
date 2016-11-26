using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class cameraScript : MonoBehaviour {

    //private string savePath = "/Users/ejer/Desktop/Medialogy/3. Semester/P3/P3-3DMapGen/Vertexmodify/Assets/WebcamBilleder"; 
    private string savePath;
    public GameObject targetPlane; 
    public RawImage rawimage;
	protected Color [] pixFromSnap; 
	Texture2D tex1; 
	WebCamTexture webcamTexture; 

	public Texture2D outputImage;
    string currentImageName;


	void Start () {

		savePath = Application.dataPath + "/Resources/WebcamBilleder/";
        print(savePath);
		StartCoroutine (fixBug ());  //AD HVOR KLAMT 
		 
    }
	

	void Update () {

	}

	public void Snapshot() {

		tex1.SetPixels(webcamTexture.GetPixels()); 
		tex1.Apply ();
		targetPlane.GetComponent<Renderer> ().material.mainTexture = tex1; 
		rawimage.enabled = false;

		currentImageName = System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") + ".png";

        System.IO.File.WriteAllBytes(savePath + currentImageName, tex1.EncodeToPNG());

	    
		pixFromSnap = tex1.GetPixels (); 
		outputImage = tex1;
		tex1 = new Texture2D (webcamTexture.width,webcamTexture.height);
	}


	public void retake(){

		rawimage.enabled = true; 
		webcamTexture.Play ();

    }


    public void choose() {
		
		//(GameObject.Find("gameState").GetComponent<gameState>()).image = (outputImage);
		//Array.Copy(outputImage, gameState.image)
		//gameState.image = outputImage;
       // gameState.chosenImage = currentImageName;
       // print("image chosen: " + currentImageName);
        Application.LoadLevel("sagen");
    }


	IEnumerator fixBug(){
		webcamTexture = new WebCamTexture();
		webcamTexture.Play();

		if (webcamTexture.width <= 16 || webcamTexture.height <= 16) {		
			while (!webcamTexture.didUpdateThisFrame) {
				yield return new WaitForEndOfFrame (); 
			}

			webcamTexture.Pause (); 
			Color32[] colors = webcamTexture.GetPixels32 (); 
			webcamTexture.Stop (); 

			yield return new WaitForEndOfFrame ();
			webcamTexture.Play (); 


		}
			
		rawimage.texture = webcamTexture;
		rawimage.material.mainTexture = webcamTexture;

		tex1 = new Texture2D (webcamTexture.width,webcamTexture.height);

		Debug.Log ("The height of the webcam is: " + webcamTexture.height + " The width of the webcam is: " + webcamTexture.width); 
	}
}