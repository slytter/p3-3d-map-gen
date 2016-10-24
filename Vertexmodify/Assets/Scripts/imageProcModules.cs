using UnityEngine;
using System.Collections;

public class imageProcModules : MonoBehaviour {


	/// <summary>
	/// Creates random value 2D array.
	/// </summary>
	/// <returns>The value gen.</returns>
	/// <param name="height">Height.</param>
	/// <param name="length">Length.</param>
	public float[,] randomValGen(int height, int length){
		float[,] randomValues = new float[height, length];
		for (int i = 0; i < randomValues.GetLength (1); i++) {
			for (int j = 0; j < randomValues.GetLength (0); j++) {
				randomValues [i,j] = UnityEngine.Random.Range (0f, 1f);
			}
		}
		return randomValues;
	}



	/// <summary>
	/// Gaussian the specified 2D Array by using smoothing.
	/// </summary>
	/// <param name="heightMap">2D float height map.</param>
	/// <param name="smoothing">Number of iterations.</param>
	public float[,] gaussian(float[,] heightMap, int smoothing){
		for (int k = 0; k < smoothing; k++) {
			for (int i = 1; i < heightMap.GetLength (1)-1; i++) {
				for (int j = 1; j < heightMap.GetLength (0)-1; j++) {
					float blur = (
						heightMap [i, j] 

						+ heightMap [i + 1, j + 1] 		* 2 
						+ heightMap [i + 1, j] 			* 2
						+ heightMap [i, j + 1] 			* 2

						+ heightMap [i - 1, j - 1] 		* 2  
						+ heightMap [i - 1, j] 			* 2
						+ heightMap [i, j - 1] 			* 2

						+ heightMap [i + 1, j - 1] 		* 2
						+ heightMap [i - 1, j + 1] 		* 2
					) 
						/ (9 + 8);
					heightMap [i, j] = blur;	
				}
			}
		}
		return heightMap;
	}
}
