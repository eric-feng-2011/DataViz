using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Add VR adaption - UI/Menu pop-up. Select points using laser. Fly in 3D (motion sickness)
public class VRInteraction : MonoBehaviour {

	public GameObject user;

	private SteamVR_TrackedObject trackedObj;
	// Keeps track of controllers. Also which controller (L/R)
	private SteamVR_Controller.Device Controller
	{
		get { return SteamVR_Controller.Input((int)trackedObj.index); }
	}

	private Vector2 touchAxis;
	private Vector3 moveAxis;

	void Awake() {
		trackedObj = GetComponent<SteamVR_TrackedObject>();
	}
	
	// Update is called once per frame
	void Update () {
		touchAxis = Controller.GetAxis ();
		if (touchAxis != Vector2.zero) {
			moveAxis = new Vector3 (touchAxis.x, 0, touchAxis.y);
			user.GetComponent<Rigidbody> ().velocity = moveAxis;
		} else {
			user.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		}

		if (Controller.GetPressDown (SteamVR_Controller.ButtonMask.Grip)) {
			
		}
	}
}
