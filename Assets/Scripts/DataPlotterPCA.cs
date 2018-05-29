using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Accord;
using Accord.Math;
using Accord.Statistics.Analysis;

//TODO: Determine how to scale graph properly

//@source Big Data Social Science Fellows @ Penn State - Plot Points 
//PCA Method: Take in N columns of data, calculate PCA, and project data onto first 3 principal components
public class DataPlotterPCA : MonoBehaviour {

	// The prefab for the data points that will be instantiated
	public GameObject PointHolder;
	public GameObject PointPrefab;
	public GameObject graphHolder;
	public GameObject textLabel;
	public GameObject labelHolder;

	public int categoryColumn = 5;

	//List for data columns to exclude
	public List<int> excludeColumns;

	// Name of the input file, no extension
	public string inputfile;

	//Scale the graph
	private float scale = 10;

	// List for holding data from CSV reader
	private List<Dictionary<string, object>> pointList;

	//Dictionary to hold mappings from species to color
	private Dictionary<string, Color> colorMap;

	//Dictionary to hold the solid color images derived from the colorMap
	private Dictionary<string, Texture2D> solidImages;

	// Use this for initialization
	void Start () {

		colorMap = new Dictionary<String, Color>();
		solidImages = new Dictionary<string, Texture2D> ();

		// Set pointlist to results of function Reader with argument inputfile
		pointList = CSVReader.Read(inputfile);

		//Log to console
		//Debug.Log(pointList);

		// Declare list of strings, fill with keys (column names)
		List<string> columnList = new List<string>(pointList[1].Keys);
		createColorMap (columnList);
		determineSolids ();

		//Calculate PCA and project data onto three most significant components before plotting
		double[][] transformedPoints = calcPCAProject ();
		plotPoints (transformedPoints, columnList);
		AssignLabels ();
	}

	private void plotPoints(double[][] pointXYZ, List<string> columnList) {

//		Instantiate at point XYZ added onto mean XYZ in order to ensure all data is "above ground"
//		float minX = Mathf.Abs(FindMinValue (pointXYZ.GetColumn (0)));
//		float minY = Mathf.Abs(FindMinValue (pointXYZ.GetColumn (1)));
//		float minZ = Mathf.Abs(FindMinValue (pointXYZ.GetColumn (2)));
//		Vector3 minVector = new Vector3(minX, minY, minZ);

		float maxX = Mathf.Abs (FindMaxValue (pointXYZ.GetColumn (0)));
		scale = 10 / maxX;

		//Loop through Pointlist, obtain points, and plot
		for (var i = 0; i < pointXYZ.Length; i++)
		{
			float x = Convert.ToSingle(pointXYZ[i][0]);

			float y = Convert.ToSingle(pointXYZ[i][1]);

			float z = Convert.ToSingle(pointXYZ[i][2]);

			// Instantiate as gameobject variable so that it can be manipulated within loop
			GameObject dataPoint = Instantiate(
				PointPrefab, 
				new Vector3(x, y, z) * scale, 
				Quaternion.identity);
			// Make dataPoint child of PointHolder object 
			dataPoint.transform.parent = PointHolder.transform;

			string type = Convert.ToString(pointList [i] [columnList[categoryColumn]]);
			//Debug.Log ("Species of point " + i + ": " + species);

			// Assigns original values to dataPointName
			string dataPointName = "(" + x + ", " + y + ", " + z + ")";

			// Assigns name to the prefab
			dataPoint.transform.name = dataPointName;

			// Gets material color and sets it to a new RGBA color we define
			dataPoint.GetComponent<Renderer>().material.color = colorMap[type];
		}
	}

	private void determineSolids() {
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

	//Create graph legend
	void OnGUI() {

		List<string> keys = new List<string>(solidImages.Keys);
		//Debug.Log (numLegend);

		// Make a background box of height '(numLegend + 1) * 20'
		GUI.Box(new Rect(10, 10, 100, (keys.Count + 1) * 20), "Graph Legend");

		for (int i = 1; i <= keys.Count; i++) {
			GUI.Label (new Rect (15, 10 + i * 20, 60, 20), keys[i - 1]); 
			GUI.DrawTexture (new Rect (80, 10 + i * 20, 15, 15), solidImages[keys[i - 1]]);
		}
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

		//Update axis titles to Principle Components
		GameObject x_Axis = Instantiate(textLabel, new Vector3(10, 3, 0), Quaternion.identity);
		x_Axis.GetComponent<TextMesh> ().text = "X-Axis: PCA1"; 
		x_Axis.transform.parent = labelHolder.transform;

		GameObject y_Axis = Instantiate(textLabel, new Vector3(3, 10, 0), Quaternion.identity);
		y_Axis.GetComponent<TextMesh> ().text = "Y-Axis: PCA2"; 
		y_Axis.transform.parent = labelHolder.transform;

		GameObject z_Axis = Instantiate(textLabel, new Vector3(0, 3, 10), Quaternion.identity);
		z_Axis.GetComponent<TextMesh> ().text = "Z-Axis: PCA3"; 
		z_Axis.transform.parent = labelHolder.transform;

		numberAxis ();
	}

	private void numberAxis() {
		for (int i = -10; i <= 10; i += 2) {
			GameObject xNum = Instantiate (textLabel, new Vector3 (i, 0, 0), Quaternion.identity);
			xNum.GetComponent<TextMesh> ().text = (i/scale).ToString ("0.0");
			xNum.transform.parent = graphHolder.transform;

			GameObject yNum = Instantiate (textLabel, new Vector3 (0, i, 0), Quaternion.identity);
			yNum.GetComponent<TextMesh> ().text = (i/scale).ToString ("0.0");
			yNum.transform.parent = graphHolder.transform;

			GameObject zNum = Instantiate (textLabel, new Vector3 (0, 0, i), Quaternion.identity);
			zNum.GetComponent<TextMesh> ().text = (i/scale).ToString ("0.0");
			zNum.transform.parent = graphHolder.transform;
		}
	}

	private double[][] calcPCAProject() {

		//The inputMatrix to the PCA
		double[][] inputMatrix = new double[pointList.Count][];

		//The 'width' of the inputMatrix
		int dataLength = pointList [1].Count;

		//Iterates through the original data and converts the wanted data into the input matrix
		for (int i = 0; i < pointList.Count; i++)
		{
			inputMatrix[i] = new double[dataLength - excludeColumns.Count - 1];
			int index = 0;
			List<object> data = new List<object>(pointList [i].Values);
			for (int j = 0; j < dataLength; j++) {
				if (j == categoryColumn || excludeColumns.Contains(j)) {
					continue;
				}
				// Get data in pointList at ith "row"
				inputMatrix[i][index] = Convert.ToDouble(data[j]);
				//Debug.Log ("value at " + i + ", " + index + ": " + inputMatrix [i] [index]);
				index++;
			}
		}

		PrincipalComponentAnalysis pca = new PrincipalComponentAnalysis(inputMatrix, AnalysisMethod.Center);

		//Computes N number of Principal components, each of dimension 3 (xyz-plane)
		//N is the number of data points/entrys
		pca.Compute();
		double[][] result = pca.Transform (inputMatrix, 3);
//		for (int i = 0; i < result.Length; i++) {
//			for (int j = 0; j < result [0].Length; j++) {
//				Debug.Log ("Value at " + i + ", " + j + ": " + result [i] [j]);
//			}
//		}

		return result;
	}

	private void setColor(LineRenderer line, Color color) {
		line.material = new Material (Shader.Find ("Particles/Additive"));
		line.startColor = color;
		line.endColor = color;
	}

	private float FindMaxValue(double[] array)
	{
		double max = double.MinValue;

		//Loop through array, overwrite existing minValue if new value is smaller
		for (var i = 0; i < array.Length; i++)
		{
			if (array[i] > max)
				max = array[i];
		}

		return (float) max;
	}

	private float FindMinValue(double[] array)
	{
		double min = double.MaxValue;

		//Loop through array, overwrite existing minValue if new value is smaller
		for (var i = 0; i < array.Length; i++)
		{
			if (array[i] < min)
				min = array[i];
		}

		return (float) min;
	}
		
	//Create the colorMap that contains unique colors for each species
	private void createColorMap(List<string> columnList) {

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
}