using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MiniReport : MonoBehaviour {

	public Fleet MyFleet;

	private float update = 0f;
	public float Gamespeed = 1.0f;

	// Use this for initialization
	void Start () {
		if (MyFleet == null)
			Debug.LogWarning ("UiREPORTER LACKS FLEETY!");
	}
	
	// Update is called once per frame
	void Update () {
		update += Time.deltaTime;
		if (update > Gamespeed) {
		
			GetComponent<Text>().text = MyFleet.GetMiniReports () +"\n";

			update = 0.0f;
		}
	}
}
