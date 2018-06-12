using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Script that dictates the behavior of the main menu
public class UIBehavior : MonoBehaviour {

	// Public variables that keep reference to the components on the main menu
	public Slider scaleSlider;
	public Text scaleText;
	public Button flipButton;
	public Button catButton;
	public Button coorButton;
	public InputField catKnownCol;

	// Dictionary that allows for conversion between strings and booleans.
	private Dictionary<bool, string> TFText = new Dictionary<bool, string>()
					{{false, "False"}, {true, "True"}};

	//PlayerPrefs Keys
	string flipDataKey = "flipData";
	string knownCategoriesKey = "knownCategories";
	string categoryColumnKey = "categoryColumn";
	string inputFileKey = "inputFile";
	string directoryKey = "directoryKey";
	string scaleKey = "scale";
	string excludeColKey = "excludeColumn";
	string coorDataKey = "coorData";
	string numExcludedKey = "numExcludedKey";

	//Variables that keep reference to PlayerPrefs values
	private bool flip;
	private bool knowCat;
	private bool coor;
	private int catCol;
	private string input;
	private string directory;
	private int scale = 10;
	private List<int> excludeCols = new List<int>();
	private int numExcluded;

	// Use this for initialization
	void Start () {
		//Set the value of the slider to a default
		scaleSlider.value = (float) scale;
		scaleText.text = "Scale: " + scale;

		//Set the values of the boolean variables to be visible and appropriate
		flipButton.GetComponentInChildren<Text> ().text = TFText [flip];
		catButton.GetComponentInChildren<Text> ().text = TFText [knowCat];
		coorButton.GetComponentInChildren<Text> ().text = TFText [coor];

		//Make the category column inputfield interactable according 
		//to if we know the category column or not
		makeCatColInteractable ();
	}

	//Occurs when appropriate button is hit. Changes coor value and text
	public void changeCoord() {
		coor = !coor;
		coorButton.GetComponentInChildren<Text> ().text = TFText [coor];
	}

	//Occurs when appropriate button is hit. Changes category known value and text
	//Also makes category column inputfield interactable as needed
	public void changeCatKnownTF() {
		knowCat = !knowCat;
		catButton.GetComponentInChildren<Text> ().text = TFText [knowCat];
		makeCatColInteractable ();
	}

	//Reads in category column from inputfield
	public void changeCatKnown(string newInputColumn) {
		catCol = Convert.ToInt32(newInputColumn);
	}

	//Occurs when appropriate button is hit. Changes flip value and text
	public void changeFlip() {
		flip = !flip;
		flipButton.GetComponentInChildren<Text> ().text = TFText [flip];
	}

	//Reads in data file name from inputfield
	public void changeInput(string newInputFile) {
		input = newInputFile;
	}

	//Reads in data file absolute location in directory from inputfield
	public void changeDirectory(string newDirectory) {
		directory = newDirectory;
	}

	//Adjusts the scale value accordingly
	public void adjustScale(float sliderScale) {
		scale = (int) sliderScale;
		scaleText.text = "Scale: " + scaleSlider.value;
	}

	//Code to parse string into integers obtained from Microsoft Docs
	//Takes in the list from the inputfield and parses it into a list of integers
	//Keeps track of the number of integers in the list
	public void changeExclusion(string excluded) {
		char[] delimiters = { ',', ' ' };

		string[] excludeColStrings = excluded.Split (delimiters);

		//Convert the string ints into actual ints and add to empty excludeColumns list
		int number = 0;
		foreach (string value in excludeColStrings) {
			if (Int32.TryParse (value, out number)) {
				excludeCols.Add(Convert.ToInt32(value));
			} else {
				continue;
			}
		}
		numExcluded = excludeCols.Count;
	}

	//When this button is hit, all the settings and inputs that the user has changed or made 
	//are recorded and effecitvely sent to another scene where the data is graphically rendered
	public void reCalculatePCA() {

		//Keep track of the variables that the user inputs
		PlayerPrefs.SetInt(categoryColumnKey, catCol);
		PlayerPrefs.SetString (inputFileKey, input);
		PlayerPrefs.SetString (directoryKey, directory);
		PlayerPrefs.SetInt (scaleKey, scale);
		for (int i = 0; i < excludeCols.Count; i++) {
			PlayerPrefs.SetInt (excludeColKey + i.ToString(), excludeCols [i]);
		}
		PlayerPrefs.SetInt ("numExcludedKey", numExcluded);
		PlayerPrefsX.SetBool (flipDataKey, flip);
		PlayerPrefsX.SetBool (knownCategoriesKey, knowCat);
		PlayerPrefsX.SetBool (coorDataKey, coor);

		SceneManager.LoadScene(1);
	}

	//Make the category column inputfield interactable as appropriate
	private void makeCatColInteractable() {
		if (!knowCat) {
			catKnownCol.interactable = false;
		} else {
			catKnownCol.interactable = true;
		}
	}
}
