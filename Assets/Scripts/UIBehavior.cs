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
	public InputField catKnownCol;

	private DataPlotterPCA plotterRef;

	private Dictionary<bool, string> TFText = new Dictionary<bool, string>()
					{{false, "False"}, {true, "True"}};

	string flipDataKey = "flipData";
	string knownCategoriesKey = "knownCategories";
	string categoryColumnKey = "categoryColumn";
	string inputFileKey = "inputFile";
	string scaleKey = "scale";
	string excludeColKey = "excludeColumn";

	// Use this for initialization
	void Start () {
		plotterRef = GameObject.FindGameObjectWithTag ("Plotter").GetComponent<DataPlotterPCA> ();
		scaleSlider.value = (float) plotterRef.scale;
		scaleText.text = "Scale: " + plotterRef.scale;
		flipButton.GetComponentInChildren<Text> ().text = TFText [plotterRef.flipData];
		catButton.GetComponentInChildren<Text> ().text = TFText [plotterRef.knownCategories];
		makeCatColInteractable ();
	}

	public void changeCatKnownTF() {
		plotterRef.knownCategories = !plotterRef.knownCategories;
		catButton.GetComponentInChildren<Text> ().text = TFText [plotterRef.knownCategories];
		makeCatColInteractable ();
	}

	public void changeCatKnown(string newInputColumn) {
		plotterRef.categoryColumn = Convert.ToInt32(newInputColumn);
	}

	public void changeFlip() {
		plotterRef.flipData = !plotterRef.flipData;
		flipButton.GetComponentInChildren<Text> ().text = TFText [plotterRef.flipData];
	}

	public void changeInput(string newInputFile) {
		plotterRef.inputfile = newInputFile;
	}

	public void adjustScale(float sliderScale) {
		plotterRef.scale = (int) sliderScale;
		scaleText.text = "Scale: " + scaleSlider.value;
	}

	//Code to parse string into integers obtained from Microsoft Docs
	public void changeExclusion(string excluded) {
		char[] delimiters = { ',', ' ' };

		string[] excludeCols = excluded.Split (delimiters);

		//Convert the string ints into actual ints and add to empty excludeColumns list
		plotterRef.excludeColumns.Clear();
		int number = 0;
		foreach (string value in excludeCols) {
			if (Int32.TryParse (value, out number)) {
				plotterRef.excludeColumns.Add (Convert.ToInt32 (value));
			} else {
				continue;
			}
		}
	}

	public void reCalculatePCA() {
//		plotterRef.graph.transform.localScale *= 10.0f / plotterRef.scale;
//
//		plotterRef.doEverything ();

		//Keep track of the variables that the user inputs
		PlayerPrefs.SetInt(categoryColumnKey, plotterRef.categoryColumn);
		PlayerPrefs.SetString (inputFileKey, plotterRef.inputfile);
		PlayerPrefs.SetInt (scaleKey, plotterRef.scale);
		for (int i = 0; i < plotterRef.excludeColumns.Count; i++) {
			PlayerPrefs.SetInt (excludeColKey + i.ToString(), plotterRef.excludeColumns [i]);
		}
		PlayerPrefsX.SetBool (flipDataKey, plotterRef.flipData);
		PlayerPrefsX.SetBool (knownCategoriesKey, plotterRef.knownCategories);

		SceneManager.LoadScene(0);
	}

	private void makeCatColInteractable() {
		if (!plotterRef.knownCategories) {
			catKnownCol.interactable = false;
		} else {
			catKnownCol.interactable = true;
		}
	}
}
