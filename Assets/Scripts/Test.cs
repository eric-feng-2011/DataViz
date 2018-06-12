using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Accord;
using Accord.Math;
using Accord.Statistics.Analysis;

//This file was used for testing and debugging.
//It has no implications on the actual PCA computation and graphical rendering of the data
public class Test : MonoBehaviour {

	//The absolute path in the directory to where the data file is
	public string folderPath;
	//The name of the datafile
	public string inputfile;

	//The list that keeps track of columns to exclude from PCA and graphical rendering
	public List<int> excludeColumns;

	// Use this for initialization
	void Start () {
		writeFile ();
		checkTranspose ();
		Debug.Log ("Finished");
	}

	//Go the directory that is given and find the relevant file before importing it as a Unity asset
	void writeFile() {
		//Create the absolute name of the relevant file
		inputfile = folderPath + "/" + inputfile;
		foreach (string file in Directory.GetFiles(folderPath)) //Search in the directory
		{
			// If find relevant file, extract text, write it into a Unity Resource, and load the resource
			if (file == inputfile) {
				string contents = File.ReadAllText (file);

				//Create the file and the relative path to it from Unity root
				string path = "Assets/Resources/test.csv";

				//Write some text to the input.txt file
				StreamWriter writer = new StreamWriter(path, false);
				writer.WriteLine(contents);
				writer.Close();

				//Re-import the file to update the reference in the editor
				AssetDatabase.ImportAsset(path); 
				TextAsset asset = (TextAsset) Resources.Load("test");
			}
		}
	}

	//Ensure that what TransposeData.TransposeList creates is the transpose of the original data
	void checkTranspose() {
		List<Dictionary<string, object>> pointList = CSVReader.Read ("test");
		List<Dictionary<string, object>> pointListTranspose = TransposeData.TransposeList (pointList);
	}

	//Read a file in the given directory and reveal file name and contents
	void readFile() {
		foreach (string file in Directory.GetFiles(folderPath))
		{
			string contents = File.ReadAllText(file);
			Debug.Log ("A File Name: " + file);

			Debug.Log ("File contents: " + contents);
		}
	}

	//Ensure that Accord PCA works
	void pcaCheck() {
		double[][] input =
		{              // age, smokes?, had cancer?
			new double[] { 1,    2  }, // false - no cancer
			new double[] { 2,    4  }, // false
			new double[] { 3,    6  }, // false
			//		    new double[] { 4,    8  }, // true  - had cancer
			//		    new double[] { 5,    10  }, // true
			//		    new double[] { 6,    12  }, // true
			//		    new double[] { 7,    14  }, // false
			//		    new double[] { 8,    16  }, // false
			//		    new double[] { 9,    18  }, // false
			//		    new double[] { 10,    20  }, // true
		};

		//Create PCA object
		PrincipalComponentAnalysis pca = new PrincipalComponentAnalysis(input);

		//Computes N number of Principal components, each of dimension 2 (xy-plane)
		//N is the number of data points/entrys
		pca.Compute();
		double[,] components = pca.ComponentMatrix; //Obtains the principal components of data
		double[] eigValues = pca.Eigenvalues;	//Obtains the eigenvalues of the data

		//Ensure the number of principal components is correct
		Debug.Log("Number of principal components: " + components.GetLength(0));

		//Principal Components (eigenvectors) are in matrix vertically
		//In other words, numbers that share same column number are part of same vector
		//Print out pca and eigenvalues (singularity values) for extra confirmation
		for(int x = 0; x < components.GetLength(0); x++) {
			for(int y = 0; y < components.GetLength(1); y++) {
				Debug.Log("col: " + y + ", row: " + x + ": " + components[x, y]);
			}
		}
		for(int x = 0; x < eigValues.Length; x++) {
			Debug.Log("EigenValue " + x + ": " + eigValues[x]);
		}
	}
}
