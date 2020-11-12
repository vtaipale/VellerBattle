using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Leaks fuel from starship every 10 rounds.
/// </summary>
public class FuelLeak : TravellerBehaviour
{
    public Spaceship Myship;
    public int NextRoundToLeak = 0;

    // Start is called before the first frame update
    void Start()
    {
        Myship = GetComponent<Spaceship>();

        Myship.UpdateBattleLog(" Fuel leak!");
        NextRoundToLeak = GetCurrentRound() + 10;
        
        //Debug.Log(Myship.name + " is leaking fuel! ");
    }

    public virtual void LeakFuelCheck()
    {
        if (GetCurrentRound() == NextRoundToLeak)
        {
            
                Myship.UpdateBattleLog(" Leaking Fuel!");
                Myship.FuelChange( d6(1) * -1);
                NextRoundToLeak = GetCurrentRound() + 10;
        }
    }
}
