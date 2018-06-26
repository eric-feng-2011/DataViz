using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.IO;
using Accord;
using Accord.Math;
using Accord.Statistics.Analysis;

//@source Big Data Social Science Fellows @ Penn State - Plot Points 
//PCA Method: Take in N columns of data, calculate PCA, and project data onto first 3 principal components

//TODO: print out the name of the point that is pointed to. Orient Labels correctly
public class DataPlotterPCA : MonoBehaviour {

	// The various public variables used in the script
	public GameObject PointHolder;
	public GameObject PointPrefab;
	public GameObject graphHolder;
	public GameObject textLabel;
	public GameObject labelHolder;
	public GameObject graph;

    //Variable to create VR Legend
    public GameObject legendPanel;
    public GameObject legendItem;

    //Variable to determine spacing between items in legend
    private int legendSpacing = 5;

	//Variables to keep track of the settings and inputs that the user had previously input in the main menu
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

	//The number of excluded columns. (excludeColumns.Count)
	private int numExcluded;

	//The mask that will be assigned to datapoints. Needed so that users can select data points in VR
	private int dataPointMask = 8;

	// List for holding data from CSV reader
	private List<Dictionary<string, object>> pointList;

	//Dictionary to hold mappings from species to color
	private Dictionary<string, Color> colorMap;

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
		//Check the playerprefs and reset various private variables above to match user input
		checkPlayerPrefs ();

		colorMap = new Dictionary<String, Color>();
	}

	// Use this for initialization
	void Start () {

		//Scale the graph
		graph.transform.localScale *= scale / 10.0f;

		// Set pointlist to results of function Reader with argument inputfile
		writeFile();
		pointList = CSVReader.Read("input");

		//If the user wants the data flipped, here is where it is done
		if (flipData) {
			pointList = TransposeData.TransposeList (pointList);
		}

		// Declare list of strings, fill with keys (column names)
		columnList = new List<string>(pointList[1].Keys);

		//Create a graph legend and color the points if know categories
		if (knownCategories) {
			ColorGraph.createColor (pointList, categoryColumn);
			colorMap = ColorGraph.getColorMap ();
		}

		//Plot according to input data given
		if (coorData) {

			//Coordinate data does not have PCA run on it. 
			//Converting the file into an appropriate format for plotting is enough
			double[][] pointCoor = convertTo2D(pointList);
			plot (pointCoor);

		} else {
			//Calculate PCA and project data onto three most significant components before plotting
			double[][] transformedPoints = calcPCAProject ();
			plot (transformedPoints);
		}
        createLegend();
	}

    //Check all the playerprefs and reset various private variables above to match user input
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

	//Read in a file from outside Unity and load it into a file named 'input.csv'
	void writeFile() {
		inputfile = directory + "/" + inputfile;
		foreach (string file in Directory.GetFiles(directory))
		{
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
			}
		}
	}

	//Take a List of Dictionaries and convert it into a 2D array.
	//This method converts the pointList into a format that is easier to run PCA on and/or plot
	private double[][] convertTo2D(List<Dictionary<string, object>> pointList) {
		//The inputMatrix to the PCA
		double[][] inputMatrix = new double[pointList.Count][];

		//The 'width' of the inputMatrix
		int dataLength = pointList [1].Count;

		//Iterates through the original data and converts the wanted data into the input matrix
		for (int i = 0; i < inputMatrix.Length; i++)
		{
			//Determine the size of a nested array
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
				// Get data in pointList in ith "row", jth "column"
				// Import the aforementioned value into the appropriate 2D array location
				inputMatrix[i][index] = Convert.ToDouble(data[j]);
	
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

		//Transforms the initial data by projecting it into three dimensions
		//using the found principle component axises
		double[][] result = pca.Transform (inputMatrix, 3);

		//Return the transformed data. Contains the coordinates of the data points after projection
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

            //Default name of point
			string type = "Point: " + i;

			// Gets material color and sets it to a new RGBA color we define
			if (knownCategories) {
				type = Convert.ToString(pointList [i] [columnList[categoryColumn]]);
				dataPoint.GetComponent<Renderer> ().material.color = colorMap [type];
			}

			// Assigns original values to dataPointName
			string dataPointName = type;

			// Assigns name to the individual data point
			dataPoint.transform.name = dataPointName;

			// Gives the data point the appropriate layerMask so that users can interact with it in VR
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
        increaseResolution(data_Label);

		// Update point counter
		GameObject point_Count = Instantiate(textLabel, new Vector3(12, 8, 0), Quaternion.identity);
		point_Count.GetComponent<TextMesh> ().text = "Number of Points: "
				+ pointList.Count.ToString ("0");
		point_Count.transform.parent = labelHolder.transform;
        increaseResolution(point_Count);

        //Update axis titles to Principle Components
        GameObject x_Axis = Instantiate(textLabel, new Vector3(scale, 1, 0), Quaternion.identity);
		x_Axis.GetComponent<TextMesh> ().text = "X-Axis: PCA1"; 
		x_Axis.transform.parent = labelHolder.transform;
        increaseResolution(x_Axis);

        GameObject y_Axis = Instantiate(textLabel, new Vector3(1, scale, 0), Quaternion.identity);
		y_Axis.GetComponent<TextMesh> ().text = "Y-Axis: PCA2"; 
		y_Axis.transform.parent = labelHolder.transform;
        increaseResolution(y_Axis);

        GameObject z_Axis = Instantiate(textLabel, new Vector3(0, 1, scale), Quaternion.identity);
		z_Axis.GetComponent<TextMesh> ().text = "Z-Axis: PCA3"; 
		z_Axis.transform.parent = labelHolder.transform;
        increaseResolution(z_Axis);
    }

    private void increaseResolution(GameObject label)
    {
        if (!label.CompareTag("Label"))
        {
            return;
        }

        label.GetComponent<TextMesh>().fontSize = 100;
        label.transform.localScale = new Vector3(0.25f, 0.25f, 1);
    }

    private void createLegend()
    {

        // If there are no known categories, we can not create a graph legend
        if (!knownCategories)
        {
            return;
        }

        List<string> keys = new List<string>(colorMap.Keys);

        int offsetX = 0;
        int offsetY = 0;
        //Fill the panel with an appropriate legend
        for (int i = 0; i < keys.Count; i++)
        {
            GameObject legendContent = Instantiate(legendItem, legendPanel.transform);
            if (i != 0) {
                offsetY += legendSpacing;
            }
            if (legendContent.transform.position.y - offsetY <= 0)
            {
                offsetY = 0;
                offsetX += 20;
            }
            legendContent.transform.position = new Vector3(legendContent.transform.position.x + offsetX, legendContent.transform.position.y - offsetY, legendContent.transform.position.z);
            legendContent.GetComponentInChildren<Text>().text = keys[i];
            legendContent.GetComponentInChildren<Image>().color = colorMap[keys[i]];
        }
    }

	// Method to find the largest value in an array
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