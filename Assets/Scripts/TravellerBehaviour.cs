using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dice Rolling and ubitous Tech Levels.
/// </summary>
public class TravellerBehaviour : MonoBehaviour {


    //SPACEBAND RANGES
    //GLOBALLY: 1 Unity Unit = 10 KM Traveller Space
    public static int RangeB_Adjacent = 0;
    public static int RangeB_Close = 1;
    public static int RangeB_Short = 125;
    public static int RangeB_Medium = 1000;
    public static int RangeB_Long = 2500;
    public static int RangeB_VLong = 5000;
    public static int RangeB_Distant = 30000;
    public static int RangeB_VDistant = 500000;   //edge of anything sane battlethingys

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

    /// <summary>
    /// Roll X three sided dice, return sum.
    /// </summary>
    /// <param name="HowMany">X</param>
    public int d3(int HowMany)
    {
        int returnoitava = 0;

        for (int i = HowMany; i >= 1; i--)
            returnoitava += Mathf.RoundToInt(Random.Range(1f, 3f));

        return returnoitava;
    }

    /// <summary>
    /// Asks GameFlowController what current round is.
    /// </summary>
    /// <returns>Current Round</returns>
    public int GetCurrentRound()
    {
        return FindObjectOfType<GameFlowController>().GetRoundNumber();
    }
    /// <summary>
    /// Asks GameFlowController what current round is as String.
    /// </summary>
    /// <returns>R-Current Round</returns>
    public string GetCurrentRoundString()
    {
        return ("R-" + FindObjectOfType<GameFlowController>().GetRoundNumber());
    }
}
