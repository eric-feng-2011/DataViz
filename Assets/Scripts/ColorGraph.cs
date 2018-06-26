using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorGraph : MonoBehaviour {

	//Private variables to keep track of the color maps 
	//and also allow for other script access through get methods
	private static Dictionary<string, Color> colorMap = new Dictionary<string, Color>();

	//Variable holders to keep track of arguments passed in so that other static functions can use them
	private static List<Dictionary<string, object>> pointList = new List<Dictionary<string, object>>();
	private static List<string> columnList = new List<string> ();

	//Create color according to the data and the column with the categories
	public static void createColor(List<Dictionary<string, object>> dataList, int catColumn) {

		//Ensure that past colors aren't getting in the way or being reapplied
		colorMap.Clear ();

		//Create references to data from input
		pointList = dataList;

		columnList = new List<string>(pointList[1].Keys);

		//Run these methods to create the colorMap and textureMap
		createColorMap (catColumn);
	}

	//Return the colorMap
	public static Dictionary<string, Color> getColorMap() {
		return colorMap;
	}

	//Create the colorMap that contains unique, random colors for each species
	private static void createColorMap(int categoryColumn) {

		int numUniq = 0;
		HashSet<string> trackSpecies = new HashSet<string> ();

		//First determine how many different categories there are
		for (int i = 0; i < pointList.Count; i++) {
			string species = Convert.ToString (pointList [i] [columnList [categoryColumn]]);

			if (!trackSpecies.Contains (species)) {
				trackSpecies.Add (species);
				numUniq += 1;
			}
		}

		//The step with which to create color buckets
		float step = 1f/numUniq;

		//Create random colors for each type in trackSpecies
		foreach (string species in trackSpecies) {

			float randR = UnityEngine.Random.Range (0, numUniq);
			float randG = UnityEngine.Random.Range (0, numUniq);
			float randB = UnityEngine.Random.Range (0, numUniq);

			//Add the random color that maps to some species into the colorMap dictionary
			colorMap.Add(species, new Color (UnityEngine.Random.Range (randR * step, (randR + 1) * step), 
				UnityEngine.Random.Range (randG * step, (randG + 1) * step),
				UnityEngine.Random.Range (randB * step, (randB + 1) * step)));
		}			
	}
}
