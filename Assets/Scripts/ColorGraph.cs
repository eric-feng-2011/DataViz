using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGraph : MonoBehaviour {

	private static Dictionary<string, Color> colorMap = new Dictionary<string, Color>();
	private static Dictionary<string, Texture2D> solidImages = new Dictionary<string, Texture2D> ();

	private static List<Dictionary<string, object>> pointList = new List<Dictionary<string, object>>();
	private static List<string> columnList = new List<string> ();

	public static void createColor(List<Dictionary<string, object>> dataList, int catColumn) {

		colorMap.Clear ();
		solidImages.Clear ();

		pointList = dataList;

		columnList = new List<string>(pointList[1].Keys);

		createColorMap (catColumn);
		determineSolids ();
	}

	public static Dictionary<string, Color> getColorMap() {
		return colorMap;
	}

	public static Dictionary<string, Texture2D> getSolidImages() {
		return solidImages;
	}

	//Create the colorMap that contains unique, random colors for each species
	private static void createColorMap(int categoryColumn) {

		int numUniq = 0;
		HashSet<string> trackSpecies = new HashSet<string> ();

		for (int i = 0; i < pointList.Count; i++) {
			string species = Convert.ToString (pointList [i] [columnList [categoryColumn]]);
			//Debug.Log ("Species of point " + i + ": " + species);

			if (!trackSpecies.Contains (species)) {
				trackSpecies.Add (species);
				numUniq += 1;
			}
		}

		float step = 1f/numUniq;
		//Debug.Log (numUniq);

		foreach (string species in trackSpecies) {

			float randR = UnityEngine.Random.Range (0, numUniq);
			//Debug.Log ("RandR: " + randR);
			float randG = UnityEngine.Random.Range (0, numUniq);
			//Debug.Log ("RandG: " + randG);
			float randB = UnityEngine.Random.Range (0, numUniq);
			//Debug.Log ("RandB: " + randB);

			colorMap.Add(species, new Color (UnityEngine.Random.Range (randR * step, (randR + 1) * step), 
				UnityEngine.Random.Range (randG * step, (randG + 1) * step),
				UnityEngine.Random.Range (randB * step, (randB + 1) * step)));
		}			
	}

	//Create the solid textures that will be used for the graph legend
	private static void determineSolids() {
		List<string> keys = new List<string>(colorMap.Keys);
		for (int i = 0; i < keys.Count; i++) {
			Texture2D solidImage = new Texture2D (20, 20);
			string key = keys [i];
			Color[] legendColor = new Color[400];
			for (int j = 0; j < 400; j++) {
				legendColor [j] = colorMap [key];
			}
			solidImage.SetPixels (legendColor);
			solidImage.Apply ();

			solidImages.Add (key, solidImage);
		}
	}
}
