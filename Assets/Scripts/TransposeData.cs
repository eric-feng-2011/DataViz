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

		//The transpose of pointList that will be returned laster
		List<Dictionary<string, object>> transpose = new List<Dictionary<string, object>> ();

		//Get the keys from the pointList
		List<string> keys = new List<string> (pointList [1].Keys);

		//Get the number of rows in the future transpose matrix
		int dataLength = pointList [1].Count;

		//Create the 2D array that will hold the transpose values
		object[, ] colHeadersTranspose = new object[dataLength, pointList.Count + 1];

		//Iterate through the original pointList and set values in the transpose array
		for (int i = 0; i < pointList.Count; i++) {
			for (int j = 0; j < dataLength; j++) {
				object value = pointList [i] [keys [j]];
				colHeadersTranspose[j, i + 1] = value;
			}
		}
			
		//Place original keys in the first column of the colHeadersTranspose
		for (int i = 0; i < keys.Count; i++) {
			colHeadersTranspose [i, 0] = keys [i];
		}

		//Switch back to a list of dictionaries. Only now, the data is transposed

		//Keep track of the new column headers
		string[] headers = new string[pointList.Count + 1];

		//Fill in the headers with the appropriate values from the first row of data
		for (int i = 0; i < headers.Length; i++) {
			headers [i] = (string) colHeadersTranspose [0, i];
		}

		//Iterate through the rest of the transposed data, creating dictionaries according to 
		//the previous headers/keys. 
		for (int i = 1; i < dataLength; i++) {

			Dictionary<string, object> entry = new Dictionary<string, object> ();

			for (int j = 0; j < pointList.Count + 1; j++) {
				//Add key/value pair to dictionary
				entry.Add(headers[j], colHeadersTranspose[i, j]);
			}

			//Add dictionary to list of dictionaries
			transpose.Add (entry);
		}

		//Return list of dictionaries
		return transpose;
	}
}
