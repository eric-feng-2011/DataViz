using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//TODO: Add VR adaption - UI/Menu pop-up. Select points using laser. Fly in 3D (check motion sickness)
//TODO: Revise Laser implementation so that one can click with it on menu and datapoints.
//TODO: Consider changing moving mechanism in order to prevent motion sickness

//@source https://www.raywenderlich.com/149239/htc-vive-tutorial-unity

/* VR Adaption for HTC Vive. Controls as follows:
 * TouchPad + Grip = Move
 * Trigger = Select Point
 * Application Menu = Opens up UI Menu
*/

public class VRInteraction : MonoBehaviour {

    public Vector3 holdRotation = new Vector3(0, 0, 1);
    public Vector3 holdPosition;

	//Keep track of user that needs to move
	public GameObject user;

	//PauseMenu
	public GameObject pauseMenu;

	//Text to show datapoint selected
	public GameObject pointText;

	//Keep track of laser prefab
	public GameObject laserPrefab;
	// Instantiated laser gameobject and information
	private GameObject laser;
	private Transform laserTransform;
	// Where the laser should hit
	private Vector3 hitPoint; 

	//Mask to make sure that the raycast hits only dataPoints and UI components
	public LayerMask dataPointMask;
	public LayerMask UIMask;

    private int leftMostIndex;
    private int rightMostIndex;

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
        leftMostIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
        rightMostIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
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
		if (Controller.GetPressDown (SteamVR_Controller.ButtonMask.Grip) && (int)trackedObj.index == 0) {
			moveAxis = moveAxis + new Vector3 (0, -1, 0);
		} else if (Controller.GetPressDown (SteamVR_Controller.ButtonMask.Grip) && (int)trackedObj.index == 1) {
			moveAxis = moveAxis + new Vector3 (0, 1, 0);
		}

		//Set user velocity
		user.GetComponent<Rigidbody> ().velocity = moveAxis;

        ShowLaser();

        //Have the laser pointer point to a datapoint and print out datapoint name
        //if (Controller.GetHairTriggerDown ()) {
        //	RaycastHit hit;
        //	//This allows laser to point out various data points
        //	if (Physics.Raycast (trackedObj.transform.position, transform.forward, out hit, 100, dataPointMask)) {
        //		ShowLaser (hit);
        //		pointText.GetComponent<TextMesh>().text = hit.collider.name;
        //	} else if (Physics.Raycast (trackedObj.transform.position, transform.forward, out hit, 100, UIMask)) { //This allows user to hit various buttons
        //		ShowLaser (hit);
        //		var btn = hit.collider.GetComponent<Button> ();
        //		if (btn != null) {
        //			btn.onClick.Invoke ();
        //		}
        //	}
        //} else {
        //	pointText.GetComponent<TextMesh>().text = "";
        //}

        //Have the menu pause menu pop up when the application menu button is hit
        //if (Controller.GetPressDown (SteamVR_Controller.ButtonMask.ApplicationMenu)) {
        //	pauseMenu.SetActive (true);
        //	Time.timeScale = 0.0f;
        //}
    }

    //Showing the laser in general
    private void ShowLaser() { 

        // Show the laser
        laser.SetActive(true);
        laserTransform.parent = transform;

        laserTransform.localPosition= holdPosition;
        laserTransform.localEulerAngles = holdRotation;

        // Position the laser object (which is actually a rectangular prism) in the forward direction
        //laserTransform.position = transform.forward;
        // Point the laser in the forward direction
        //laserTransform.LookAt(transform.forward); 
        // Scale the laser appropriately. Currently set to an arbitrary number
        //laserTransform.localScale = Vector3.one * 10;
    }

    //Showing the laser when it hits a datapoint
    private void ShowLaser(RaycastHit hit) {

		hitPoint = hit.point;
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
	}
}
