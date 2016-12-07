using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class cameraScript : MonoBehaviour
{

	//private string savePath = "/Users/ejer/Desktop/Medialogy/3. Semester/P3/P3-3DMapGen/Vertexmodify/Assets/WebcamBilleder";
	private string savePath;
	public GameObject targetPlane;
	public RawImage rawimage;
	protected Color[] pixFromSnap;
	Texture2D originalImageFromWebcam;
	Texture2D flippedImageFromCamera;
	WebCamTexture webcamTexture;

	public Texture2D outputImage;
	string currentImageName;
	gameState gameState;

	public GameObject chooseButton, retakeButton, snapButton, calibrateButton;

	void Start ()
	{

		savePath = Application.dataPath + "/Resources/WebcamBilleder/";
		print (savePath);
		StartCoroutine (initWebcam ()); 

		chooseButton.SetActive (false);
		retakeButton.SetActive (false);
		calibrateButton.SetActive (false);
		snapButton.SetActive (true);

	}

	public void Snapshot ()
	{

		originalImageFromWebcam.SetPixels (webcamTexture.GetPixels ()); 
		originalImageFromWebcam.Apply ();
		targetPlane.GetComponent<Renderer> ().material.mainTexture = originalImageFromWebcam; 
		rawimage.enabled = false;

		currentImageName = System.DateTime.Now.ToString ("dd-MM-yyyy-HH-mm-ss") + ".png";

		flippedImageFromCamera = flipXAndY (originalImageFromWebcam);

		System.IO.File.WriteAllBytes (savePath + currentImageName, flippedImageFromCamera.EncodeToPNG ());

		originalImageFromWebcam = new Texture2D (webcamTexture.width, webcamTexture.height);

		chooseButton.SetActive (true);
		retakeButton.SetActive (true);
		//calibrateButton.SetActive (true);
		snapButton.SetActive (false);

	}


	public void retake ()
	{
		rawimage.enabled = true; 
		webcamTexture.Play ();
		chooseButton.SetActive (false);
		retakeButton.SetActive (false);
		snapButton.SetActive (true);
	}


	public void choose ()
	{
		gameState = GameObject.Find ("gameState").GetComponent<gameState> ();
		gameState.chosenImage = currentImageName;
		print ("image chosen: " + currentImageName);
		Application.LoadLevel ("sagen");
	}

	public Texture2D flipXAndY (Texture2D original)
	{
		TimingModule.timer ("flipX&Y", "start");
		Texture2D flipped = new Texture2D (original.height, original.height);

		int cropAreaOnEachSide = (original.width - original.height) / 2;

		int xN = original.width - 1 - cropAreaOnEachSide;
		int yN = original.height - 1;


		for (int i = cropAreaOnEachSide; i < xN; i++) {
			for (int j = 0; j < yN; j++) {
				flipped.SetPixel (yN - j, xN - i, original.GetPixel (i, j));

			}
		}
		flipped.Apply ();
		TimingModule.timer ("flipX&Y", "end");

		return flipped;
	}


	public void calibrate ()
	{
		gameState = GameObject.Find ("gameState").GetComponent<gameState> ();
		gameState.chosenImage = currentImageName;
		print ("image chosen: " + currentImageName);
		Application.LoadLevel ("calibrate");
	}


	IEnumerator initWebcam ()
	{
		webcamTexture = new WebCamTexture ();
		webcamTexture.Play ();
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
		//rawimage.material.mainTexture = webcamTexture;

		originalImageFromWebcam = new Texture2D (webcamTexture.width, webcamTexture.height);

		Debug.Log ("The height of the webcam is: " + webcamTexture.height + " The width of the webcam is: " + webcamTexture.width); 
	}
}
