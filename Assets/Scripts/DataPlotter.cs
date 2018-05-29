using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Accord;
using Accord.Math;
using Accord.Statistics.Analysis;

//@source Big Data Social Science Fellows @ Penn State - Plot Points 
//PCA Method: Take in 3 columns of data and plot PCA and points in 3D with standard basis
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

	public LineRenderer PCA1;
	public LineRenderer PCA2;
	public LineRenderer PCA3;

	// Indices for columns to be assigned
	public int columnX = 1;
	public int columnY = 2;
	public int columnZ = 3;

	public int speciesColumn = 5;

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

	//Dictionary to hold mappings from species to color
	private Dictionary<String, Color> colorMap;

	// Use this for initialization
	void Start () {

		colorMap = new Dictionary<String, Color>();

		GameObject graph = Instantiate (graphHolder, new Vector3 (0, 0, 0), Quaternion.identity);
		graph.transform.localScale *= plotScale / 10f;
		graph.transform.parent = graphHolder.transform;

		// Set pointlist to results of function Reader with argument inputfile
		pointList = CSVReader.Read(inputfile);

		//Log to console
		//Debug.Log(pointList);

		// Declare list of strings, fill with keys (column names)
		List<string> columnList = new List<string>(pointList[1].Keys);
		createColorMap (columnList);

		// Print number of keys (using .count)
		//Debug.Log("There are " + columnList.Count + " columns in CSV");

		//foreach (string key in columnList)
		//	Debug.Log("Column name is " + key);
		
		plotPoints (columnList);
		AssignLabels ();
		Vector3 [] threePCA = calcPCA ();
//		for (int i = 0; i < threePCA.Length; i++) {
//			Debug.Log (threePCA [i]);
//		}
		plotPCA (threePCA);
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

			string species = Convert.ToString(pointList [i] [columnList[speciesColumn]]);
			//Debug.Log ("Species of point " + i + ": " + species);

			// Gets material color and sets it to a new RGBA color we define
			dataPoint.GetComponent<Renderer>().material.color = 
				colorMap[species];
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

	private Vector3[] calcPCA() {

		double[][] inputMatrix = new double[pointList.Count][];

		for (var i = 0; i < pointList.Count; i++)
		{
			// Get value in poinList at ith "row", in "column" Name,
			double x = Convert.ToDouble(pointList[i][xName]);

			double y = Convert.ToDouble(pointList[i][yName]);

			double z = Convert.ToDouble(pointList[i][zName]);

			inputMatrix [i] = new double[3] { x, y, z };
		}
		Vector3 meanCenter = new Vector3(getAverage(inputMatrix.GetColumn(0)), 
			getAverage(inputMatrix.GetColumn(1)), getAverage(inputMatrix.GetColumn(2)));
		
		PrincipalComponentAnalysis pca = new PrincipalComponentAnalysis(inputMatrix, AnalysisMethod.Center);

		//Computes N number of Principal components, each of dimension 3 (xyz-plane)
		//N is the number of data points/entrys
		pca.Compute();
		double[,] components = pca.ComponentMatrix; //Obtains the principal components of data
		double[] eigValues = pca.Eigenvalues;
		//double[][] components = pca.Transform(inputMatrix, 3);

		//First three vectors are principal components
		//Fourth is meanCenter of data. Fifth is eigenvalues of principal components
		Vector3[] majorThree = new Vector3[5];

		for (int i = 0; i < 3; i++) {
			float x = (float) components[0, i];

			float y = (float) components[1, i];

			float z = (float) components[2, i];

			majorThree[i] = new Vector3(x, y, z);
		}

		majorThree [3] = meanCenter;
		majorThree [4] = new Vector3((float) eigValues[0], (float) eigValues[1], (float) eigValues[2]);

		return majorThree;
	}

	private void plotPCA(Vector3[] pca) {

		int numPositions = 2;

		PCA1.positionCount = numPositions;
		PCA2.positionCount = numPositions;
		PCA3.positionCount = numPositions;

		Vector3 pca1 = pca [0];
		Vector3 pca2 = pca [1];
		Vector3 pca3 = pca [2];

		Vector3 center = pca [3];

		for (int i = 0; i < numPositions; i++) {
			PCA1.SetPosition (i, center + pca1 * i * plotScale);
			PCA2.SetPosition (i, center + pca2 * i * plotScale);
			PCA3.SetPosition (i, center + pca3 * i * plotScale);
		}

		setColor (PCA1, Color.red);
		setColor (PCA2, Color.yellow);
		setColor (PCA3, new Vector4 (1f, 0.5f, 0f, 1f));
	}

	private void setColor(LineRenderer line, Color color) {
		line.material = new Material (Shader.Find ("Particles/Additive"));
		line.startColor = color;
		line.endColor = color;
	}

	private float getAverage(double[] array) {
		double sum = 0;
		for (int i = 0; i < array.Length; i++) {
			sum += array [i];
		}
		return (float)sum / array.Length;
	}


	//Create the colorMap that contains unique colors for each species
	private void createColorMap(List<string> columnList) {

		int numUniq = 0;
		HashSet<string> trackSpecies = new HashSet<string> ();

		for (int i = 0; i < pointList.Count; i++) {
			string species = Convert.ToString (pointList [i] [columnList [speciesColumn]]);
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
}