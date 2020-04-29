using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : SpaceObject {

	public string HullType = "Patrol Cruiser";
	public int Hullpoints = 160;
	public int HullpointsOrig = 160;
	public int Armour = 4;
	public int Sensors = 0; 	//standard mil sensors
	public int Stealth = 0;		//for stealth ships...
	public bool Transponders = false;

	public string Side; //more complex?
	public string Status = "OK";
	public string Alarm = "Yellow";
		/*	White = do not move, surrender
		 *  Green = standard, no hostiles present
		 *  Yellow = Ready for action, do not fire first
		 *  TODO: Orange: Lasers OK, Missiles no?
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

	public Spaceship Enemy;
	public Spaceship Targetlock;
	public Transform Destination;
	public List<MissileSalvo> IncomingMissiles = new List<MissileSalvo>();

	public string myBattleLog = "";

	// Use this for initialization
	void Start () {

		this.MyGuns = this.gameObject.GetComponentsInChildren<Shipweapon> ();
		//this.Targetlock = this;

		this.Skill_Pilot = Mathf.RoundToInt (Random.Range (0f, 2f));
		this.Skill_Electronics = Mathf.RoundToInt (Random.Range (0f, 2f));

		//UpdateBattleLog ("---Captain " + this.CaptainName + "s battlelog for " + this.HullType + " " + this.name);

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
			int hours = Mathf.RoundToInt ((turnNumber / 10) - 0.5f);
			UpdateBattleLog ("\n--Elapsed time: " + hours +" h, " + (turnNumber * 6 - hours*6) + " m, " + d6 (2) + " s");
		}
		else
			UpdateBattleLog ("\n--Elapsed time: " + turnNumber*6 + " m, " + d6 (2) + " s");
		UpdateBattleLog (" -Location: " + this.transform.position);

		//if (this.Hullpoints <= 0 && this.gameObject.activeSelf == true)
		//this.Die();

		//MOVEMENT STEP

		//TODO: MOVEMENTLOGIC. Straight Towards enemy, traight to any direction, dogfight, follow friend?

		if (Order == "Engage" && HasEnemy() && Alarm == "Red")
			Destination = Enemy.transform;


		if (!(Alarm == "White" | Order == "Stop")) {
			if (Destination != null && Order != "Engage" )
				UpdateBattleLog (" -Destination: " + Destination.transform.position + " Distance: " + this.DistanceTo(Destination));
			this.MovementLogic ();
		}
		//ATTACK STEP
		if (Alarm == "Red")
			this.AttackLogic();

		//ACTIONS STEP
		this.PerformSensorAction ();


	}

    //
    private void MovementLogic()
    {

        if (Destination == null | (Alarm == "Red" && (HasEnemy() && this.DistanceTo(Enemy) < 2)) ) { 
            this.Move(this.Thrust / 2, this.transform.position + this.transform.forward * this.Thrust); //Default move, merely forward
            //UpdateBattleLog(" Defaultmoving, howw boring..");
          
        }
		else 
		{
			//TODO complain upwards that hey gimme me ssomething to do!

			if (this.Move (this.Thrust, Destination.transform.position) == true) {
				if (this.Order == "Move") {
					UpdateBattleLog (" Destination reached, coming to full stop!");
					this.Order = "Stop";
					this.Destination = null;
				}
			}
		}
	}

	private void AttackLogic ()
	{
		if (HasEnemy ()) 
		{
			UpdateBattleLog (" -Target: " + Enemy.name + " Distance: " + this.DistanceTo (Enemy));
		
			this.Attack (Enemy);
		}
		else //Ne enemy needed!
		{
			this.SeekNewEnemy ();
			this.Attack (Enemy);
		}
	}

	public void Attack (Spaceship AttackTarget)
	{

		if (AttackTarget != null && AttackTarget != this) 
		{
			foreach (Shipweapon Gun in MyGuns) {		
				//Debug.Log (Gun.Attack (Enemy));;
				UpdateBattleLog (Gun.Attack (AttackTarget));
			}
		}
	}

	public void Attack ()
	{
		this.Attack (Enemy);
	}

	/// <summary>
	/// Damage the ship HowMuch.
	/// </summary>
	/// <param name="HowMuch">Amount of Damage</param>
	/// <param name="Source">Source of Damage</param>
	public string Damage (int HowMuch, string Source)
	{

		int ActualDamage = Mathf.Max(HowMuch - Armour,0);

		if (Source.Contains("missile"))	//brutal check here but good enough for now;
			ActualDamage = HowMuch;

		this.Hullpoints -= ActualDamage;

		// TODO CRITICAL HITS - now just somthign

		if ((Hullpoints < HullpointsOrig / 2) && this.Armour == 4) {
			this.Armour = 3;
			Debug.Log (this.name + " CRITICALLY DAMAGED!");
			UpdateBattleLog (" CRITICALLY DAMAGED!");

			this.Status = "Critical";

			if (Random.Range (0, 10) > 7) {
				MyGuns [Mathf.RoundToInt (Random.Range (0f, 3f))].Skill_Gunnery -= 1; //wounded crewmember
				UpdateBattleLog (" Crew hit!");
			} else if (Random.Range (0, 10) > 7) {
				MyGuns [Mathf.RoundToInt (Random.Range (0f, 3f))].gameObject.SetActive (false); //disabled gun
				UpdateBattleLog (" Turret disabled!");
			} else if (Random.Range (0, 10) > 7) {
				this.Armour = Mathf.Max (0, this.Armour - 1);
				UpdateBattleLog (" Armor plates scarred!");
			}else {
				this.Hullpoints -= d6 (2);
				UpdateBattleLog (" Hull rupture!");
			}
		}	
		else if ((Hullpoints < HullpointsOrig / 10)) {
			//this.Armour = 3;
			//Debug.Log (this.name + " DANGER DANGER DA DAMAGED!");
			UpdateBattleLog ("  DANGER DANGER DANGER!");

			this.Status = "DANGER";

			this.SurrenderCheck ();
		}	

		if (Hullpoints <= 0) {		//DEADCHECK
			
			return this.Die ("was destroyed by " + Source);
		}

		if (ActualDamage == 0) {
			UpdateBattleLog (" Under fire: " + Source + ". Armor held!");
			return (this.name + " - Armor Deflected all damage!");
		}
			
		UpdateBattleLog (" Under fire from: " + Source + "! Received " + ActualDamage + " hull damage!");

		this.ChangeAlarm ("Red");

		AngerEngagingSwitchCheck (Source);

		return (this.name + " - " + ActualDamage + " hull damage!");


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

	public void AngerEngagingSwitchCheck(string damagesource)
		{
		
		if ( HasEnemy() == false )  //if after killing someone previously
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
		else if ((d6(2)>8) && !damagesource.Contains(Enemy.name) ) //no jos ny vaihteeks
		{			
			foreach (Spaceship question in FindObjectsOfType<Spaceship>()) {
				if (question.Side != "Neutral" && question.Side != this.Side && question.gameObject.activeSelf == true && damagesource.Contains (question.name)) {
					Engage(question);
					break;
				}
			}
		}
	}

    /// <summary>
    /// find something to blast
    /// </summary>
    /// <returns>The new enemy.</returns>
    public Spaceship SeekNewEnemy() {

        UpdateBattleLog(" Scanning for enemies:");
        //Debug.Log (this.name + " SEEKING NEW ENEMY ");

        if (this.HasLock() && Targetlock.Alarm != "White")  //like to target targetlock, duh
        {
            this.Enemy = Targetlock;
            UpdateBattleLog(" Targeting " + Enemy.HullType + " " + Enemy.name + " Distance: " + this.DistanceTo(Enemy));
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
            UpdateBattleLog(" Targeting " + Enemy.HullType + " " + Enemy.name + " Distance: " + this.DistanceTo(Enemy));
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
			UpdateBattleLog (" ENGAGING " + Target.HullType + " " + Target.name + " Distance: " + this.DistanceTo(Target));
		}
		else if (Alarm != "Red")
			UpdateBattleLog (" Cannot Engage" + Target.name + ": Alarm not Red!");
	}

	public void PerformSensorAction()
	{
		//SCAN SURROUNDINGS

		//If Missiles incoming = countermeasures!


		if (this.IncomingMissiles.Count > 0) {

			if (this.IncomingMissiles[0] != null)
				UpdateBattleLog(ElectronicCountermeasure (this.IncomingMissiles[0]) ); //should priorize moreeee but not now
		}		
		else
		{
			if (HasEnemy() && Targetlock != Enemy && Alarm == "Red" )
				this.TargetLockCheck (Enemy);
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

	/// <summary>
	/// Does ship detect a missile launch towards itself??
	/// </summary>
	/// <param name="problem">incoming missile.</param>
	public void MissileLaunchDetectCheck( MissileSalvo problem)
	{
		int Check = d6 (2) + Skill_Electronics + Sensors; 

		if (Check >= 8) 
		{
			if (problem.AmountOfMissiles > 1)
				UpdateBattleLog(" Incoming missiles from " + problem.source.name + "!");
			else
				UpdateBattleLog(" Incoming missile  from " + problem.source.name + "!");

			this.ChangeAlarm ("Red");
			if (HasEnemy() == false && problem.source.gameObject.activeSelf)
				Engage (problem.source);
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
		UpdateBattleLog (" " + this.Alarm.ToUpper() + " ALARM!!");	
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

	public void SurrenderCheck()
	{
		if (Alarm != "White") {
			int AreCrewCowards = d6 (2) + Mathf.Max (Skill_Pilot, Skill_Electronics); //Veterancy Helps

			if (AreCrewCowards > 8) {
				//Allgood
			} else if (AreCrewCowards == 8) {
				UpdateBattleLog (" Crew are getting restless!");
			} else if (AreCrewCowards < 8) {
				this.Surrender ();
			}
		} else {
			UpdateBattleLog (" Aaargh!!");
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

		foreach (MeshRenderer Flaggen in GetComponentsInChildren<MeshRenderer>()) {
			if (Flaggen.name.Contains ("Flag_")) {
				Flaggen.gameObject.SetActive(false);
			}
		}

        this.transform.parent = null; //Basically leaves the Fleet it as in.

		foreach (Spaceship PlzDontShoot in FindObjectsOfType<Spaceship>())
		{
			if (PlzDontShoot.Enemy == this) 
			{
				PlzDontShoot.UpdateBattleLog (" " + PlzDontShoot.Enemy + " surrendered!");

				int MoralityCheck = d6(2);
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
