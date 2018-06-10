using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIBehavior : MonoBehaviour {

	public Slider scaleSlider;
	public Text scaleText;
	public Button flipButton;
	public Button catButton;
	public Button coorButton;
	public InputField catKnownCol;

	private Dictionary<bool, string> TFText = new Dictionary<bool, string>()
					{{false, "False"}, {true, "True"}};

	string flipDataKey = "flipData";
	string knownCategoriesKey = "knownCategories";
	string categoryColumnKey = "categoryColumn";
	string inputFileKey = "inputFile";
	string directoryKey = "directoryKey";
	string scaleKey = "scale";
	string excludeColKey = "excludeColumn";
	string coorDataKey = "coorData";
	string numExcludedKey = "numExcludedKey";

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
		scaleSlider.value = (float) scale;
		scaleText.text = "Scale: " + scale;
		flipButton.GetComponentInChildren<Text> ().text = TFText [flip];
		catButton.GetComponentInChildren<Text> ().text = TFText [knowCat];
		coorButton.GetComponentInChildren<Text> ().text = TFText [coor];
		makeCatColInteractable ();
	}

	public void changeCoord() {
		coor = !coor;
		coorButton.GetComponentInChildren<Text> ().text = TFText [coor];
	}

	public void changeCatKnownTF() {
		knowCat = !knowCat;
		catButton.GetComponentInChildren<Text> ().text = TFText [knowCat];
		makeCatColInteractable ();
	}

	public void changeCatKnown(string newInputColumn) {
		catCol = Convert.ToInt32(newInputColumn);
	}

	public void changeFlip() {
		flip = !flip;
		flipButton.GetComponentInChildren<Text> ().text = TFText [flip];
	}

	public void changeInput(string newInputFile) {
		input = newInputFile;
	}

	public void changeDirectory(string newDirectory) {
		directory = newDirectory;
	}

	public void adjustScale(float sliderScale) {
		scale = (int) sliderScale;
		scaleText.text = "Scale: " + scaleSlider.value;
	}

	//Code to parse string into integers obtained from Microsoft Docs
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

	private void makeCatColInteractable() {
		if (!knowCat) {
			catKnownCol.interactable = false;
		} else {
			catKnownCol.interactable = true;
		}
	}
}
