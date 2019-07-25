using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Anything actually in space. 
/// Ships and missiles now
/// </summary>
public class SpaceObject : TravellerBehaviour {

	public virtual void GameTurn(int turnNumber)
	{
	}

	/// <summary>
	/// Gimme distance to other SpaceObject!
	/// </summary>
	/// <returns>The to.</returns>
	/// <param name="target">Target.</param>
	public int DistanceTo(SpaceObject target)
	{
		return Mathf.RoundToInt( Vector3.Distance (this.transform.position, target.transform.position) ) ;

	}
}
