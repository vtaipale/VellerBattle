using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class UI_FleetScanner : MonoBehaviour {

	public Fleet[] AllFleets;
    public Camera TheCamera;
	public Canvas TheCanvas;
	public GraphicRaycaster TheRaycaster;

	public RawImage[] FleetImages;

	EventSystem m_EventSystem;

    public float m_DistanceZ;
    Plane m_Plane;

    // Use this for initialization
    void Start () {
		AllFleets = FindObjectsOfType<Fleet> ();
		//TheCanvas = FindObjectOfType<Canvas> ();
		//TheRaycaster = TheCanvas.GetComponent<GraphicRaycaster> ();

		//Fetch the current EventSystem. Make sure your Scene has one.
		m_EventSystem = EventSystem.current;

        //Create a new plane with normal (0,0,1) at the position away from the camera you define in the Inspector. This is the plane that you can click so make sure it is reachable.
        //m_Plane = new Plane(Vector3.forward, 7);
        m_Plane = new Plane(AllFleets[0].Leader.transform.up, 1f);

    }
	
	// Update is called once per frame
	void Update () {
		
		//Camera TheCamera = FindObjectOfType<Camera> ();
		//Camera TheCamera = TheRaycaster.eventCamera;

		

        //Detect when there is a mouse click
        if (Input.GetMouseButton(0))
        {
            //Create a ray from the Mouse click position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //Initialise the enter variable
            float enter = 0.0f;

            if (m_Plane.Raycast(ray, out enter))
            {
                //Get the point that is clicked
                Vector3 hitPoint = ray.GetPoint(enter);
                Debug.Log("OOO HIT");

                Debug.DrawLine(AllFleets[0].Leader.transform.position, hitPoint, Color.blue);

                //Assing movement order
                //TODO: vertical boost!

                AllFleets[0].MoveOrderAll(hitPoint);
            }
        }
        else
        {
            RaycastHit hit;
            Ray ray = TheCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 5))
            {
                Transform objectHit = hit.transform;

                Debug.Log("JUUJAAAA" + objectHit.name);

                if (objectHit.GetComponent<FleetSymbol>() != null)
                {
                    Debug.Log("UUJEE");
                }
            }
        }

    }

}
