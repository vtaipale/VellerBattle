using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dice Rolling and ubitous Tech Levels.
/// </summary>
public class TravellerBehaviour : MonoBehaviour {

	/// <summary>
	/// Tech Level
	/// </summary>
	public int TL = 12;


	public int d6(int HowMany)
	{
		int returnoitava = 0;

		for (int i = HowMany; i >= 1; i--  )
			returnoitava += Mathf.RoundToInt(Random.Range(1f,6f));

		return returnoitava;
	}

}
