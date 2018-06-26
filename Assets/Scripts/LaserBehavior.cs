using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaserBehavior : MonoBehaviour {

    //Keep track of UI Layer
    //public LayerMask UILayer;
    //private int UILayerInt;

    //These materials will highlight and unhighlight buttons respectively
    public Material highlight;
    public Material transparent;

    //Private button variable
    private Button btn;

    void Start()
    {
        //UILayerInt = UILayer.value;
    }

    private void OnTriggerStay(Collider other)
    {
        btn = other.GetComponentInParent<Button>();
        Debug.Log("Trigger Name: " + other.name);
        if (btn != null)
        {
            Debug.Log("Enter Name: " + other.name);
            other.gameObject.GetComponent<Renderer>().material = highlight;
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (btn)
    //    {
    //        Debug.Log("Exit Name: " + other.name);
    //        btn = null;
    //        other.gameObject.GetComponent<Renderer>().material = transparent;
    //    }
    //}
}
