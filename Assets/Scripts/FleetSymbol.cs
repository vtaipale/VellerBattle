using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FleetSymbol : UI_Symbol {

	public Fleet MyFleet;

	// Use this for initialization


	void Start () {
		this.MyFleet = GetComponentInParent<Fleet> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		this.transform.parent.SetPositionAndRotation(MyFleet.Leader.transform.position,new Quaternion(0f,0f,0f,0f)); //notgood

		this.LookAtCamera ();

	}
}
