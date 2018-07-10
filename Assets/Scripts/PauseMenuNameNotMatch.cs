using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuNameNotMatch : MonoBehaviour {

	//Public gameobject to contain reference to pausemenu
	public GameObject pauseCanvas;
	
	// Update is called once per frame
	void Update () {
		//Without VR, if player currently presses down 'p' on keyboard, pause menu activates
		if (Input.GetKeyDown (KeyCode.P)) {
			pauseCanvas.SetActive (true);
			Time.timeScale = 0.0f;				//Timeflow is stopped. Can still look around but can't move
		}
	}

	//These functions are called by the various buttons on the main menu
    
	//Resume the application by removing the pause menu and resuming time
	public void resume() {
        VRInteraction.menuUp = false;
		pauseCanvas.SetActive (false);
		Time.timeScale = 1.0f;
	}

	//Allow the user to go back to the main menu in order to make new inputs. Time is resumed
	public void switchToInput() {
		SceneManager.LoadScene (0);
		Time.timeScale = 1.0f;
	}

	//Quit the application and/or editor.
	public void quit() {
		Application.Quit ();
	}
}
