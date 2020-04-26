using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Symbol : MonoBehaviour {

	public int ScaleFactor = 100;
	public int ShowStart = 50;

	public bool DoNotTrack = false;

	// Update is called once per frame
	void FixedUpdate () {

		this.LookAtCamera ();

	}

	virtual public void LookAtCamera()
	{
		float currentDistance = Vector3.Distance (this.transform.position, FindObjectOfType<Camera>().transform.position) ;

		if (currentDistance > ShowStart) {
			currentDistance = currentDistance / ScaleFactor ;
			this.transform.localScale = new Vector3 (currentDistance, currentDistance, currentDistance);
		}
		else if (currentDistance > (ShowStart/2)) {
			currentDistance = currentDistance*2 / ScaleFactor ;
			this.transform.localScale = new Vector3 (currentDistance, currentDistance, currentDistance);
		}
		else
			this.transform.localScale = new Vector3 (0f,0f,0f);

		if (DoNotTrack == false) {
			Camera TheCamera = FindObjectOfType<Camera> ();
			this.transform.parent.transform.LookAt (TheCamera.transform.position);
			//this.transform.LookAt (this.transform.up);
		}

	}
}
