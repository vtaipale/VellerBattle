using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Leaks fuel from starship every round
/// </summary>
public class FuelSeriousLeak : FuelLeak
{
    // Start is called before the first frame update
    void Start()
    {
        Myship = GetComponent<Spaceship>();
        
        Myship.UpdateBattleLog(" Serious Fuel leak!");
        NextRoundToLeak = GetCurrentRound() + 1;

        //Debug.Log(Myship.name + " is leaking fuel! ");
    }
    public override void LeakFuelCheck()
    {
        if (GetCurrentRound() == NextRoundToLeak)
        { 
            Myship.UpdateBattleLog(" Leaking fuel fast!");

            Myship.FuelChange(d6(1) * -1);
            NextRoundToLeak = GetCurrentRound() + 1;
            
        }
    }
}
