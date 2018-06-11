using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Add VR adaption - UI/Menu pop-up. Select points using laser. Fly in 3D (check motion sickness)

//@source https://www.raywenderlich.com/149239/htc-vive-tutorial-unity

/* VR Adaption for HTC Vive. Controls as follows:
 * TouchPad + Grip = Move
 * Trigger = Select Point
 * Application Menu = Opens up UI Menu
*/

public class VRInteraction : MonoBehaviour {

	//Keep track of user that needs to move
	public GameObject user;

	//PauseMenu
	public GameObject pauseMenu;

	//Keep track of laser prefab
	public GameObject laserPrefab;
	// Instantiated laser gameobject and information
	private GameObject laser;
	private Transform laserTransform;
	// Where the laser should hit
	private Vector3 hitPoint; 

	//Mask to make sure that the raycast hits only dataPoints
	public LayerMask dataPointMask;

	private SteamVR_TrackedObject trackedObj;
	// Keeps track of controllers. Also which controller (L/R)
	private SteamVR_Controller.Device Controller
	{
		get { return SteamVR_Controller.Input((int)trackedObj.index); }
	}

	//Variables to keep track of input on touchpad and how that translates to 
	//3D movement in the application
	private Vector2 touchAxis;
	private Vector3 moveAxis;

	//Track the tracked object
	void Awake() {
		trackedObj = GetComponent<SteamVR_TrackedObject>();
	}

	void Start() {
		laser = Instantiate (laserPrefab);
		laserTransform = laser.transform;
	}
	
	// Update is called once per frame
	// Get input from controller in order to move player
	void Update () {
		
		//Get input to move on xz plane (horizontally)
		touchAxis = Controller.GetAxis ();
		if (touchAxis != Vector2.zero) {
			moveAxis = new Vector3 (touchAxis.x, 0, touchAxis.y);
		} else {
			moveAxis = new Vector3 (0, 0, 0);
		}

		//Get input to move on yx/z plane (vertically)
		if (Controller.GetPressDown (SteamVR_Controller.ButtonMask.Grip)) {
			moveAxis = moveAxis + new Vector3 (0, 1, 0);
		}

		//Set user velocity
		user.GetComponent<Rigidbody> ().velocity = moveAxis;

		//Have the laser pointer point to a datapoint and print out datapoint name
		if (Controller.GetHairTriggerDown ()) {
			ShowLaser ();
			RaycastHit hit;
			if (Physics.Raycast (trackedObj.transform.position, transform.forward, out hit, 100, dataPointMask)) {
				hitPoint = hit.point;
				ShowLaser (hit);
			} 
		} else {
			laser.SetActive (false);
		}

		//Have the menu pause menu pop up when the application menu button is hit
		if (Controller.GetPressDown (SteamVR_Controller.ButtonMask.ApplicationMenu)) {
			pauseMenu.SetActive (true);
			Time.timeScale = 0.0f;
		}
	}

	//Showing the laser in general
	private void ShowLaser()
	{
		// Show the laser
		laser.SetActive(true);
		// Position the laser object (which is actually a rectangular prism) in the forward direction
		laserTransform.position = transform.forward;
		// Point the laser in the forward direction
		laserTransform.LookAt(transform.forward); 
		// Scale the laser appropriately. Currently set to an arbitrary number
		laserTransform.localScale = 10;
	}

	//Showing the laser when it hits a datapoint
	private void ShowLaser(RaycastHit hit) {
		// Show the laser
		laser.SetActive(true);
		// Position the laser object (which is actually a rectangular prism) between the controller
		//and the location hit by the raycast
		laserTransform.position = Vector3.Lerp(trackedObj.transform.position, hitPoint, .5f);
		// Point the laser at the location where the raycast hit
		laserTransform.LookAt(hitPoint); 
		// Scale the laser appropriately 
		laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
			hit.distance);

		//Print out the dataPoints name to console. Later can make a pseudo pop up menu?
		Debug.Log (hit.transform.gameObject.name);
	}
}
