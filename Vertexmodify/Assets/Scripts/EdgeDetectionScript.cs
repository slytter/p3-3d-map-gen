using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class EdgeDetectionScript : MonoBehaviour
{
	Texture2D originalIm;
	Texture2D newIm;	
	Color[] pixN;
	Color[] pixO;
	GameObject TxtBox;

	void Start ()
	{
		newIm = new Texture2D(512,512);
		originalIm = (Texture2D)GetComponent<Renderer> ().material.mainTexture;
		GetComponent<Renderer> ().material.mainTexture = newIm;

		pixN = originalIm.GetPixels ();
		pixO = originalIm.GetPixels ();
		newIm.SetPixels (pixN);
		newIm.Apply ();	

		TxtBox = GameObject.Find ("Txt");
	}

	void Update ()
	{

		if (Input.GetKeyDown (KeyCode.F1))
			resetImage ();
		if (Input.GetKeyDown (KeyCode.F2))
			medianFilter ();
		if (Input.GetKeyDown (KeyCode.F3))
			sobelHorisontal ();
		if (Input.GetKeyDown (KeyCode.F4))
			sobelVertical ();
		if (Input.GetKeyDown (KeyCode.F5))
			prewittHorisontalVertical ();
		if (Input.GetKeyDown (KeyCode.F6))
			gaussianEdge3x3 ();
	}

	void updateOldImAndApplyNewIm(){

		newIm.SetPixels (pixN);
		newIm.Apply ();	
		pixO = newIm.GetPixels ();
	}

	void resetImage ()
	{
		pixN = originalIm.GetPixels ();
		updateOldImAndApplyNewIm ();
	}

	void medianFilter()
	{
		//int[] kernel = {0, 1, 0, 1, 1, 1, 0, 1, 0};
		float[] sumR = new float[9];
		float[] sumG = new float[9];
		float[] sumB = new float[9];


		for (int y = 1; y < newIm.height-1; y++)
		{
			for (int x = 1; x < newIm.width-1; x++)
			{

				for (int ky = 0; ky < 2; ky++)
				{	
					for (int kx = 0; kx < 2; kx++)
					{
						sumR [ky * 3 + kx] = pixO [(y+ky-1) * newIm.width + (x + kx - 1)].r;
						sumG [ky * 3 + kx] = pixO [(y+ky-1) * newIm.width + (x + kx - 1)].g;
						sumB [ky * 3 + kx] = pixO [(y+ky-1) * newIm.width + (x + kx - 1)].b;
					}
					
				}

				Array.Sort (sumR);
				Array.Sort (sumG);
				Array.Sort (sumB);

				pixN [y * newIm.width + x].r = sumR [4];
				pixN [y * newIm.width + x].g = sumG [4];
				pixN [y * newIm.width + x].b = sumB [4];


			}
		}

		updateOldImAndApplyNewIm ();
	}

	void gaussianEdge3x3 ()
	{

		int[] kernel = {0,-1,0,-1,4,-1,0,-1,0};
		for (int y = 1; y < newIm.height-1; y++) {
			for (int x = 1; x < newIm.width-1; x++) {
				float sumR = 0.0f;
				float sumG = 0.0f;
				float sumB = 0.0f;
				for (int ky = 0; ky <= 2; ky++) {
					for (int kx = 0; kx <= 2; kx++) {

						sumR += pixO [(y+ky-1) * newIm.width + (x+kx-1)].r * kernel[ky*3+kx];
						sumG += pixO [(y+ky-1) * newIm.width + (x+kx-1)].g * kernel[ky*3+kx];
						sumB += pixO [(y+ky-1) * newIm.width + (x+kx-1)].b * kernel[ky*3+kx];
					}
				}
				pixN [y * newIm.width + x].r = sumR;
				pixN [y * newIm.width + x].g = sumG;
				pixN [y * newIm.width + x].b = sumB;
			}
		}
		updateOldImAndApplyNewIm ();
	}

	void sobelHorisontal ()
	{

		int[] kernel = {-1,-2,-1,0,0,0,1,2,1};
		for (int y = 1; y < newIm.height-1; y++) {
			for (int x = 1; x < newIm.width-1; x++) {
				float sumR = 0.0f;
				float sumG = 0.0f;
				float sumB = 0.0f;
				for (int ky = 0; ky <= 2; ky++) {
					for (int kx = 0; kx <= 2; kx++) {

						sumR += pixO [(y+ky-1) * newIm.width + (x+kx-1)].r * kernel[ky*3+kx];
						sumG += pixO [(y+ky-1) * newIm.width + (x+kx-1)].g * kernel[ky*3+kx];
						sumB += pixO [(y+ky-1) * newIm.width + (x+kx-1)].b * kernel[ky*3+kx];
					}
				}
				pixN [y * newIm.width + x].r = sumR;
				pixN [y * newIm.width + x].g = sumG;
				pixN [y * newIm.width + x].b = sumB;
			}
		}
		updateOldImAndApplyNewIm ();
	}

	void sobelVertical ()
	{

		int[] kernel = {-1,0,1,-2,0,2,-1,0,1};
		for (int y = 1; y < newIm.height-1; y++) {
			for (int x = 1; x < newIm.width-1; x++) {
				float sumR = 0.0f;
				float sumG = 0.0f;
				float sumB = 0.0f;
				for (int ky = 0; ky <= 2; ky++) {
					for (int kx = 0; kx <= 2; kx++) {

						sumR += pixO [(y+ky-1) * newIm.width + (x+kx-1)].r * kernel[ky*3+kx];
						sumG += pixO [(y+ky-1) * newIm.width + (x+kx-1)].g * kernel[ky*3+kx];
						sumB += pixO [(y+ky-1) * newIm.width + (x+kx-1)].b * kernel[ky*3+kx];
					}
				}
				pixN [y * newIm.width + x].r = sumR;
				pixN [y * newIm.width + x].g = sumG;
				pixN [y * newIm.width + x].b = sumB;
			}
		}
		updateOldImAndApplyNewIm ();
	}

	void prewittHorisontalVertical ()
	{

		int[] kernel = {-1,-1,-1,0,0,0,1,1,1};
		int[] kernel2 = {-1,0,1,-1,0,1,-1,0,1};


		for (int y = 1; y < newIm.height-1; y++) {
			for (int x = 1; x < newIm.width-1; x++) {

				float sumR = 0.0f;
				float sumG = 0.0f;
				float sumB = 0.0f;

				float sumR2 = 0.0f;
				float sumG2 = 0.0f;
				float sumB2 = 0.0f;

				for (int ky = 0; ky <= 2; ky++) {
					for (int kx = 0; kx <= 2; kx++) {

						sumR += pixO [(y+ky-1) * newIm.width + (x+kx-1)].r * kernel[ky*3+kx];
						sumG += pixO [(y+ky-1) * newIm.width + (x+kx-1)].g * kernel[ky*3+kx];
						sumB += pixO [(y+ky-1) * newIm.width + (x+kx-1)].b * kernel[ky*3+kx];

						sumR2 += pixO [(y+ky-1) * newIm.width + (x+kx-1)].r * kernel2[ky*3+kx];
						sumG2 += pixO [(y+ky-1) * newIm.width + (x+kx-1)].g * kernel2[ky*3+kx];
						sumB2 += pixO [(y+ky-1) * newIm.width + (x+kx-1)].b * kernel2[ky*3+kx];
					}
				}
				pixN [y * newIm.width + x].r = Mathf.Abs(sumR + sumR2);
				pixN [y * newIm.width + x].g = Mathf.Abs(sumG + sumG2);
				pixN [y * newIm.width + x].b = Mathf.Abs(sumB + sumB2);
			}
		}
		updateOldImAndApplyNewIm ();
	}	
}
