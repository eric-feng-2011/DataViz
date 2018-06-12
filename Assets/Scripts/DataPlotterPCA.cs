using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using Accord;
using Accord.Math;
using Accord.Statistics.Analysis;

//@source Big Data Social Science Fellows @ Penn State - Plot Points 
//PCA Method: Take in N columns of data, calculate PCA, and project data onto first 3 principal components
public class DataPlotterPCA : MonoBehaviour {

	// The various public variables used in the script
	public GameObject PointHolder;
	public GameObject PointPrefab;
	public GameObject graphHolder;
	public GameObject textLabel;
	public GameObject labelHolder;
	public GameObject graph;

	private bool coorData = false;
	private bool flipData = false;
	private bool knownCategories = false;
	private int categoryColumn;

	//List for data columns to exclude
	private List<int> excludeColumns = new List<int>();

	// Name of the input file, with extension
	private string inputfile;

	//Path to location of file
	private string directory;

	//Scale the graph
	[Range(1, 100)]
	private int scale = 10;
	private float newScale;

	private int numExcluded;

	private int dataPointMask = 8;

	// List for holding data from CSV reader
	private List<Dictionary<string, object>> pointList;

	//Dictionary to hold mappings from species to color
	private Dictionary<string, Color> colorMap;

	//Dictionary to hold the solid color images derived from the colorMap
	private Dictionary<string, Texture2D> solidImages;

	//Private list to hold the keys of the dictionary in data
	private List<string> columnList;

	//PlayerPrefs keys
	string flipDataKey = "flipData";
	string knownCategoriesKey = "knownCategories";
	string categoryColumnKey = "categoryColumn";
	string inputFileKey = "inputFile";
	string directoryKey = "directoryKey";
	string scaleKey = "scale";
	string excludeColKey = "excludeColumn";
	string coorDataKey = "coorData";
	string numExcludedKey = "numExcludedKey";


	void Awake() {
		checkPlayerPrefs ();

		colorMap = new Dictionary<String, Color>();
		solidImages = new Dictionary<string, Texture2D> ();

	}

	// Use this for initialization
	void Start () {

		graph.transform.localScale *= scale / 10.0f;

		// Set pointlist to results of function Reader with argument inputfile
		writeFile();
		pointList = CSVReader.Read("input");

		if (flipData) {
			pointList = TransposeData.TransposeList (pointList);
		}

		// Declare list of strings, fill with keys (column names)
		columnList = new List<string>(pointList[1].Keys);

		//Create a graph legend and color the points if know categories
		if (knownCategories) {
			ColorGraph.createColor (pointList, categoryColumn);
			colorMap = ColorGraph.getColorMap ();
			solidImages = ColorGraph.getSolidImages ();
		}

		//Plot according to input data given
		if (coorData) {

			double[][] pointCoor = convertTo2D(pointList);
			plot (pointCoor);

		} else {
			//Calculate PCA and project data onto three most significant components before plotting
			double[][] transformedPoints = calcPCAProject ();
			plot (transformedPoints);
		}
	}

	//Create graph legend
	void OnGUI() {

		if (!knownCategories) {
			return;
		}

		List<string> keys = new List<string>(solidImages.Keys);
		//Debug.Log (numLegend);

		// Make a background box of height '(numLegend + 1) * 20'
		GUI.Box(new Rect(10, 10, 100, (keys.Count + 1) * 20), "Graph Legend");

		for (int i = 1; i <= keys.Count; i++) {
			GUI.Label (new Rect (15, 10 + i * 20, 60, 20), keys[i - 1]); 
			GUI.DrawTexture (new Rect (80, 10 + i * 20, 15, 15), solidImages[keys[i - 1]]);
		}
	}

	private void checkPlayerPrefs() {
		if (PlayerPrefs.HasKey (flipDataKey)) {
			flipData = PlayerPrefsX.GetBool (flipDataKey);
		}
		if (PlayerPrefs.HasKey (categoryColumnKey)) {
			categoryColumn = PlayerPrefs.GetInt (categoryColumnKey);
		}
		if (PlayerPrefs.HasKey (knownCategoriesKey)) {
			knownCategories = PlayerPrefsX.GetBool (knownCategoriesKey);
		}
		if (PlayerPrefs.HasKey (inputFileKey)) {
			inputfile = PlayerPrefs.GetString (inputFileKey);
		}
		if (PlayerPrefs.HasKey (scaleKey)) {
			scale = PlayerPrefs.GetInt (scaleKey);
		}
		if (PlayerPrefs.HasKey (coorDataKey)) {
			coorData = PlayerPrefsX.GetBool (coorDataKey);
		}
		if (PlayerPrefs.HasKey(directoryKey)) {
			directory = PlayerPrefs.GetString(directoryKey);
		}
		if (PlayerPrefs.HasKey (numExcludedKey)) {
			numExcluded = PlayerPrefs.GetInt (numExcludedKey);
		}

		excludeColumns.Clear ();
		for (int i = 0; i < numExcluded; i++) {
			if (PlayerPrefs.HasKey (excludeColKey + i.ToString ())) {
				excludeColumns.Add (PlayerPrefs.GetInt (excludeColKey + i.ToString ()));
			}
		}
	}

	void writeFile() {
		//Debug.Log (directory);
		inputfile = directory + "/" + inputfile;
		//Debug.Log (inputfile);
		foreach (string file in Directory.GetFiles(directory))
		{
			//Debug.Log (file);
			if (file == inputfile) {
				string contents = File.ReadAllText (file);

				string path = "Assets/Resources/input.csv";

				//Write some text to the input.txt file
				StreamWriter writer = new StreamWriter(path, false);
				writer.WriteLine(contents);
				writer.Close();

				//Re-import the file to update the reference in the editor
				AssetDatabase.ImportAsset(path); 
				TextAsset asset = (TextAsset) Resources.Load("input");

				//Debug.Log (asset.text);
			}
		}
	}

	private double[][] convertTo2D(List<Dictionary<string, object>> pointList) {
		//The inputMatrix to the PCA
		double[][] inputMatrix = new double[pointList.Count][];

		//The 'width' of the inputMatrix
		int dataLength = pointList [1].Count;

		//Iterates through the original data and converts the wanted data into the input matrix
		for (int i = 0; i < inputMatrix.Length; i++)
		{
			if (knownCategories) {
				inputMatrix [i] = new double[dataLength - excludeColumns.Count - 1];
			} else {
				inputMatrix [i] = new double[dataLength - excludeColumns.Count];
			}

			int index = 0;
			List<object> data = new List<object>(pointList [i].Values);

			for (int j = 0; j < dataLength; j++) {
				if ((knownCategories && j == categoryColumn) || excludeColumns.Contains(j)) {
					continue;
				}
				// Get data in pointList at ith "row"
				try {
					inputMatrix[i][index] = Convert.ToDouble(data[j]);
					//Debug.Log(excludeColumns.Contains(j) + " " + j + " " + data[j]);
				} catch (Exception e) {
					Debug.Log ("Wrong Dimensions or not double value: " + e);
				}
				//Debug.Log ("value at " + i + ", " + index + ": " + inputMatrix [i] [index]);
				index++;
			}
		}

		return inputMatrix;
	}

	private double[][] calcPCAProject() {

		//The inputMatrix to the PCA
		double[][] inputMatrix = convertTo2D(pointList);

		PrincipalComponentAnalysis pca = new PrincipalComponentAnalysis(inputMatrix, AnalysisMethod.Center);

		//Computes N number of Principal components
		//N is the number of data points/entrys
		pca.Compute();

		//Transforms the initial data by projecting it into the third dimension
		//using the found principle component axises
		double[][] result = pca.Transform (inputMatrix, 3);

		return result;
	}

	public void plot(double[][] coordinates) {

		plotPoints (coordinates);
		AssignLabels ();
	}

	private void plotPoints(double[][] pointXYZ) {

		float maxX = Mathf.Abs (FindMaxValue (pointXYZ.GetColumn (0)));
		newScale = scale / maxX;

		//Loop through Pointlist, obtain points, and plot
		for (var i = 0; i < pointXYZ.Length; i++)
		{
			float x = Convert.ToSingle(pointXYZ[i][0]);

			float y = Convert.ToSingle(pointXYZ[i][1]);

			float z = Convert.ToSingle(pointXYZ[i][2]);

			// Instantiate as gameobject variable so that it can be manipulated within loop
			GameObject dataPoint = Instantiate(
				PointPrefab, 
				new Vector3(x, y, z) * newScale, 
				Quaternion.identity);
			// Make dataPoint child of PointHolder object 
			dataPoint.transform.parent = PointHolder.transform;

			string type = "Point";

			// Gets material color and sets it to a new RGBA color we define
			if (knownCategories) {
				type = Convert.ToString(pointList [i] [columnList[categoryColumn]]);
				//Debug.Log ("Species of point " + i + ": " + species);
				dataPoint.GetComponent<Renderer> ().material.color = colorMap [type];
			}

			// Assigns original values to dataPointName
			string dataPointName = type + ": (" + x + ", " + y + ", " + z + ")";

			// Assigns name to the prefab
			dataPoint.transform.name = dataPointName;

			dataPoint.layer = dataPointMask;
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
		point_Count.GetComponent<TextMesh> ().text = "Number of Points: "
				+ pointList.Count.ToString ("0");
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
		for (int i = -scale; i <= scale; i += 2) {
			GameObject xNum = Instantiate (textLabel, new Vector3 (i, 0, 0), Quaternion.identity);
			xNum.GetComponent<TextMesh> ().text = (i/newScale).ToString ("0.0");
			xNum.transform.parent = graphHolder.transform;

			GameObject yNum = Instantiate (textLabel, new Vector3 (0, i, 0), Quaternion.identity);
			yNum.GetComponent<TextMesh> ().text = (i/newScale).ToString ("0.0");
			yNum.transform.parent = graphHolder.transform;

			GameObject zNum = Instantiate (textLabel, new Vector3 (0, 0, i), Quaternion.identity);
			zNum.GetComponent<TextMesh> ().text = (i/newScale).ToString ("0.0");
			zNum.transform.parent = graphHolder.transform;
		}
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


}