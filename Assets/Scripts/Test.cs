using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accord;
using Accord.Math;
using Accord.Statistics.Analysis;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
    	double[][] input =
		{              // age, smokes?, had cancer?
		    new double[] { 55,    0  }, // false - no cancer
		    new double[] { 28,    0  }, // false
		    new double[] { 65,    1  }, // false
		    new double[] { 46,    0  }, // true  - had cancer
		    new double[] { 86,    1  }, // true
		    new double[] { 56,    1  }, // true
		    new double[] { 85,    0  }, // false
		    new double[] { 33,    0  }, // false
		    new double[] { 21,    1  }, // false
		    new double[] { 42,    1  }, // true
		};
			
		PrincipalComponentAnalysis pca = new PrincipalComponentAnalysis(input);

		//Computes N number of Principal components, each of dimension 2 (xy-plane)
		//N is the number of data points/entrys
		pca.Compute();
		double[][] components = pca.Transform(input, 2);

		Debug.Log("Number of col: " + components[0].Length);
		Debug.Log("Number of rows: " + components.Length);

		for(int x = 0; x < components.Length; x++) {
			for(int y = 0; y < components[0].Length; y++) {
				Debug.Log("col: " + y + ", row: " + x + ": " + components[x][y]);
			}
		}
		Debug.Log("Finished");
	}
}
