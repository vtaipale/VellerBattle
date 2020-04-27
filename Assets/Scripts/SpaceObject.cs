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
	public int DistanceTo(SpaceObject target)
	{
		return Mathf.RoundToInt( Vector3.Distance (this.transform.position, target.transform.position) ) ;

	}

	public int DistanceTo(Vector3 target)
	{
		return Mathf.RoundToInt( Vector3.Distance (this.transform.position, target) ) ;

	}

	/// <summary>
	/// Moves towards direction.
	/// </summary>
	public void Move(int amount, Vector3 direction)
	{
		this.transform.LookAt (direction);

		if (amount > this.Thrust)
			amount = this.Thrust;

		if (amount > this.DistanceTo (direction))
			this.transform.position = direction;
		else			
			this.transform.Translate (this.transform.forward * amount,Space.World);
	}
}
