using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Accord;
using Accord.Math;
using Accord.Statistics.Analysis;

//TODO: In addition to the scaling problems mentioned previously, scaling in general is really bad.
//TODO: When making new plot, graph is not scaling well. Additionally, previous labels still there,
//make 3D graph confusing. Not only graph. EVERYTHING is still there. Need to get rid of previous plot
//TODO: Getting errors regarding color scheme and colorMap dictionary. Probably need to clear when 
//recalculating
//TODO: And of course, add VR adaption

//Notes: Elements that should be included in the UI included everything that is currently part of the
//Unity Inspector and is not a GameObject. In addition need bool for data flip and a button that would
//recalulate PCA after new inputs are all put in.

//@source Big Data Social Science Fellows @ Penn State - Plot Points 
//PCA Method: Take in N columns of data, calculate PCA, and project data onto first 3 principal components
public class DataPlotterPCA : MonoBehaviour {

	// The prefab for the data points that will be instantiated
	public GameObject PointHolder;
	public GameObject PointPrefab;
	public GameObject graphHolder;
	public GameObject textLabel;
	public GameObject labelHolder;
	public GameObject graph;

	public bool flipData = false;
	public bool knownCategories = false;
	public int categoryColumn;

	//List for data columns to exclude
	public List<int> excludeColumns;

	// Name of the input file, no extension
	public string inputfile;

	//Scale the graph
	[Range(1, 100)]
	public int scale = 10;
	private float newScale;

	// List for holding data from CSV reader
	private List<Dictionary<string, object>> pointList;

	//Dictionary to hold mappings from species to color
	private Dictionary<string, Color> colorMap;

	//Dictionary to hold the solid color images derived from the colorMap
	private Dictionary<string, Texture2D> solidImages;

	//Private list to hold the keys of the dictionary in data
	private List<string> columnList;

	private Dictionary<string, bool> TFText = new Dictionary<string, bool>()
	{{"False", false}, {"True", true}};

	//PlayerPrefs keys
	string flipDataKey = "flipData";
	string knownCategoriesKey = "knownCategories";
	string categoryColumnKey = "categoryColumn";
	string inputFileKey = "inputFile";
	string scaleKey = "scale";
	string excludeColKey = "excludeColumn";

	// Use this for initialization
	void Start () {

		checkPlayerPrefs ();

		colorMap = new Dictionary<String, Color>();
		solidImages = new Dictionary<string, Texture2D> ();

		doEverything ();
	}

	public void doEverything() {

		graph.transform.localScale *= scale / 10.0f;

		colorMap.Clear ();
		solidImages.Clear ();

		// Set pointlist to results of function Reader with argument inputfile
		pointList = CSVReader.Read(inputfile);

		//Log to console
		//Debug.Log(pointList);

		// Declare list of strings, fill with keys (column names)
		columnList = new List<string>(pointList[1].Keys);
		if (knownCategories) {
			createColorMap ();
			determineSolids ();
		}

		//Calculate PCA and project data onto three most significant components before plotting
		double[][] transformedPoints = calcPCAProject ();
		plotPoints (transformedPoints);
		AssignLabels ();
	}

	private void plotPoints(double[][] pointXYZ) {

//		Instantiate at point XYZ added onto mean XYZ in order to ensure all data is "above ground"
//		float minX = Mathf.Abs(FindMinValue (pointXYZ.GetColumn (0)));
//		float minY = Mathf.Abs(FindMinValue (pointXYZ.GetColumn (1)));
//		float minZ = Mathf.Abs(FindMinValue (pointXYZ.GetColumn (2)));
//		Vector3 minVector = new Vector3(minX, minY, minZ);

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

			// Assigns original values to dataPointName
			string dataPointName = "(" + x + ", " + y + ", " + z + ")";

			// Assigns name to the prefab
			dataPoint.transform.name = dataPointName;

			// Gets material color and sets it to a new RGBA color we define
			if (knownCategories) {
				string type = Convert.ToString(pointList [i] [columnList[categoryColumn]]);
				//Debug.Log ("Species of point " + i + ": " + species);
				dataPoint.GetComponent<Renderer> ().material.color = colorMap [type];
			}
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
		for (int i = -scale; i <= scale; i += scale / 5) {
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

	private double[][] calcPCAProject() {

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
				inputMatrix[i][index] = Convert.ToDouble(data[j]);
				//Debug.Log ("value at " + i + ", " + index + ": " + inputMatrix [i] [index]);
				index++;
			}
		}

//		Somehow, this is throwing the data into an infinite loop. Or maybe my mac is not powerful enough to run the computations
		if (flipData) {
			inputMatrix = inputMatrix.Transpose ();
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
	private void createColorMap() {

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
		int excludeCount = excludeColumns.Count;
		excludeColumns.Clear ();
		for (int i = 0; i < excludeCount; i++) {
			if (PlayerPrefs.HasKey (excludeColKey + i.ToString ())) {
				excludeColumns.Add (PlayerPrefs.GetInt (excludeColKey + i.ToString ()));
			}
		}
	}
}