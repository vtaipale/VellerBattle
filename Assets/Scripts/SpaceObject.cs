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

    public string RangeBandToString(Transform Target)
    {
        int DistanceMath = this.DistanceTo(Target);

        if (this.transform.position == Target.position)
            return "Adjacent";
        else if (DistanceMath < RangeB_Close)
            return "Close";
        else if (DistanceMath < RangeB_Short)
            return "Short";
        else if (DistanceMath < RangeB_Medium)
            return "Medium";
        else if (DistanceMath < RangeB_Long)
            return "Long";
        else if (DistanceMath < RangeB_VLong)
            return "Very Long";
        else if (DistanceMath < RangeB_Distant)
            return "Distant";
        else if (DistanceMath < RangeB_VDistant)
            return "Very Distant";

        return "Far";
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

		if (amount >= this.DistanceTo (Target)) {
            
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
