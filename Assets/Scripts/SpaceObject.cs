using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Anything actually in space. 
/// Ships and missiles now
/// </summary>
public class SpaceObject : TravellerBehaviour {

	public int Thrust = 1;
	public int Initiative = 0;

    //SPACEBAND RANGES
    //GLOBALLY: 1 Unity Unit = 10 KM Traveller Space
    private static int RangeB_Adjacent  = 0;
    private static int RangeB_Close     = 1;
    private static int RangeB_Short     = 125;
    private static int RangeB_Medium    = 1000;
    private static int RangeB_Long      = 2500;
    private static int RangeB_VLong     = 5000;
    private static int RangeB_Distant   = 30000;
    private static int RangeB_VDistant  = 500000;   //edge of anything sane battlethingys


    public virtual void GameTurn(int turnNumber)
	{
	}

	/// <summary>
	/// Gimme distance to other SpaceObject!
	/// </summary>
	/// <returns>The distance to the Target.</returns>
	/// <param name="target">OtherObject</param>
	public int DistanceTo(SpaceObject Target)
	{
		return this.DistanceTo(Target.transform.position);
	}

	public int DistanceTo(Transform Target)
	{
		return this.DistanceTo(Target.position);
	}

	public int DistanceTo(Vector3 target)
	{
		return Mathf.RoundToInt( Vector3.Distance (this.transform.position, target) ) ;

	}

	/// <summary>
	/// Moves tovards a destination. 
    /// Returns FALSE if moving, TRUE if the destination reached.
	/// </summary>
	/// <param name="amount">Amount to move.</param>
	/// <param name="Target">Destination to move.</param>
	public bool Move(int amount, Vector3 Target)
	{
		this.transform.LookAt (Target);

		if (amount > this.Thrust)
			amount = this.Thrust;

        amount = amount * 130; //The Space ratio 1297 km/round /10 to get to unity scale

		if (amount > this.DistanceTo (Target)) {
            
			this.transform.position = Target + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)); // a bit of scatter
            return true;
		}
			
		this.transform.Translate (this.transform.forward * amount,Space.World);

		return false; //stull moving
	}

    public bool MoveForward(int amount)
    {
        return this.Move(amount, this.transform.position + this.transform.forward * this.Thrust * 1300);  
    }
}
