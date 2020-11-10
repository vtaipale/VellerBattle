using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UI Symbol that moves along a fleet
/// </summary>
public class FleetSymbol : UI_Symbol {

	public Fleet MyFleet;

	// Use this for initialization
    void Start () {
		this.MyFleet = GetComponentInParent<Fleet> ();
	}

    // Update is called once per frame
    void FixedUpdate() {


        if (MyScanner.CurrentFleet.IsVisible(MyFleet))
        { 
            this.transform.parent.SetPositionAndRotation(MyFleet.Leader.transform.position, new Quaternion(0f, 0f, 0f, 0f)); //notgood

            this.LookAtCamera();
        }
        else
            this.transform.localScale = new Vector3(0, 0, 0);


        

	}
}
