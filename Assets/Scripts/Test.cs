using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Accord;
using Accord.Math;
using Accord.Statistics.Analysis;

public class Test : MonoBehaviour {

	public string folderPath;
	public string inputfile;
	public List<int> excludeColumns;

	// Use this for initialization
	void Start () {
		writeFile ();
		checkTranspose ();
		Debug.Log ("Finished");
	}

	void writeFile() {
		//Debug.Log (directory);
		inputfile = folderPath + "/" + inputfile;
		//Debug.Log (inputfile);
		foreach (string file in Directory.GetFiles(folderPath))
		{
			//Debug.Log (file);
			if (file == inputfile) {
				string contents = File.ReadAllText (file);

				string path = "Assets/Resources/test.csv";

				//Write some text to the input.txt file
				StreamWriter writer = new StreamWriter(path, false);
				writer.WriteLine(contents);
				writer.Close();

				//Re-import the file to update the reference in the editor
				AssetDatabase.ImportAsset(path); 
				TextAsset asset = (TextAsset) Resources.Load("test");

				//Debug.Log (asset.text);
			}
		}
	}

	void checkTranspose() {
		List<Dictionary<string, object>> pointList = CSVReader.Read ("test");
		List<Dictionary<string, object>> pointListTranspose = TransposeData.TransposeList (pointList);
//		Debug.Log ("Number of rows" + pointList.Count);
//		Debug.Log ("Number of columns" + pointList [0].Count);
//		Debug.Log ("Number of rows in transpose: " + pointListTranspose.Count);
//		Debug.Log ("Number of cols in transpose: " + pointListTranspose [0].Count);
		//double[][] testInput = convertTo2D (pointList);
	}

	void readFile() {
		foreach (string file in Directory.GetFiles(folderPath))
		{
			string contents = File.ReadAllText(file);
			Debug.Log ("A File Name: " + file);

			Debug.Log ("File contents: " + contents);
		}
	}

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

		PrincipalComponentAnalysis pca = new PrincipalComponentAnalysis(input);

		//Computes N number of Principal components, each of dimension 2 (xy-plane)
		//N is the number of data points/entrys
		pca.Compute();
		double[,] components = pca.ComponentMatrix; //Obtains the principal components of data
		double[] eigValues = pca.Eigenvalues;	//Obtains the eigenvalues of the data
		//double[][] components = pca.Transform(input, 2);

		Debug.Log("Number of principal components: " + components.GetLength(0));

		//Principal Components (eigenvectors) are in matrix vertically
		//In other words, numbers that share same column number are part of same vector
		for(int x = 0; x < components.GetLength(0); x++) {
			for(int y = 0; y < components.GetLength(1); y++) {
				Debug.Log("col: " + y + ", row: " + x + ": " + components[x, y]);
			}
		}
		for(int x = 0; x < eigValues.Length; x++) {
			Debug.Log("EigenValue " + x + ": " + eigValues[x]);
		}
		Debug.Log("Finished");
	}
}
