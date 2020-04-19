using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : SpaceObject {

	public string HullType = "Patrol Corvette";
	public int Hullpoints = 160;
	public int HullpointsOrig = 160;
	public int Armour = 4;
	public int Sensors = 0; 	//standard mil sensors
	public int Stealth = 0;		//for stealth ships...
	public bool Transponders = false;

	public string Side; //more complex?
	public string Status = "OK";

	//public Captain = Haddoc
	public string CaptainName = "Haddock";

	//public int Skill_Gunnery = 0;
	public int Skill_Pilot = 0;
	public int Skill_Electronics = 0;

	public Shipweapon[] MyGuns;

	public Spaceship Enemy;
	public Spaceship Targetlock;	//if set to self it is nonexistant
	public List<MissileSalvo> IncomingMissiles = new List<MissileSalvo>();

	public string myBattleLog = "";

	// Use this for initialization
	void Start () {

		this.MyGuns = this.gameObject.GetComponentsInChildren<Shipweapon> ();
		this.Targetlock = this;

		this.Skill_Pilot = Mathf.RoundToInt (Random.Range (0f, 2f));
		this.Skill_Electronics = Mathf.RoundToInt (Random.Range (0f, 2f));

		//UpdateBattleLog ("---Captain " + this.CaptainName + "s battlelog for " + this.HullType + " " + this.name);

	}

	public override void GameTurn(int turnNumber)
	{
		UpdateBattleLog ("--Elapsed time: " + turnNumber*6 + " minutes, " + d6 (2) + "seconds");

		//if (this.Hullpoints <= 0 && this.gameObject.activeSelf == true)
		//this.Die();

		//MOVEMENTLOGIC

		//this.Move (this.Thrust, this.transform.forward);

		if (HasLock() && Targetlock.gameObject.activeSelf == false)	//targetlock dies
			this.Targetlock = this;
	
		if (Enemy == null) {
			UpdateBattleLog (" Scanning for new target..");
			this.SeekNewEnemy ();
			this.Attack (Enemy);
		}
		else if (Enemy.gameObject.activeSelf == true)
			//Debug.Log (this.Attack (Enemy));
			this.Attack (Enemy);
		else {
			UpdateBattleLog (" Target destroyed, seeking new target.");
			this.SeekNewEnemy ();
			this.Attack (Enemy);
		}

		this.PerformSensorAction ();
	}


	public void Attack (Spaceship Enemy)
	{

		if (Enemy != null && Enemy != this) 
		{
			//this.transform.LookAt (Enemy.transform);


			foreach (Shipweapon Gun in MyGuns) {		
				//Debug.Log (Gun.Attack (Enemy));;
				UpdateBattleLog (Gun.Attack (Enemy));
			}
		}

	}
		
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
			this.Armour = 3;
			//Debug.Log (this.name + " DANGER DANGER DA DAMAGED!");
			UpdateBattleLog ("  DANGER DANGER DANGER!");

			this.Status = "DANGER";
		}	


		if (Hullpoints <= 0) {
			
			return this.Die ("was destroyed by " + Source);
		}

		if (ActualDamage == 0) {
			UpdateBattleLog (" Under fire: " + Source + ". Armor held!");
			return (this.name + " - Armor Deflected all damage!");
		}
			
		UpdateBattleLog (" Under fire from: " + Source + "! Received " + ActualDamage + " hull damage!");

		AngerEngagingSwitchCheck (Source);

		return (this.name + " - " + ActualDamage + " hull damage!");


	}

	public bool HasLock()
	{
		if (this.Targetlock != this)
			return true;
		return false;
	}

	public void AngerEngagingSwitchCheck(string damagesource)
		{
		
		if ( (Enemy == null  )  )  //if after killing someone previously
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
		else if (this.HasLock() && Targetlock == Enemy && this.Targetlock.gameObject.activeSelf == true)
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
	public Spaceship SeekNewEnemy(){

		//Debug.Log (this.name + " SEEKING NEW ENEMY ");


		if (this.HasLock() && this.Targetlock.gameObject.activeSelf == true && this.Targetlock != this)	//like to target targetlock, duh
		{	
			Engage(Targetlock);
			return Targetlock;
		}

		foreach (Spaceship question in FindObjectsOfType<Spaceship>())
		{
			if (question.Side != "Neutral" && question.Side != this.Side && question.gameObject.activeSelf == true )
			{
				//targetinglogic here??
				if (Random.Range (0, 10) > 6) {
					Engage(question);
					return question;
				}

			}
		}

		if (d6(1)>3){
			UpdateBattleLog (" No targets found!");

			return this; //urhgtihetseith
		}
		return SeekNewEnemy(); //urhgtihetseith
		//Debug.Log (this.name + " IS VICTORIOUS! CHEEERING!");


	}

	/// <summary>
	/// Attack the specified Target ship.
	/// </summary>
	/// <param name="Target">Enemy to attack.</param>
	public void Engage(Spaceship Target)
	{
		if (this.Enemy != Target) 
		{
			this.Enemy = Target;
			//Debug.Log (this.name + " ENGAGING " + question.name);
			UpdateBattleLog (" ENGAGING " + Target.HullType + " " + Target.name);
		}
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
			if (Enemy != null && Enemy.gameObject.activeSelf == true && Targetlock != Enemy )
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

				UpdateBattleLog (" Sensor Locked " + potentialtarget.name + "!");
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

			// todo AI logic??
		}

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


	// Update is called once per frame
	public string Die () {
		return this.Die("was wrecked!");
	}

	public string Die (string how) {

		this.Status = how;

		Debug.Log (this.name + " " + how);
		UpdateBattleLog ("....Log ends");

		foreach (Spaceship toNote in FindObjectsOfType<Spaceship>()) 
		{
			
			if (toNote.Enemy != null) 
			{
				if (toNote.Enemy == this)
					toNote.UpdateBattleLog (" " + this.name + " " + how);
			}
		}

		this.gameObject.SetActive (false);

		return (this.name + " " + how);
	}
}
