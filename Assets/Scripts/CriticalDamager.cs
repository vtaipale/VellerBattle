using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Takes a ship and Crit Damages it.
/// </summary>
public class CriticalDamager : TravellerBehaviour
{
    private Spaceship ShipToDamage;

    public CriticalDamager(Spaceship ShipToDamageInject)
    {
        ShipToDamage = ShipToDamageInject;
    }

    /// <summary>
    /// Actual crit damage done here
    /// </summary>
    /// <param name="DamageType">Subsystem critted</param>
    /// <param name="damagetext">Used only for crits</param>
    /// <param name="NuSeverity">Severity to add</param>
    private void CritDamage(string DamageType, string damagetext, int NuSeverity)
    {
        int CurrentSeverity = ShipToDamage.GetCurrentCritSeverity(DamageType);

        //Debug.Log(ShipToDamage.name + " taking " + DamageType + " " + (CurrentSeverity + 1) + " crit!");

        if (CurrentSeverity >= 6)
        {

            if ((ShipToDamage.gameObject.activeSelf == true) && (ShipToDamage.Hullpoints > 0)) //no eternal annihilation chains
                ShipToDamage.Damage(d6(6), "catastrophic hull rupture", true);
            ShipToDamage.MyCritDamages.Add("Rupture");
            
        }
        else
        {
            if (NuSeverity <= 0)
                NuSeverity = CurrentSeverity + 1;

            string ThisDamage = (DamageType + NuSeverity);

            ShipToDamage.MyCritDamages.Add(ThisDamage);



            switch (DamageType)
            {
                case "Sensors":
                    this.Crit_Sensors(NuSeverity);
                    break;
                case "PowerPlant":
                    this.Crit_PPlant(NuSeverity);
                    break;
                case "Fuel":
                    this.Crit_Fuel(NuSeverity);
                    break;
                case "Weapon":
                    this.Crit_Weapon(NuSeverity);
                    break;
                case "Armour":
                    this.Crit_Armour(NuSeverity);
                    break;
                case "Hull":
                    this.Crit_Hull(NuSeverity, damagetext);
                    break;
                case "M-Drive":
                    this.Crit_MDrive(NuSeverity);
                    break;
                case "Cargo":
                    break;
                case "J-Drive":
                    this.Crit_JDrive(NuSeverity);
                    break;
                case "Crew":
                    this.Crit_Crew(NuSeverity);
                    break;
                case "Computer":
                    this.Crit_Computer(NuSeverity);
                    break;
                default:
                    Debug.LogError("UNDEFINED DAMAGE TYPE");
                    break;
            }
        }
    }
    /// <summary>
    /// Actual crit damage done here. Severity increased by one.
    /// </summary>
    /// <param name="DamageType">Subsystem critted</param>
    /// <param name="damagetext">Used only for crits</param>
    private void CritDamage(string DamageType, string damagetext)
    {
        this.CritDamage(DamageType, damagetext, 0);
    }

    /// <summary>
    /// Actual Critical Damage is done here.
    /// </summary>
    /// <param name="DamageType">Subsystem critted</param>
    public void CritDamage(string DamageType)
    {
        this.CritDamage(DamageType, "");
    }

    public void Crit_Hull(int Severity, string rupturetext) //some of these crits are easy at least...
    {
        Severity = Mathf.Min(Severity, 6); //Max damage is 6

        if (rupturetext == "")
            ShipToDamage.Damage(d6(Severity), "critical hull rupture", true); 

        else
            ShipToDamage.Damage(d6(Severity), rupturetext, true);
    }

    public void Crit_Sensors(int Severity) //THESE ARE LETHAL FOR FLEETLEADER
    {
        switch (Severity)
        {
            case 1:
                ShipToDamage.Sensors -= 2;
                ShipToDamage.UpdateBattleLog(" Sensors damaged!");
                break;
            case 2:
                ShipToDamage.MaxSensorRange = RangeB_Medium;
                ShipToDamage.UpdateBattleLog(" Sensors damaged: Can see only up to Medium Range!");
                break;
            case 3:
                ShipToDamage.MaxSensorRange = RangeB_Short;
                ShipToDamage.UpdateBattleLog(" Sensors damaged: Can see only up to Short Range!!");
                break;
            case 4:
                ShipToDamage.MaxSensorRange = RangeB_Close;
                ShipToDamage.UpdateBattleLog(" Sensors damaged: Can see only up to Close Range!!!");
                ShipToDamage.SurrenderCheck();
                break;
            case 5:
                ShipToDamage.MaxSensorRange = RangeB_Adjacent;
                ShipToDamage.UpdateBattleLog(" Sensors disabled: Can see only out of the windows!!!");
                ShipToDamage.Surrender(); //honorary thing to do at these points...
                break;
            case 6:
                ShipToDamage.MaxSensorRange = RangeB_Adjacent;
                ShipToDamage.UpdateBattleLog(" Sensors are gone...");
                break;
            default:
                Debug.LogError("UNDEFINED SEVERITY");
                break;
        }
        
    }

    public void Crit_Fuel(int Severity) 
    {
        switch (Severity)
        {
            case 1:
                //ShipToDamage.UpdateBattleLog(" Fuel Leak!");
                ShipToDamage.gameObject.AddComponent<FuelLeak>();
                break;
            case 2:
                //ShipToDamage.UpdateBattleLog(" Serious Fuel Leak!");
                ShipToDamage.gameObject.AddComponent<FuelSeriousLeak>();
                break;
            case 3:
                ShipToDamage.UpdateBattleLog(" Significant fuel leak!");
                ShipToDamage.FuelChange(ShipToDamage.FuelOrig/10*-d6(1));
                break;
            case 4:
                ShipToDamage.UpdateBattleLog(" Fuel tank destroyed!");
                ShipToDamage.FuelChange(-99999);
                break;
            case 5:
                CritDamage("Hull", "fuel tank breach");
                break;
            case 6:
                CritDamage("Hull", "major fuel tank breach",d6(1));
                break;
            default:
                Debug.LogError("UNDEFINED SEVERITY");
                break;
        }

    }
    public void Crit_Weapon(int Severity)
    {
        bool DoesShipHaveGuns = (ShipToDamage.MyGuns.Length > 0);

        if (DoesShipHaveGuns == true)
        {
            switch (Severity)
            {
                case 1:
                    Shipweapon ToDamage = ShipToDamage.MyGuns[Mathf.RoundToInt(Random.Range(0, ShipToDamage.MyGuns.Length - 1))];
                    ToDamage.Damaged = true;
                    ShipToDamage.UpdateBattleLog(" " + ToDamage.name + " damaged!");
                    break;
                case 2:
                    Shipweapon ToDisable = ShipToDamage.MyGuns[Mathf.RoundToInt(Random.Range(0, ShipToDamage.MyGuns.Length - 1))];
                    ShipToDamage.UpdateBattleLog(" " + ToDisable.name + " is unusable!");
                    ToDisable.gameObject.SetActive(false);
                    break;
                case 3:
                    Shipweapon ToDestroy = ShipToDamage.MyGuns[Mathf.RoundToInt(Random.Range(0, ShipToDamage.MyGuns.Length - 1))];
                    ShipToDamage.UpdateBattleLog(" " + ToDestroy.name + " is destroyed!");
                    Destroy(ToDestroy.gameObject); //urgh this can lead to troubles..
                    break;
                case 4:
                    Crit_GunBoom();
                    break;
                case 5:
                    Crit_GunBoom();
                    break;
                case 6:
                    Crit_GunBoom();
                    break;
                default:
                    Debug.LogError("UNDEFINED SEVERITY");
                    break;
            }

            if (ShipToDamage.GetComponentInChildren<Shipweapon>() == null)
            {
                ShipToDamage.UpdateBattleLog(" All weapons are gone!");
                ShipToDamage.Surrender(); //honorific surrender;
            }
            
        }
        else
        {
            
        }
    }

    private void Crit_GunBoom()
    {
        Shipweapon ToBoom = ShipToDamage.MyGuns[Mathf.RoundToInt(Random.Range(0, ShipToDamage.MyGuns.Length - 1))];
        ShipToDamage.UpdateBattleLog(" " + ToBoom.name + " exploded!");
        CritDamage("Hull", ToBoom.name + " explosion");
        Destroy(ToBoom.gameObject); //urgh this can lead to troubles..
    }

    public void Crit_Armour(int Severity)
    {
        bool DoesShipHaveArmour = (ShipToDamage.Armour > 0);

        if (DoesShipHaveArmour == true)
        {
            switch (Severity)
            {
                case 1:
                    ShipToDamage.Armour = Mathf.Max(ShipToDamage.Armour - 1, 0);
                    break;
                case 2:
                    ShipToDamage.Armour = Mathf.Max(ShipToDamage.Armour - d3(1), 0);
                    break;
                case 3:
                    ShipToDamage.Armour = Mathf.Max(ShipToDamage.Armour - d6(1), 0);
                    break;
                case 4:
                    ShipToDamage.Armour = Mathf.Max(ShipToDamage.Armour - d6(1), 0);
                    break;
                case 5:
                    ShipToDamage.Armour = Mathf.Max(ShipToDamage.Armour - d6(2), 0);
                    break;
                case 6:
                    ShipToDamage.Armour = Mathf.Max(ShipToDamage.Armour - d6(2), 0);
                    break;
                default:
                    Debug.LogError("UNDEFINED SEVERITY");
                    break;
            }

            if (ShipToDamage.Armour >= 0)
            {
                ShipToDamage.UpdateBattleLog(" Armour reduced to " + ShipToDamage.Armour + "!");
            }
            else if (DoesShipHaveArmour == true && ShipToDamage.Armour == 0) //ship HAD armour, no more!
                ShipToDamage.UpdateBattleLog(" Armour destroyed!!");

            if (Severity >= 5)
                CritDamage("Hull", "armour breach");

        }
        else
        {
            //
        }
    }

    public void Crit_MDrive(int Severity)
    {
        switch (Severity)
        {
            case 1:
                ShipToDamage.UpdateBattleLog(" M-Drive damaged!");
                ShipToDamage.Handling += -1;
                break;
            case 2:
                ShipToDamage.UpdateBattleLog(" M-Drive futher damaged!");
                ShipToDamage.Handling += -1;
                CritToThrust(1);
                break;
            case 3:
                ShipToDamage.UpdateBattleLog(" M-Drive severely damaged!");
                ShipToDamage.Handling += -1;
                CritToThrust(1);
                break;
            case 4:
                ShipToDamage.UpdateBattleLog(" M-Drive heavily damaged!");
                ShipToDamage.Handling += -1;
                CritToThrust(1);
                break;
            case 5:
                ShipToDamage.UpdateBattleLog(" M-Drive critically damaged: No Thrust!");
                ShipToDamage.Thrust = 0;
                break;
            case 6:
                ShipToDamage.UpdateBattleLog(" M-Drive critically damaged: No Thrust!");
                ShipToDamage.Thrust = 0;
                CritDamage("Hull", "M-Drive critical");
                break;
            default:
                Debug.LogError("UNDEFINED SEVERITY");
                break;
        }
    }

    private void CritToThrust(int Amount)
    {
        if (ShipToDamage.Thrust > 0)
        {
            ShipToDamage.Thrust = Mathf.Max(0, (ShipToDamage.Thrust - Amount));
            ShipToDamage.UpdateBattleLog(" Thrust reduced! Now " + ShipToDamage.Thrust + "!");
        }
    }

    public void Crit_PPlant(int Severity) //todo pover calculations
    {
        switch (Severity)
        {
            case 1:
                ShipToDamage.UpdateBattleLog(" Power Plant damaged!");
                //Power 90%
                CritToThrust(1);
                break;
            case 2:
                ShipToDamage.UpdateBattleLog(" Power Plant severely damaged!");
                //Power 80%
                CritToThrust(1);
                break;
            case 3:
                ShipToDamage.UpdateBattleLog(" Power Plant heavily damaged!");
                //Power 50%
                CritToThrust(1);
                break;
            case 4:
                ShipToDamage.UpdateBattleLog(" Power Plant critically damaged: No Power or Thrust!");
                //Power 0%
                ShipToDamage.Thrust = 0;
                ShipToDamage.Surrender();
                break;
            case 5:
                ShipToDamage.Thrust = 0;
                CritDamage("Hull", "Power Plant critical");
                break;
            case 6:
                ShipToDamage.Thrust = 0;
                CritDamage("Hull", "Major Power Plant critical", d6(1));
                break;
            default:
                Debug.LogError("UNDEFINED SEVERITY");
                break;
        }
    }

    public void Crit_JDrive(int Severity)
    {
        switch (Severity)
        {
            case 1:
                //This should be handled in actual JUMPCHECK
                break;
            case 2:
                ShipToDamage.UpdateBattleLog(" Jump Drive is disabled!");
                ShipToDamage.JumpClass = -1;
                break;
            case 3:
                ShipToDamage.UpdateBattleLog(" Jump Drive is destroyed!");
                ShipToDamage.JumpClass = -2;
                break;
            case 4:
                CritDamage("Hull", "Jump Drive rupture");
                break;
            case 5:
                CritDamage("Hull", "Jump Drive rupture");
                break;
            case 6:
                CritDamage("Hull", "Jump Drive rupture");
                break;
            default:
                Debug.LogError("UNDEFINED SEVERITY");
                break;
        }
    }

    public void Crit_Computer(int Severity)
    {
        switch (Severity)
        {
            case 1:
                //again, this should be handled in actual checks. IF computerseverity > 0
                ShipToDamage.UpdateBattleLog(" Computer damaged!");
                break;
            case 2:
                ShipToDamage.UpdateBattleLog(" Computer futher damaged!");
                CritToComputerRating(1);
                break;
            case 3:
                ShipToDamage.UpdateBattleLog(" Computer severely damaged!");
                CritToComputerRating(1);
                break;
            case 4:
                ShipToDamage.UpdateBattleLog(" Computer heavily damaged!");
                CritToComputerRating(1);
                break;
            case 5:
                ShipToDamage.UpdateBattleLog(" Computer disabled!");
                ShipToDamage.ComputerRating = -1;
                ShipToDamage.Surrender(); // no computer = no use to fight on.
                break;
            case 6:
                ShipToDamage.UpdateBattleLog(" Computer destroyed!");
                ShipToDamage.ComputerRating = -2;
                break;
            default:
                Debug.LogError("UNDEFINED SEVERITY");
                break;
        }
    }
    private void CritToComputerRating(int Amount)
    {
        if (ShipToDamage.ComputerRating > 0)
        {
            ShipToDamage.ComputerRating = Mathf.Max(0, (ShipToDamage.ComputerRating - Amount));
            ShipToDamage.UpdateBattleLog(" Computer Rating reduced! Now " + ShipToDamage.ComputerRating + "!");
        }
    }
    public void Crit_Crew(int Severity)
    {
        switch (Severity)
        {
            case 1:
                bool HitToCaptain = (Mathf.RoundToInt(Random.Range(1, ShipToDamage.Crew)) == 1);

                if (HitToCaptain == true)
                {
                    ShipToDamage.GetCommander().Damage(d6(1));
                }
                else
                {
                    ShipToDamage.DamageCrew(1);
                }
                break;
            case 2:
                ShipToDamage.UpdateBattleLog(" Life Support damaged!");
                break;
            case 3:

                int Casualties = d6(1);
                int CreDamage = d6(2);

                bool SecondHitToCaptain = (Mathf.RoundToInt(Random.Range(1, ShipToDamage.Crew)) <= Casualties);

                if (SecondHitToCaptain == true && ShipToDamage.GetCommander().IsAlive())
                {
                    ShipToDamage.GetCommander().Damage(CreDamage);

                    if (Casualties > 1)
                    {
                        if (ShipToDamage.Crew > Casualties)
                            Casualties = ShipToDamage.Crew;

                        ShipToDamage.DamageCrew(Casualties-1);
                    }
                }
                else
                {
                    ShipToDamage.DamageCrew(Casualties);
                }
                break;
            case 4:
                ShipToDamage.UpdateBattleLog(" Life Support seriously damaged!");
                break;
            case 5:
                int BigCreDamage = d6(3);

                if (ShipToDamage.GetCommander().IsAlive())
                {
                    ShipToDamage.GetCommander().Damage(BigCreDamage);
                    ShipToDamage.DamageCrew((ShipToDamage.Crew / 2)-1);
                }
                else
                {
                    ShipToDamage.DamageCrew(ShipToDamage.Crew/2);
                }
                break;
            case 6:
                ShipToDamage.UpdateBattleLog(" Life support failed!");
                ///check if voidsuits. If false = KILL EVERYONE
                break;
            default:
                Debug.LogError("UNDEFINED SEVERITY");
                break;
        }
    }


}
