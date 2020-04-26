using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class UI_FleetScanner : MonoBehaviour {

	public Fleet[] AllFleets;

	public Canvas TheCanvas;
	public GraphicRaycaster TheRaycaster;

	public RawImage[] FleetImages;

	EventSystem m_EventSystem;



	// Use this for initialization
	void Start () {
		AllFleets = FindObjectsOfType<Fleet> ();
		TheCanvas = FindObjectOfType<Canvas> ();
		TheRaycaster = TheCanvas.GetComponent<GraphicRaycaster> ();

		//Fetch the current EventSystem. Make sure your Scene has one.
		m_EventSystem = EventSystem.current;
	}
	
	// Update is called once per frame
	void Update () {
		
		Camera TheCamera = FindObjectOfType<Camera> ();
		//Camera TheCamera = TheRaycaster.eventCamera;

		RaycastHit hit;
		/*Ray ray = camera.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out hit)) {
			Transform objectHit = hit.transform;

			if (objectHit.GetComponent<FleetSymbol>() != null)
			{
				Debug.Log ("JUUJAAAA"); 
			}
		}*/
	}

}
