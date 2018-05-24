using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Accord;
using Accord.Math;
using Accord.Statistics.Analysis;

//@source Big Data Social Science Fellows @ Penn State
public class DataPlotter : MonoBehaviour {

	//Scale the graph
	public float plotScale = 10;
	public float offset = 0.25f;

	// The prefab for the data points that will be instantiated
	public GameObject PointHolder;
	public GameObject PointPrefab;
	public GameObject graph;
	public GameObject graphHolder;
	public GameObject textLabel;
	public GameObject labelHolder;

	// Indices for columns to be assigned
	public int columnX = 1;
	public int columnY = 2;
	public int columnZ = 3;

	// Full column names
	private string xName;
	private string yName;
	private string zName;

	// Max and Min of colums
	private float xMax;
	private float yMax;
	private float zMax;

	private float xMin;
	private float yMin;
	private float zMin;

	// Name of the input file, no extension
	public string inputfile;

	// List for holding data from CSV reader
	private List<Dictionary<string, object>> pointList;

	// Use this for initialization
	void Start () {

		GameObject graph = Instantiate (graphHolder, new Vector3 (0, 0, 0), Quaternion.identity);
		graph.transform.localScale *= plotScale / 10f;
		graph.transform.parent = graphHolder.transform;

		// Set pointlist to results of function Reader with argument inputfile
		pointList = CSVReader.Read(inputfile);

		//Log to console
		//Debug.Log(pointList);

		// Declare list of strings, fill with keys (column names)
		List<string> columnList = new List<string>(pointList[1].Keys);

		// Print number of keys (using .count)
		//Debug.Log("There are " + columnList.Count + " columns in CSV");

		//foreach (string key in columnList)
		//	Debug.Log("Column name is " + key);
		
		plotPoints (columnList);
		AssignLabels ();
		double [][] threePCA = calcPCA ();
		for (int i = 0; i < threePCA.Length; i++) {
			for (int j = 0; j < threePCA[0].Length; j++) {
				Debug.Log("Row: " + i + ", Col: " + j + ": " + threePCA[i][j]);
			}
		}
	}

	private void plotPoints(List<string> columnList) {
		// Assign column name from columnList to Name variables
		xName = columnList[columnX];
		yName = columnList[columnY];
		zName = columnList[columnZ];

		// Get maxes of each axis
		xMax = FindMaxValue(xName);
		yMax = FindMaxValue(yName);
		zMax = FindMaxValue(zName);

		// Get minimums of each axis
		xMin = FindMinValue(xName);
		yMin = FindMinValue(yName);
		zMin = FindMinValue(zName);

		//Loop through Pointlist
		for (var i = 0; i < pointList.Count; i++)
		{
			// Get value in poinList at ith "row", in "column" Name, normalize
			float x = 
				(Convert.ToSingle(pointList[i][xName]) - xMin) / (xMax - xMin);
			//Convert.ToSingle(pointList[i][xName]);

			float y = 
				(Convert.ToSingle(pointList[i][yName]) - yMin) / (yMax - yMin);
			//Convert.ToSingle(pointList[i][yName]);

			float z = 
				(Convert.ToSingle(pointList[i][zName]) - zMin) / (zMax - zMin);
			//Convert.ToSingle(pointList[i][zName]);			

			// Instantiate as gameobject variable so that it can be manipulated within loop
			GameObject dataPoint = Instantiate(
				PointPrefab, 
				new Vector3(x, y, z) * plotScale, 
				Quaternion.identity);
			// Make dataPoint child of PointHolder object 
			dataPoint.transform.parent = PointHolder.transform;

			// Assigns original values to dataPointName
			string dataPointName = 
				pointList[i][xName] + " "
				+ pointList[i][yName] + " "
				+ pointList[i][zName];

			// Assigns name to the prefab
			dataPoint.transform.name = dataPointName;

			// Gets material color and sets it to a new RGBA color we define
			dataPoint.GetComponent<Renderer>().material.color = 
				new Color(x,y,z, 1.0f);
		}
	}

	private float FindMaxValue(string columnName)
	{
		//set initial value to first value
		float maxValue = Convert.ToSingle(pointList[0][columnName]);

		//Loop through Dictionary, overwrite existing maxValue if new value is larger
		for (var i = 0; i < pointList.Count; i++)
		{
			if (maxValue < Convert.ToSingle(pointList[i][columnName]))
				maxValue = Convert.ToSingle(pointList[i][columnName]);
		}

		//Spit out the max value
		return maxValue;
	}

	private float FindMinValue(string columnName)
	{

		float minValue = Convert.ToSingle(pointList[0][columnName]);

		//Loop through Dictionary, overwrite existing minValue if new value is smaller
		for (var i = 0; i < pointList.Count; i++)
		{
			if (Convert.ToSingle(pointList[i][columnName]) < minValue)
				minValue = Convert.ToSingle(pointList[i][columnName]);
		}

		return minValue;
	}

	// Finds labels named in scene, assigns values to their text meshes
	// WARNING: game objects need to be named within scene 
	private void AssignLabels()
	{
		//update file name
		GameObject data_Label = Instantiate (textLabel, new Vector3 (12, 10, 0), Quaternion.identity);
		data_Label.GetComponent<TextMesh> ().text = inputfile;
		data_Label.transform.parent = labelHolder.transform;

		// Update point counter
		GameObject point_Count = Instantiate(textLabel, new Vector3(12, 8, 0), Quaternion.identity);
		point_Count.GetComponent<TextMesh>().text = "Number of Points: " 
			+ pointList.Count.ToString("0");
		point_Count.transform.parent = labelHolder.transform;

		// Update axis titles to ColumnNames
		GameObject x_Axis = Instantiate(textLabel, new Vector3(plotScale, 3, 0), Quaternion.identity);
		x_Axis.GetComponent<TextMesh> ().text = "X-Axis: " + xName; 
		x_Axis.transform.parent = labelHolder.transform;

		GameObject y_Axis = Instantiate(textLabel, new Vector3(3, plotScale, 0), Quaternion.identity);
		y_Axis.GetComponent<TextMesh> ().text = "Y-Axis: " + yName; 
		y_Axis.transform.parent = labelHolder.transform;

		GameObject z_Axis = Instantiate(textLabel, new Vector3(0, 3, plotScale), Quaternion.identity);
		z_Axis.GetComponent<TextMesh> ().text = "Z-Axis: " + zName; 
		z_Axis.transform.parent = labelHolder.transform;

		numberAxis ();
	}

	private void numberAxis() {
		for (int i = 0; i <= plotScale; i += 2) {
			GameObject xNum = Instantiate (textLabel, new Vector3 (i, 0, 0), Quaternion.identity);
			xNum.GetComponent<TextMesh> ().text = i.ToString ("0");
			xNum.transform.parent = graphHolder.transform;

			GameObject yNum = Instantiate (textLabel, new Vector3 (0, i, 0), Quaternion.identity);
			yNum.GetComponent<TextMesh> ().text = i.ToString ("0");
			yNum.transform.parent = graphHolder.transform;

			GameObject zNum = Instantiate (textLabel, new Vector3 (0, 0, i), Quaternion.identity);
			zNum.GetComponent<TextMesh> ().text = i.ToString ("0");
			zNum.transform.parent = graphHolder.transform;
		}
	}

	private double[][] calcPCA() {

		double[][] inputMatrix = new double[pointList.Count][];

		for (var i = 0; i < pointList.Count; i++)
		{
			// Get value in poinList at ith "row", in "column" Name,
			double x = Convert.ToDouble(pointList[i][xName]);

			double y = Convert.ToDouble(pointList[i][yName]);

			double z = Convert.ToDouble(pointList[i][zName]);

			inputMatrix [i] = new double[3] { x, y, z };
		}

		PrincipalComponentAnalysis pca = new PrincipalComponentAnalysis(inputMatrix);

		//Computes N number of Principal components, each of dimension 3 (xyz-plane)
		//N is the number of data points/entrys
		pca.Compute();
		double[][] components = pca.Transform(inputMatrix, 3);

		double[][] majorThree = new double[3][];

		for (int i = 0; i < 3; i++) {
			double x = components[i][0];

			double y = components[i][1];

			double z = components[i][2];

			majorThree [i] = new double[3] { x, y, z };
		}

		return majorThree;
	}
}