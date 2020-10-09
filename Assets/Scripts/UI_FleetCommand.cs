using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A bridge betveen Fleet and UI commands to fleet.
/// </summary>
public class UI_FleetCommand : MonoBehaviour
{

    public Fleet MyFleet;

    // Start is called before the first frame update
    void Start()
    {
        if (MyFleet == null)
            Debug.LogError("MYFLEET IS NULL!");
    }

    public void FleetMoveOrderAll(GameObject DestinationPoint)
    {
        MyFleet.MoveOrderAll(DestinationPoint);
    }

    public void FleetMoveOrderAll(Vector3 Destination)
    {
        MyFleet.MoveOrderAll(Destination);
    }
}
