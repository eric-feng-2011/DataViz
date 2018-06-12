using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accord.Math;
using System;

public class TransposeData : MonoBehaviour {

	//Method to transpose the read input file
	/*
	 * Done in this manner:
	 * 1. Put everything in a 2D array, transpose wise
	 * 2. Switch back to a list of dictionaries
	 * 
	*/
	public static List<Dictionary<string, object>> TransposeList(List<Dictionary<string, object>> pointList) {

		List<Dictionary<string, object>> transpose = new List<Dictionary<string, object>> ();

		//Get the keys from the 
		List<string> keys = new List<string> (pointList [1].Keys);

		int dataLength = pointList [1].Count;

		object[, ] colHeadersTranspose = new object[dataLength, pointList.Count + 1];

		for (int i = 0; i < pointList.Count; i++) {
			for (int j = 0; j < dataLength; j++) {
				object value = pointList [i] [keys [j]];
				colHeadersTranspose[j, i + 1] = value;
			}
		}

		//Place keys in the first column of the colHeadersTranspose
		for (int i = 0; i < keys.Count; i++) {
			colHeadersTranspose [i, 0] = keys [i];
		}

		//Switch back to a list of dictionaries. Only now, the data is transposed
		string[] headers = new string[pointList.Count + 1];

		//Fill in the headers with the appropriate values from the first row of data
		for (int i = 0; i < headers.Length; i++) {
			headers [i] = (string) colHeadersTranspose [0, i];
		}

		for (int i = 1; i < dataLength; i++) {

			Dictionary<string, object> entry = new Dictionary<string, object> ();

			for (int j = 0; j < pointList.Count + 1; j++) {
				try {
					entry.Add(headers[j], colHeadersTranspose[i, j]);
				} catch (Exception e) {
					Debug.Log (headers[j]);
					Debug.Log (colHeadersTranspose[i,j]);
				}
			}

			transpose.Add (entry);
		}

		return transpose;
	}
}
