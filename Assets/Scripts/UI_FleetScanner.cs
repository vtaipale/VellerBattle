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

    public Fleet CurrentFleet;

    public bool MovementSelection = false;
    public LineRenderer MovLine;
    public GameObject MovPlane;

    public bool RangePlanesVisible = false;
    public GameObject RangePlanes;

    public RawImage[] FleetImages;

	EventSystem m_EventSystem;

    private float temptime;

    public float m_DistanceZ;
    Plane m_Plane;

    // Use this for initialization
    void Start () {

        MovLine.gameObject.SetActive(false);

		AllFleets = FindObjectsOfType<Fleet> ();
        //TheCanvas = FindObjectOfType<Canvas> ();
        //TheRaycaster = TheCanvas.GetComponent<GraphicRaycaster> ();

        CurrentFleet = AllFleets[0];

		//Fetch the current EventSystem. Make sure your Scene has one.
		m_EventSystem = EventSystem.current;


        //Create a new plane with normal (0,0,1) at the position away from the camera you define in the Inspector. This is the plane that you can click so make sure it is reachable.
        //m_Plane = new Plane(Vector3.forward, 7);
        m_Plane = new Plane(Vector3.up, this.gameObject.transform.position);
        MovPlane.SetActive(false);
        RangePlanes.SetActive(false);
    }

    // Update is called once per frame
    void Update () {

        //Camera TheCamera = FindObjectOfType<Camera> ();
        //Camera TheCamera = TheRaycaster.eventCamera;

        if (RangePlanesVisible == true)
        {
            RangePlanes.transform.position = this.CurrentFleet.Leader.transform.position;
        }

        if (Input.GetKeyUp(KeyCode.Space)) // SCANNER ONLINE
        {
            this.SetRangePlanesVisible(!RangePlanesVisible);
        }

        if (MovementSelection == true)
        {
            if (Input.GetKeyUp(KeyCode.Escape) | Input.GetMouseButtonDown(1))
            {
                MovementSelecting(false);
            }
            else
            {
                //Create a ray from the Mouse
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                //Initialise the enter variable
                float enter = 0.0f;

                m_Plane.SetNormalAndPosition(Vector3.up, this.CurrentFleet.Leader.transform.position);
                MovPlane.transform.position = this.CurrentFleet.Leader.transform.position;

                //Debug.Log("mplane : " + m_Plane);

                if (m_Plane.Raycast(ray, out enter))
                {

                    //Get the point that is clicked
                    Vector3 hitPoint = ray.GetPoint(enter);
                    //Debug.Log("OOO HIT");

                    //HelpoerLine
                    List<Vector3> pos = new List<Vector3>();
                    pos.Add(CurrentFleet.Leader.transform.position);
                    pos.Add(hitPoint);

                    MovLine.SetPositions(pos.ToArray());

                    //resizing the line

                    UI_Mouselook Looky = FindObjectOfType<UI_Mouselook>();

                    float NevSize = Looky.GetZoomLevel();

                   
                    MovLine.startWidth = NevSize/50;
                    MovLine.endWidth = NevSize/50;

                    float CircleScaler = Vector3.Distance(CurrentFleet.Leader.transform.position, hitPoint) / 4.85f;

                    MovPlane.transform.localScale = new Vector3(CircleScaler, CircleScaler, CircleScaler);

                    //Assing movement order
                    //TODO: vertical boost!
                    if (Input.GetMouseButtonUp(0))
                    {
                        CurrentFleet.MoveOrderAll(hitPoint);
                        MovementSelecting(false);
                    }
                }
            }
        }

        //Start a MOVEMENT change if short click, otherise just drag camera
        else if (Input.GetMouseButtonDown(1))
        {
            temptime = Time.time;
            
        }
        else if (Input.GetMouseButtonUp(1) && ((Time.time - temptime) < 0.25)) 
        {
            MovementSelecting(true);
        }
        /*
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
        */

    }

    private void MovementSelecting(bool yesno)
    {
       Debug.Log("MOVEMENTSELECTING " + yesno);

       MovementSelection = yesno;
       MovLine.gameObject.SetActive(yesno);
       MovPlane.SetActive(yesno);
    }

    private void SetRangePlanesVisible(bool yesno)
    {
        Debug.Log("RangeShov " + yesno);

        RangePlanesVisible = yesno;
        RangePlanes.SetActive(yesno);
    }

}
