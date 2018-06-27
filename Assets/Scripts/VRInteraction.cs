using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//@source https://www.raywenderlich.com/149239/htc-vive-tutorial-unity

/* VR Adaption for HTC Vive. Controls as follows:
 * TouchPad + Grip = Move
 * Trigger = Select Point
 * Application Menu = Opens up UI Menu
*/

public class VRInteraction : MonoBehaviour {


    //The majority of these variables are for use in the plot scene. Many of them will be left as none at the main menu
    
    // These variables will be used in all scenes
    public LayerMask UIMask;                            // Mask to make sure laser can hit UI elements

    public Vector3 holdRotation;                        // Vector3 variables to ensure that laser is positioned correctly according to controller
    public Vector3 holdPosition;

    //How fast the user moves
    public float speed;

    //Keep track of laser prefab
    public GameObject laserPrefab;

    // Instantiated laser gameobject and transform
    private GameObject laser;
    private Transform laserTransform;
    // Where the laser should hit
    private Vector3 hitPoint;

    private SteamVR_TrackedObject trackedObj;
    // Keeps track of controllers. Also which controller (L/R)
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    //PauseMenu
    public GameObject pauseMenu;

    //Boolean to keep track of whether or not pauseMenu is up
    public static bool menuUp;

    //These variables will be used in only the mainmenu scene

    //Track the initial x position of the controller when on slider

    private float initialXPos;
    //Track whether or not the controller is on the slider
    private bool onSlider;

    //CoRoutine to track slider values
    private Coroutine sliderValue;

    // These variables will be used primarily in only the plot scene
    // Mask to make sure that the raycast hits dataPoints
    public LayerMask dataPointMask;

    //Keep track of user that needs to move
    public GameObject user;

    //Text to show datapoint selected
    private TextMesh dataText;

    //public GameObject pointText;
    private static int leftMostIndex;
    private static int rightMostIndex;

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

    //Create the laser
	void Start() {
        dataText = GetComponentInChildren<TextMesh>();
		laser = Instantiate (laserPrefab);
		laserTransform = laser.transform;
	}
	
	// Update is called once per frame
	// Get input from controller in order to move player
	void Update () {

        //Reset the laser to the normal size
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y,
            1);

        //Get input to move on xz plane (horizontally) from touchPad
        touchAxis = Controller.GetAxis ();
		if (touchAxis != Vector2.zero) {
			moveAxis = new Vector3 (touchAxis.x, 0, touchAxis.y);
		} else {
			moveAxis = new Vector3 (0, 0, 0);
		}

        //Use input to move user on xz plane if user clicks touchPad button
        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            user.transform.position += moveAxis * speed;
        }


		//Get input to move on yx/z plane (vertically) Left grip button moves player downward. Right grip button moves player upward
		if (Controller.GetPressDown (SteamVR_Controller.ButtonMask.Grip) && (int)trackedObj.index == leftMostIndex) {
            user.transform.position -= new Vector3(0, 1, 0) * speed;
		} else if (Controller.GetPressDown (SteamVR_Controller.ButtonMask.Grip) && (int)trackedObj.index == rightMostIndex) {
            user.transform.position += new Vector3(0, 1, 0) * speed;
        }

        //Laser interaction
        // 1) Have the laser pointer point to a datapoint and print out datapoint name
        // 2) Have the laser pointer to a UI element and allow user interaction
        // 3) Otherwise, just show the normal laser

        RaycastHit hit;
        //This allows laser to point out various data points
        if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100, dataPointMask))
        {
            ShowLaser(hit);
            dataText.text = hit.collider.name;
        }
        else if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 100, UIMask))
        { //This allows user to hit various buttons and interact with a slider
            ShowLaser(hit);
            var btn = hit.collider.GetComponentInParent<Button>();
            if (btn != null && Controller.GetHairTriggerDown())
            {
                btn.onClick.Invoke();
            }
            var slider = hit.collider.GetComponentInParent<Slider>();
            if (slider != null && Controller.GetHairTrigger())
            {
                if (!onSlider)
                {
                    onSlider = true;
                    initialXPos = trackedObj.transform.position.x;
                    Debug.Log("initialXPos: " + initialXPos);
                }
                slider.value = (-(trackedObj.transform.position.x) + initialXPos) * 500;


                if (Controller.GetHairTriggerUp())
                {
                    onSlider = false;
                }
            }
            dataText.text = "";
        }
        else
        {
            ShowLaser();
            dataText.text = "";
            onSlider = false;
        }

        //Have the pause menu pop up when the application menu button is hit
        //Resumes the game when application menu hit again
        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            if (!menuUp)
            {
                menuUp = true;
                pauseMenu.SetActive(true);
                Time.timeScale = 0.0f;
            }
            else
            {
                menuUp = false;
                pauseMenu.SetActive(false);
                Time.timeScale = 1.0f;
            }
        }
    }

    //Showing the laser in general
    private void ShowLaser() { 

        // Show the laser and make move with controller
        laser.SetActive(true);
        laserTransform.parent = transform;

        // Set the laser's position and rotation so that it matches the controller's
        laserTransform.localPosition= holdPosition;
        laserTransform.localEulerAngles = holdRotation;
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
