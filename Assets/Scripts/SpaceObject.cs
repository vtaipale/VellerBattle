using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Anything actually in space. 
/// Ships and missiles now
/// </summary>
public class SpaceObject : TravellerBehaviour {

	public int Thrust = 1;

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
	/// Moves tovards the Direction. Returns FALSE if moving, TRUE if already there.
	/// </summary>
	/// <param name="amount">Amount to move.</param>
	/// <param name="Target">.</param>
	public bool Move(int amount, Vector3 Target)
	{
		this.transform.LookAt (Target);

		if (amount > this.Thrust)
			amount = this.Thrust;

		if (amount > this.DistanceTo (Target)) {
			this.transform.position = Target;
			return true;
		}
			
		this.transform.Translate (this.transform.forward * amount,Space.World);

		return false; //stull moving
	}
}
