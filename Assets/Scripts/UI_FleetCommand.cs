using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A bridge betveen Fleet and UI commands to fleet.
/// </summary>
public class UI_FleetCommand : MonoBehaviour
{

    public Fleet MyFleet;
    public UI_FleetScanner MyFleetScanner;

    // Start is called before the first frame update
    void Start()
    {
        if (MyFleet == null)
            Debug.LogError("MYFLEET IS NULL!");

        MyFleetScanner = FindObjectOfType<UI_FleetScanner>();

        if (MyFleetScanner == null)
            Debug.LogError("MYFLEETScanner IS NULL!");
    }
    void Awake()
    {
        this.SelectOtherFleet(this.MyFleet);
    }

    public void SelectOtherFleet(Fleet NuFleet) {

        MyFleet = NuFleet;
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
