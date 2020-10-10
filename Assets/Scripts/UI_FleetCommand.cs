using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A bridge betveen Fleet and UI commands to fleet.
/// </summary>
public class UI_FleetCommand : MonoBehaviour
{
    public Fleet MyFleet;
    public UI_FleetScanner MyFleetScanner;
    public Text FleetText;

    void Awake()
    {
        if (MyFleet == null)
            Debug.LogError("MYFLEET IS NULL!");

        MyFleetScanner = FindObjectOfType<UI_FleetScanner>();

        if (MyFleetScanner == null)
            Debug.LogError("MYFLEETScanner IS NULL!");

        if (FleetText == null)
            Debug.LogError("FleetText IS NULL!");
    }

    // Start is called before the first frame update
    void Start()
    {
       
        this.SelectOtherFleet(this.MyFleet);

    }

    void Update()
    {

        FleetText.text = MyFleet.OfficialName + " | COM: " + MyFleet.Leader.CaptainName + "\n";
        FleetText.text += "SHIPS: " + MyFleet.GetMyCurrentShips().Length + "/" + MyFleet.MyShips.Length + "\n";
        FleetText.text += "ALARM: " + MyFleet.CurrentAlarm + "\n";
        FleetText.text += "ORDER: " + MyFleet.CurrentOrder;

        if (MyFleet.CurrentOrder == "Move")
            FleetText.text += " " + MyFleet.Destination;
        else if (MyFleet.CurrentOrder == "Engage" && (MyFleet.MyEnemies != null))
            FleetText.text += " " + MyFleet.MyEnemies.OfficialName;


        //FLEETCHANGERS
        if (Input.GetKeyUp(KeyCode.Alpha1)) 
        {
            Fleet[] Fleets = FindObjectsOfType<Fleet>();

            if (Fleets.Length>=1)
            {
                this.SelectOtherFleet(Fleets[0]);
            }
        }
       else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            Fleet[] Fleets = FindObjectsOfType<Fleet>();

            if (Fleets.Length >= 2)
            {
                this.SelectOtherFleet(Fleets[1]);
            }
            else
            {
                Debug.Log("Tried to Change to Fleet Number 2!");
            }
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            Fleet[] Fleets = FindObjectsOfType<Fleet>();

            if (Fleets.Length >= 3)
            {
                this.SelectOtherFleet(Fleets[2]);
            }
            else
            {
                Debug.Log("Tried to Change to Fleet Number 2!");
            }
        }
        else if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            Fleet[] Fleets = FindObjectsOfType<Fleet>();

            if (Fleets.Length >= 4)
            {
                this.SelectOtherFleet(Fleets[3]);
            }
            else
            {
                Debug.Log("Tried to Change to Fleet Number 4!");
            }
        }
    }


    public void SelectOtherFleet(Fleet NuFleet) {

        this.MyFleet = NuFleet;
        FleetCamToFollowLeader();
        MyFleetScanner.CurrentFleet = MyFleet;
        MyFleetScanner.MovementSelecting(false);
        MyFleetScanner.SetRangePlanesVisible(false);
    }

    public void FleetCamToFollowLeader()
    {
        MyFleet.CamToFollowLeader(0);
    }

    public void FleetMoveOrderAll(GameObject DestinationPoint)
    {
        MyFleet.MoveOrderAll(DestinationPoint);
    }

    public void FleetMoveOrderAll(Vector3 Destination)
    {
        MyFleet.MoveOrderAll(Destination);
    }

    public void FleetOrder(string NuOrder)
    {
        MyFleet.OrderAll(NuOrder);
    }
    public void FleetAlarm(string NuAlarm)
    {
        MyFleet.AlarmAll(NuAlarm);
    }
    public void FleetScanForEnemyFleets()
    {
        MyFleet.ScanForEnemyFleets();
    }
}
