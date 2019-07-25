using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spaceship : SpaceObject {

	public string HullType = "Patrol Corvette";
	public int Hullpoints = 160;
	public int HullpointsOrig = 160;
	public int Armour = 4;
	public string Side; //more complex?
	public string Status = "OK";

	//public Captain = Haddoc
	public string CaptainName = "Haddock";

	//public int Skill_Gunnery = 0;
	public int Skill_Pilot = 0;
	public int Skill_Electronics = 0;

	public Shipweapon[] MyGuns;

	public Spaceship Enemy;
	public int IncomingMissiles = 0;

	public string myBattleLog = "";

	// Use this for initialization
	void Start () {

		this.MyGuns = this.gameObject.GetComponentsInChildren<Shipweapon> ();

		this.Skill_Pilot = Mathf.RoundToInt (Random.Range (0f, 2f));
		this.Skill_Electronics = Mathf.RoundToInt (Random.Range (0f, 2f));

		//UpdateBattleLog ("---Captain " + this.CaptainName + "s battlelog for " + this.HullType + " " + this.name);

	}

	public override void GameTurn(int turnNumber)
	{
		UpdateBattleLog ("--Elapsed time: " + turnNumber*6 + " minutes, " + d6 (2) + "seconds");

		//if (this.Hullpoints <= 0 && this.gameObject.activeSelf == true)
		//this.Die();

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
	}


	public void PointDefenceNeeded(int amount)
	{
		this.IncomingMissiles += amount;
	}

	public void Attack (Spaceship Enemy)
	{

		foreach (Shipweapon Gun in MyGuns)
		{		
			//Debug.Log (Gun.Attack (Enemy));;
			UpdateBattleLog(Gun.Attack(Enemy));
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

			if (Random.Range (0, 10) > 5) {
				MyGuns [Mathf.RoundToInt (Random.Range (0f, 3f))].Skill_Gunnery -=  1; //wounded crewmember
			} else if (Random.Range (0, 10) > 5) {
				MyGuns [Mathf.RoundToInt (Random.Range (0f, 3f))].gameObject.SetActive (false); //disabled gun
			}
			else
				this.Hullpoints -= d6(2);
		}	


		if (Hullpoints <= 0) {
			
			return this.Die ("was destroyed by " + Source);
		}

		if (ActualDamage == 0) {
			UpdateBattleLog (" Under fire: " + Source + ". Armor held!");
			return (this.name + " - Armor Deflected all damage!");
		}
			
		UpdateBattleLog (" Under fire: " + Source + " - took " + ActualDamage + " hull damage!");

		AngerEngagingSwitchCheck (Source);

		return (this.name + " - " + ActualDamage + " hull damage!");


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

	public Spaceship SeekNewEnemy(){

		//Debug.Log (this.name + " SEEKING NEW ENEMY ");


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

		if (d6(1)>3)
			return this; //urhgtihetseith
		
		return SeekNewEnemy(); //urhgtihetseith
		//Debug.Log (this.name + " IS VICTORIOUS! CHEEERING!");


	}

	public void Engage(Spaceship Target)
	{
		this.Enemy = Target;
		//Debug.Log (this.name + " ENGAGING " + question.name);
		UpdateBattleLog (" ENGAGING " + Target.HullType + " "+ Target.name);
	}

	/// <summary>
	/// Updates the battle log of the ship.
	/// </summary>
	/// <param name="Newline">Newline.</param>
	public void UpdateBattleLog(string Newline)
	{
		if (Newline.Contains ("No missiles left") == false )
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
