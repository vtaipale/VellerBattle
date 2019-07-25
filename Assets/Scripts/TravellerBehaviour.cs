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

	/// <summary>
	/// Roll X regular dice, return sum .
	/// </summary>
	/// <param name="HowMany">X</param>
	public int d6(int HowMany)
	{
		int returnoitava = 0;

		for (int i = HowMany; i >= 1; i--  )
			returnoitava += Mathf.RoundToInt(Random.Range(1f,6f));

		return returnoitava;
	}

	/// <summary>
	/// Roll X+1 regular dice, drop lowest.
	/// </summary>
	/// <param name="HowMany">X</param>
	public int d6boon(int HowMany)
	{
		int returnoitava = 0;
		int pieninheitto = 10;

		for (int i = HowMany+1; i >= 1; i--) {
			int noppaheitto =  Mathf.RoundToInt (Random.Range (1f, 6f));
			returnoitava += noppaheitto;
			if (noppaheitto < pieninheitto)
				pieninheitto = noppaheitto;
		}
		returnoitava -= pieninheitto;

		return returnoitava;
	}

	/// <summary>
	/// Roll X+1 regular dice, drop highest.
	/// </summary>
	/// <param name="HowMany">X</param>
	public int d6bane(int HowMany)
	{
		int returnoitava = 0;
		int suurinheitto = 0;

		for (int i = HowMany+1; i >= 1; i--) {
			int noppaheitto =  Mathf.RoundToInt (Random.Range (1f, 6f));
			returnoitava += noppaheitto;
			if (noppaheitto > suurinheitto)
				suurinheitto = noppaheitto;
		}
		returnoitava -= suurinheitto;

		return returnoitava;
	}
}
