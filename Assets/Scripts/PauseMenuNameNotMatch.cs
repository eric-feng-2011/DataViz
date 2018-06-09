using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuNameNotMatch : MonoBehaviour {

	public GameObject pauseMenu;
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.P)) {
			pauseMenu.SetActive (true);
			Time.timeScale = 0.0f;
		}
	}

	public void resume() {
		pauseMenu.SetActive (false);
		Time.timeScale = 1.0f;
	}

	public void switchToInput() {
		SceneManager.LoadScene (0);
		Time.timeScale = 1.0f;
	}

	public void quit() {
		Application.Quit ();
		UnityEditor.EditorApplication.isPlaying = false;
	}
}
