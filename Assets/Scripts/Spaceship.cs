using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The actual things that are important in this game :D
/// </summary>
public class Spaceship : SpaceObject {

    public string HullType = "Patrol Cruiser";
    public int Tonnage = 400;
    public int Hullpoints = 160;
    public int HullpointsOrig = 160;
    public int Fuel = 124;
    public int FuelOrig = 124;
    public string HullConfig = "Streamlined";
    public int Armour = 4;
    public int ComputerRating = 15;
    public int Sensors = 0; 	//standard mil sensors
    public int SensorRoll = 0;  //The Standard Sensor Roll for this round.
    public int MaxSensorRange = RangeB_VDistant;
    public int Stealth = 0;		//for stealth ships...
    public int JumpClass = 3;
    public int Handling = 0;
    public bool Transponders = false;
    public string TransponderMessage = "";

    public string Side; //more complex?
    public string Status = "OK";
    public string Alarm = "Yellow";
    /*	White = do not move, surrender
     *  Green = standard, no hostiles present
     *  Yellow = Ready for action, do not fire first
     *  Orange: Lasers OK, Missiles no?
     *  Red = Fire at will!
     */
    public string Order = "Move";
    /*	Move = move towards movementTarget, attacking directed by alert status
     *  Stop = Do Not Move, attacking directed by alert status
     * 	Engage = will move towards Enemy at max speed.
     *  (later = Dock)
     */

    //public Captain = Haddoc
    public string CaptainName = "Haddock";

    //public int Skill_Gunnery = 0;
    public int Skill_Pilot = 0;
    public int Skill_Electronics = 0;
    public GameObject ShipExplosion;
    public Shipweapon[] MyGuns;
    public List<string> MyCritDamages; 

    public Spaceship Enemy;
    public Spaceship Targetlock;
    public Transform Destination;
    public List<MissileSalvo> IncomingMissiles = new List<MissileSalvo>();

    public string myBattleLog = "";

    // Use this for initialization
    void Start() {

        if (this.TransponderMessage == "")
            TransponderMessage = this.name;


        this.MyGuns = this.gameObject.GetComponentsInChildren<Shipweapon>();
        //this.Targetlock = this;

        this.Skill_Pilot = Mathf.RoundToInt(Random.Range(0f, 2f));
        this.Skill_Electronics = Mathf.RoundToInt(Random.Range(0f, 2f));

        //UpdateBattleLog ("---Captain " + this.CaptainName + "s battlelog for " + this.HullType + " " + this.name);

        if (this.ShipExplosion == null)
            Debug.LogError(name + "has no ShipExplosion!");

        if (GetComponentInParent<Fleet>() == null)
            Debug.LogError(name + "has no Fleet!");

    }

    /*
	void Update ()
	{
		if (Enemy != null) {
			if (Enemy.gameObject.activeSelf == false)
				Enemy = null;
		}
	} 
	*/

    public override void GameTurn(int turnNumber)
    {

        if (turnNumber >= 10) {
            int hours = Mathf.RoundToInt((turnNumber / 10) - 0.5f);
            UpdateBattleLog("\n--Elapsed time: " + hours + " h, " + ((turnNumber * 6) - (hours * 60)) + " m, " + d6(2) + " s");
        }
        else
            UpdateBattleLog("\n--Elapsed time: " + turnNumber * 6 + " m, " + d6(2) + " s");
        UpdateBattleLog(" -Location: " + this.transform.position);

        if (GetComponents<FuelLeak>() != null)
        {
            foreach (FuelLeak leaky in GetComponents<FuelLeak>())
            {
                leaky.LeakFuelCheck();
                //TODO: Engineers fix leak!
            }
        }
        

        //MOVEMENT STEP

        //TODO: MOVEMENTLOGIC. Straight Towards enemy, traight to any direction, dogfight, follow friend?

        if (Order == "Engage" && HasEnemy() && Alarm == "Red")
            Destination = Enemy.transform;


        if (!(Alarm == "White" | Order == "Stop")) {
            if (Destination != null && Order != "Engage")
                UpdateBattleLog(" -Destination: " + Destination.transform.position + " Distance: " + this.DistanceTo(Destination) + " KM = " + this.RangeBandToString(Destination));
            this.MovementLogic();
        }
        //ATTACK STEP
        if (Alarm == "Red")
            this.AttackLogic();

        //ACTIONS STEP
        this.PerformSensorAction();


    }

    //
    private void MovementLogic()
    {
        if (Order == "Stop")
        { }
        else if (Destination == null) {
            this.MoveForward(this.Thrust / 2); //Default move, merely forward
                                               //UpdateBattleLog(" Defaultmoving, howw boring..");

        }
        //else if (Alarm == "Red" && (HasEnemy() && this.DistanceTo(Enemy) < RangeB_Close))
        //DOGFIGHTING
        else
        {
            //TODO complain upwards that hey gimme me ssomething to do!

            if (this.Move(this.Thrust, Destination.transform.position) == true) {
                if (this.Order == "Move") {
                    UpdateBattleLog(" Destination reached, coming to full stop!");
                    this.Order = "Stop";
                    this.Destination = null;
                }
            }
        }
    }
    private void AttackLogic()
    {
        if (HasEnemy())
        {
            UpdateBattleLog(" -Target: " + Enemy.name + " Distance: " + this.DistanceTo(Enemy) + " KM = " + this.RangeBandToString(Enemy.transform));

            this.Attack(Enemy);
        }
        else //Ne enemy needed!
        {
            this.SeekNewEnemy();
            this.Attack(Enemy);
        }
    }

    public void Attack(Spaceship AttackTarget)
    {

        if (AttackTarget != null && AttackTarget != this)
        {
            foreach (Shipweapon Gun in MyGuns) {
                //Debug.Log (Gun.Attack (Enemy));;
                UpdateBattleLog(Gun.Attack(AttackTarget));
            }
        }
    }

    public void Attack()
    {
        this.Attack(Enemy);
    }

    /// <summary>
    /// Damage the ship HowMuch.
    /// </summary>
    /// <param name="HowMuch">Amount of Damage</param>
    /// <param name="Source">Source of Damage</param>
    public string Damage(int HowMuch, string Source, bool IgnoreArmour)
    {

        int ActualDamage = Mathf.Max(HowMuch - Armour, 0); //armor removes parts of damage

        if (IgnoreArmour == true) 
            ActualDamage = HowMuch;

        this.Hullpoints -= ActualDamage;

        // TODO CRITICAL HITS - now just somthign

        if (Hullpoints <= 0) {      //DEADCHECK

            return this.Die("was destroyed by " + Source);
        }

        if (ActualDamage == 0) {
            UpdateBattleLog(" Received 0 hull damage from " + Source + " - armour held!");
            return (this.name + " - Armor Deflected all damage!");
        }

        UpdateBattleLog(" Received " + ActualDamage + " hull damage from " + Source + "!");

        if (((Hullpoints < HullpointsOrig / 10)) && (this.Hullpoints>0))
        {
            //Debug.Log (this.name + " DANGER DANGER DA DAMAGED!");
            UpdateBattleLog("  DANGER DANGER DANGER!");

            this.Status = "DANGER";

            this.SurrenderCheck();
        }

        this.CriticalHitCheck(ActualDamage);

        this.ChangeAlarm("Red");

        AngerEngagingSwitchCheck(Source);

        return (this.name + " - " + ActualDamage + " hull damage!");


    }


    public string Damage(int HowMuch, string Source)
    {
        return this.Damage(HowMuch, Source, false);
    }

    /// <summary>
    /// Handles Critical Hits
    /// </summary>
    /// <returns>true, if critical damage inflicted.</returns>
    public bool CriticalHitCheck(int DamageIncurred)
    {
        int PreDamageHP = this.Hullpoints + DamageIncurred;

        int PostDamageHP = this.Hullpoints;

        int CritCheck = Mathf.RoundToInt(PreDamageHP / (HullpointsOrig/10)) - Mathf.RoundToInt(PostDamageHP / (HullpointsOrig / 10));

        //Debug.Log(this.name + " CritCheck: " + CritCheck);

        //Crits happen if damage takes HPs into ther tenth of original HP.

        if (CritCheck > 0)
        {
            if (this.MyCritDamages.Count == 0)
            {
                this.Status = "Damaged";
                UpdateBattleLog(" Status " + Status + ": HP " + Hullpoints + "/" + HullpointsOrig);
            }
            else if (this.MyCritDamages.Count == 2)
            {
                this.Status = "Severe";
                UpdateBattleLog(" Status " + Status + ": HP " + Hullpoints + "/" + HullpointsOrig);
            }
            else if (this.MyCritDamages.Count == 4)
            {
                this.Status = "Critical";
                UpdateBattleLog(" Status " + Status + ": HP " + Hullpoints + "/" + HullpointsOrig);
            }
            else if (this.MyCritDamages.Count == 7)
            {
                this.Status = "Grave";
                UpdateBattleLog(" Status " + Status + ": HP " + Hullpoints + "/" + HullpointsOrig);
            }

            for (int e = CritCheck; e > 0; e--)
                this.CriticalDamage();

            return true;
        }

        return false;

    }


    public int GetCurrentCritSeverity(string DamageType)
    {
        int ToReturn = 0;

        foreach (string damagy in this.MyCritDamages)
        {
            if (damagy.Contains(DamageType))
                ToReturn++;
        }

        return ToReturn;
    }

    /// <summary>
    /// Actual Critical Damage.
    /// Rather complicated mess.
    /// </summary>
    public void CriticalDamage()
    {
        int CritRoll = d6(2);

        CriticalDamager auts = new CriticalDamager(this);

        switch (CritRoll)
        {
            case 2:
                //Sensors
                auts.CritDamage("Sensors");
                break;
            case 3:
                //PowerPlant
                auts.CritDamage("PowerPlant");
                break;
            case 4:
                //Fuel urgh
                auts.CritDamage("Fuel");
                break;
            case 5:
                //Weapon
                auts.CritDamage("Weapon");
                break;
            case 6:
                //Armour
                auts.CritDamage("Armour");
                break;
            case 8:
                //M-Drive
                auts.CritDamage("M-Drive");
                break;
            case 9:
                //Cargo
                auts.CritDamage("Cargo");
                break;
            case 10:
                //J-Drive
                auts.CritDamage("J-Drive");
                break;
            case 11:
                //Crew
                auts.CritDamage("Crew");
                break;
            case 12:
                //Computer
                auts.CritDamage("Computer");
                break;
            default: //7
                //Hull
                auts.CritDamage("Hull");
                break;
        }
    
    }

    /// <summary>
    /// Changes current fuel by amount.
    /// If fuel == 0, surrenders.
    /// </summary>
    /// <param name="amount"></param>
    public void FuelChange(int amount)
    {
        if (Fuel > 0)
        {
            this.Fuel = Mathf.Min(Fuel + amount, FuelOrig);

            if (Fuel <= 0) //TODO calc for no enough fuel to run Power Plants
            {
                Fuel = 0;
                UpdateBattleLog(" No fuel left!");
                this.Surrender();
            }
            else if (amount > 0)
            {
                UpdateBattleLog(" Refueled for " + amount + " tons.");
            }
            else if (amount < 0)
            {
                UpdateBattleLog(" Lost " + amount * -1 + " tons of fuel.");
            }
        }
    }

    /// <summary>
    /// Tries to Jump. Currently in basic form only.
    /// </summary>
    /// <param name="ParsekDistance">Distance to target</param>
    /// <param name="JumpTarget"></param>
    /// <returns></returns>
    public bool Jump(int ParsekDistance, string JumpTarget)
    {
        if (this.JumpClass <= 0)
        {
            UpdateBattleLog(" Jump: Cannot jump to " + JumpTarget + ": No functioning jump engine!");
        }
        else if (ParsekDistance > this.JumpClass)
        {
            UpdateBattleLog(" Jump: Cannot jump to " + JumpTarget + ": Target too far!");
        }
        else if (JumpFuelCalculate(ParsekDistance) + 1 > this.Fuel)
        {
            UpdateBattleLog(" Jump: Cannot jump to " + JumpTarget + ": Not enough fuel!");
        }

        UpdateBattleLog(" Jump: Initialising jump travel to " + JumpTarget + ".");

        this.FuelChange(JumpFuelCalculate(ParsekDistance));
        UpdateBattleLog(" +++ JUMP +++");
        this.gameObject.SetActive(false); // good enough for this situation..
        return true;
    }

    public int JumpFuelCalculate(int ParsekDistance)
    {
        int FuelCalculation = (this.Tonnage / 10) * ParsekDistance;
        return FuelCalculation;
    }

    public bool HasLock()
	{
		if (Targetlock != null && this.Targetlock != this && Targetlock.gameObject.activeSelf == true)
			return true;
		return false;
	}

	/// <summary>
	/// Checks if there is an ALIVE and TARGETABLE enemy. If Enemy is dead, resets Enemy.
	/// </summary>
	/// <returns><c>true</c> if yes; <c>false</c> in other cases.</returns>
	public bool HasEnemy()
	{
		if (Enemy == null | Enemy == this)
			return false;

		if (Enemy.gameObject.activeSelf == false) 
		{
			Enemy = null;
			return false;
		}

		return true;
	}

    /// <summary>
    /// If attacked vhile engaging, does change attack target?
    /// </summary>
    /// <param name="damagesource"></param>
	public void AngerEngagingSwitchCheck(string damagesource)
		{

        if (Order == "Engage")
        {
            if (HasEnemy() == false)  //if after killing someone previously
            {
                foreach (Spaceship question in FindObjectsOfType<Spaceship>())
                {
                    if (question.Side != "Neutral" && question.Side != this.Side && question.gameObject.activeSelf == true && damagesource.Contains(question.name))
                    {
                        Engage(question);
                        break;
                    }
                }

            }
            else if (this.HasLock() && Targetlock == Enemy)
            {
                //keep on shooting
            }
            else if ((d6(2) > 8) && !damagesource.Contains(Enemy.name)) //no jos ny vaihteeks
            {
                foreach (Spaceship question in FindObjectsOfType<Spaceship>())
                {
                    if (question.Side != "Neutral" && question.Side != this.Side && question.gameObject.activeSelf == true && damagesource.Contains(question.name))
                    {
                        Engage(question);
                        break;
                    }
                }
            }
        }
	}

    /// <summary>
    /// find something to blast
    /// </summary>
    /// <returns>The new enemy.</returns>
    public Spaceship SeekNewEnemy() {

        if (GetComponentInParent<Fleet>() == null | GetComponentInParent<Fleet>().MyEnemies == null)
        {
            UpdateBattleLog(" No enemies!");

            return this; //not the most ideaal
        }

        UpdateBattleLog(" Scanning for enemies:");
        //Debug.Log (this.name + " SEEKING NEW ENEMY ");

        if (this.HasLock() && Targetlock.Alarm != "White")  //like to target targetlock, duh
        {
            this.Enemy = Targetlock;
            UpdateBattleLog(" Targeting " + Enemy.HullType + " " + Enemy.name + " Distance: " + this.DistanceTo(Enemy) + " KM = " + this.RangeBandToString(Enemy.transform));
            return Targetlock;
        }

        //TODO more detailed way of randomising the next target: perhaps by range?

        if (GetComponentInParent<Fleet>().MyEnemies.GetMyCurrentShips().Length == 0)
        {
            UpdateBattleLog(" No targets found!");

            return this; //not the most ideaal
        }

        Spaceship NuEnemy = GetComponentInParent<Fleet>().GiveRandomEnemy();

        if (this.Order == "Engage")
        {
            this.Engage(NuEnemy);
        }
        else {
            this.Enemy = NuEnemy;
            UpdateBattleLog(" Targeting " + Enemy.HullType + " " + Enemy.name + " Distance: " + this.DistanceTo(Enemy) + " KM = " + this.RangeBandToString(Enemy.transform));
        }

        return this.Enemy;


		//Debug.Log (this.name + " IS VICTORIOUS! CHEEERING!");


	}

	/// <summary>
	/// Attack the specified Target ship.
	/// </summary>
	/// <param name="Target">Enemy to attack.</param>
	public void Engage(Spaceship Target)
	{
		if (this.Enemy != Target && Alarm == "Red") 
		{
			this.Enemy = Target;
			this.Destination = Target.transform;
			this.Order = "Engage";
			//Debug.Log (this.name + " ENGAGING " + question.name);
			UpdateBattleLog (" ENGAGING " + Target.HullType + " " + Target.name + " Distance: " + this.DistanceTo(Target) + " KM = " + this.RangeBandToString(Target.transform));
		}
		else if (Alarm != "Red")
			UpdateBattleLog (" Cannot Engage" + Target.name + ": Alarm not Red!");
	}

    public void PerformSensorAction()
    {
        
        //If Missiles incoming = countermeasures!

        this.SensorRoll = -10; //aka not done this round

        if (this.IncomingMissiles.Count > 0)
        {

            if (this.IncomingMissiles[0] != null)
                UpdateBattleLog(ElectronicCountermeasure(this.IncomingMissiles[0])); //should priorize moreeee but not now
        }
        else if (HasEnemy() && Targetlock != Enemy && Alarm == "Red")

        {
            this.TargetLockCheck(Enemy);
        }
        else // Sensor operator has time to do something else this round!
        {
            this.SensorRoll = d6(2) + Skill_Electronics + Sensors; //SCAN SURROUNDINGS
        }


    }

    public string ElectronicCountermeasure( MissileSalvo problem)
	{
		int Check = d6 (2) + Skill_Electronics + Sensors; 

		if (Check >= 10)
		{
			int Effect = Check - 9;

			problem.ReduceMissiles (Effect);

			return (" Successfully countermeasured against " + problem.name +"!");

		}
		return (" Failed countermeasures against "+ problem.name + "! ");

	}

	public void TargetLockCheck( Spaceship potentialtarget)
	{
		if (Targetlock != this | Targetlock != potentialtarget) {	//do nothing if already has target lock!
			int Check = d6 (2) + Skill_Electronics + Sensors - potentialtarget.Stealth; 

			if (Check >= 8 | potentialtarget.Transponders == true) {
				this.Targetlock = potentialtarget;

				UpdateBattleLog (" Sensors locked to " + potentialtarget.name + "!");
				potentialtarget.UpdateBattleLog (" " + this.name + " got a sensor lock on us!");	//nothing more for nooow??

			}
		}


	}

    public override bool IsVisible(SpaceObject Objecty)
    {
        bool LineCasty = ((Physics.Linecast(this.transform.position, Objecty.transform.position) == false) && (this.DistanceTo(Objecty) < this.MaxSensorRange));
        //Debug.Log("Linecast from " + this.name + " to " + Objecty + ": " + LineCasty);
        return LineCasty;
    }

    /// <summary>
    /// Ship is scanned by another ship/fleet
    /// </summary>
    /// <param name="Distance"></param>
    /// <returns>Scan result</returns>
    public string Scanned(float Distance)
    {
        string ScanResult = "";

        // THERMAL


        if (this.Hullpoints > 0)
            ScanResult += "Warm ";
        else
            ScanResult += "Cold ";

        if (Distance < RangeB_Short)
            ScanResult += this.Side + " ";    // Fine Details - basically visual Inspection

        //VISUAL

        if (Distance < RangeB_Medium)
            ScanResult += (this.HullpointsOrig * 2.5f) + " ton ";

        if (Distance < RangeB_Short)
            ScanResult += this.HullType;    // Fine Details
        else if (Distance < RangeB_Long)
            ScanResult += this.HullConfig;  // Shape and structure
        else
            ScanResult += "unidentified";  // Basic outline

        // EM

        if (Distance < RangeB_VLong && Distance >= RangeB_Short)
            ScanResult += " spaceship";   
        else if (Distance < RangeB_Distant && Distance >= RangeB_VLong)
            ScanResult += " object";        //TODO: scan for other SpaceObjects?

        ScanResult += "\n";

        if (this.Transponders == true)
            ScanResult += " Transponder: " + this.TransponderMessage + "\n";
        
        if (this.Order != "Stop")
        {
            ScanResult += " Speed: " + this.Thrust + "\n";
            //ScanResult += " Direction: " + this.transform.forward + "\n";
        }

        // Fine Details

        if (Distance < RangeB_Short)
            ScanResult += " Hull: " + Hullpoints + " / " + HullpointsOrig + "\n";

        if (this.MyGuns.Length > 0 )
        {
            if (Distance < RangeB_Short)
            {
                ScanResult += " Weapons:\n";
                foreach (Shipweapon gunnen in this.MyGuns)  // Fine Details
                {
                    if (gunnen.gameObject.activeInHierarchy == true)
                        ScanResult += "  " + gunnen.name + "\n";
                }
            }
            else if (Distance < RangeB_Long)
            {
                ScanResult += " Weapons: " + MyGuns.Length + "\n"; // Hot or Cold Spots + Sources
               
            }
            
        }


        return ScanResult;


    }


	/// <summary>
	/// Does ship detect a missile launch towards itself??
	/// </summary>
	/// <param name="problem">incoming missile.</param>
	public void MissileLaunchDetectCheck( MissileSalvo problem)
	{

		if (SensorRoll >= 8) 
		{
			if (problem.AmountOfMissiles > 1)
				UpdateBattleLog(" Incoming missiles from " + problem.source.name + "!");
			else
				UpdateBattleLog(" Incoming missile  from " + problem.source.name + "!");
            
			this.ChangeAlarm ("Red"); //justified automation

            if (HasEnemy() == false && problem.source.gameObject.activeSelf)
            {
                if (Order == "Engage")
                    Engage(problem.source);
                else
                { 
                    this.Enemy = problem.source;
                    UpdateBattleLog(" Targeting missile source " + Enemy.HullType + " " + Enemy.name + " Distance: " + this.DistanceTo(Enemy));
                }
            }
            //TODO AI logic?? should this only here add them to IncomingMissiles?
        }

	}

	/// <summary>
	/// Changes the Ships alert to a new type.
	/// </summary>
	/// <param name="NuAlarm">what to attempt changing</param>
	public bool ChangeAlarm (string NuAlarm)
	{
        //Debug.Log(this.name + " ChangeAlarm " + this.Alarm + " : " + NuAlarm);


        if (this.Alarm == "White") {
			return false;
		}
		else if ((NuAlarm == "White") && this.Alarm != "White") {
			return SetAlarm (NuAlarm);
		}
		else if ((NuAlarm == "Green") && this.Alarm != "Green") {
			return SetAlarm (NuAlarm);
		}
		else if ((NuAlarm == "Yellow") && this.Alarm != "Yellow") {
			return SetAlarm (NuAlarm);
		}
		else if ((NuAlarm == "Red") && this.Alarm != "Red") {	//Only Yellow allows going to Red!
			return SetAlarm (NuAlarm);
		}
		else if (!(NuAlarm == "White" | NuAlarm == "Green" | NuAlarm == "Yellow" | NuAlarm == "Red"))
			Debug.LogWarning (this.name + " Trying to change to wrong kind of Alarm = " + NuAlarm);

		return false;

	}

	private bool SetAlarm(string NuAlarm)
	{
		this.Alarm = NuAlarm;
		if (NuAlarm == "Red")
            UpdateBattleLog (" " + this.Alarm.ToUpper() + " ALARM!!");	
        else
            UpdateBattleLog(" " + this.Alarm + " Alarm!");

        return true;

	}


	/// <summary>
	/// Updates the battle log of the ship.
	/// </summary>
	/// <param name="Newline">Newline.</param>
	public void UpdateBattleLog(string Newline)
	{
		if (Newline.Contains ("No missiles left") == false | Newline.Contains (" Trying Touch selfshoot WTF"))
			this.myBattleLog += (Newline + "\n");
	}


	public string MiniReport()
	{
		if (this.Order == "Move" && this.Destination != null)
			return (this.name + " | Moving to " + Destination.name + ", Dist " + this.DistanceTo(Destination));
		if (this.Order == "Move" && this.Destination == null)
			return (this.name + " | Advancing towards " + this.transform.forward );
		else if (this.Order == "Engage" && HasEnemy())
			return (this.name + " | Engaging " + Enemy.name + ", Dist " + this.DistanceTo(Enemy));
		else if (this.Alarm == "Red" && HasEnemy())
			return (this.name + " | Firing at " + Enemy.name + ", Dist " + this.DistanceTo(Enemy));
		else if (this.Alarm == "White")
			return (this.name + " | Surrendering" );
		else if (this.Order == "Stop")
			return (this.name + " | At " + this.transform.position );

		return (this.name + " | No orders");
		 
	}

    public Commander GetCommander()
    {
        return this.GetComponentInChildren<Commander>(); // every ship should have a commander.
    }

	public void SurrenderCheck()
	{
		if (Alarm != "White") {
			int AreCrewCowards = d6 (2) - Mathf.Max (Skill_Pilot, Skill_Electronics) - Mathf.Max(GetCommander().Skill_Blade,GetCommander().Skill_Leadership); //Veterancy Helps

			if (AreCrewCowards > 8) {
				//Allgood
			} else if (AreCrewCowards == 8) {
				UpdateBattleLog (" Crew are getting restless!");
			} else if (AreCrewCowards < 8) {
				this.Surrender ();
			}
		} else {
			UpdateBattleLog (" Aaargh!!"); //are getting shot at after surrender!
		}
	}

	public void Surrender()
	{
		Debug.Log (this.name + " Surrendered!");
		UpdateBattleLog (" Surrendering!");
		ChangeAlarm ("White");
		this.Order = "Stop";
		this.Thrust = 0;
		this.Enemy = null;
		this.Destination = null;
		this.Targetlock = null;
		this.Transponders = true;
        this.TransponderMessage = "SOS from " + this.name ;

        foreach (MeshRenderer Flaggen in GetComponentsInChildren<MeshRenderer>()) {
			if (Flaggen.name.Contains ("Flag_")) {
				Flaggen.gameObject.SetActive(false);
			}
		}

        this.transform.parent = null; //Basically leaves the Fleet it as in. TODO something better here!

		foreach (Spaceship PlzDontShoot in FindObjectsOfType<Spaceship>())
		{
			if (PlzDontShoot.Enemy == this) 
			{
				PlzDontShoot.UpdateBattleLog (" " + PlzDontShoot.Enemy.name + " surrendered!");

				int MoralityCheck = d6(2) + GetCommander().Skill_Leadership;
				if (MoralityCheck >= 8)
					PlzDontShoot.SeekNewEnemy();
				else
					PlzDontShoot.UpdateBattleLog (" Muahahhaa!");
				
			}

		}
	}

	public string Die () {
		return this.Die("was wrecked!");
	}

	public string Die (string how) {

		this.Status = how;

		Debug.Log (this.name + " " + how);
		UpdateBattleLog ("....Log ends");

//		foreach (Spaceship toNote in FindObjectsOfType<Spaceship>()) 
//		{
//			
//			if (toNote.Enemy != null) 
//			{
//				if (toNote.Enemy == this)
//					toNote.UpdateBattleLog (" " + this.name + " " + how);
//			}
//		}

		Instantiate (ShipExplosion, this.transform.position,this.transform.rotation);

		this.gameObject.SetActive (false);

		return (this.name + " " + how);
	}

	void OnDestroy(){
		//Debug.LogError ("This should not happen!!");
	}
}
