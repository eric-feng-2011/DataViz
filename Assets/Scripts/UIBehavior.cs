using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBehavior : MonoBehaviour {

	public Slider scaleSlider;
	public Text scaleText;
	public Button flipButton;
	public Button catButton;
	public InputField catKnownCol;

	private DataPlotterPCA plotterRef;

	private Dictionary<bool, string> TFText = new Dictionary<bool, string>()
					{{false, "False"}, {true, "True"}};

	// Use this for initialization
	void Start () {
		plotterRef = GameObject.FindGameObjectWithTag ("Plotter").GetComponent<DataPlotterPCA> ();
		scaleSlider.value = (float) plotterRef.scale;
		scaleText.text = "Scale: " + scaleSlider.value;
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
		plotterRef.graph.transform.localScale *= 10.0f / plotterRef.scale;

		plotterRef.doEverything ();
	}

	private void makeCatColInteractable() {
		if (!plotterRef.knownCategories) {
			catKnownCol.interactable = false;
		} else {
			catKnownCol.interactable = true;
		}
	}
}
