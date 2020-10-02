using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Charater who is in command of the Fleet.
/// </summary>
public class Commander : TravellerBehaviour {


	public string FirstName = "Alice";
	public string LastName = "Haddock";
	public int rank = 1;
	public int age = 22; //assuming somewhat competent dudes.

	public int Skill_Blade = 1; //Everyone is an ensign
	public int Skill_Leadership = 0;
	public int Skill_Tactics = 0;
	public int Morale = 7;
		//add others if nessessry

	public int INT = 7;
	public int EDU = 7;
	public int SOC = 7;

	// Use this for initialization
	public override string ToString() {
		return (this.GetRank() + " " + this.FirstName + " " + this.LastName);
	}

	public void CreateChar()
	{
		//assumption that name comes from ship/fleet
		
		this.INT = d6 (2);
		this.EDU = d6 (2);
		this.SOC = d6 (2);

		//TODO NavyAcademycheck

		if ((d6(2)+StatBonus(INT)) >= 6)
		{
			this.FourYearTerm (1);	//Agtually got into the darn Navy 
			//this.name = this.ToString();
		}
		else
			this.CreateChar();
		
	}

	public void FourYearTerm(int TermNumber)
	{
		age += 4;

		int skillroll = d6 (1);

		if (skillroll == 1)
			Skill_Leadership++;
		else if (skillroll == 2)
			Skill_Tactics++;
		else if (skillroll == 3)
			INT++;
		else if (skillroll == 4)
			EDU++;
		else if (skillroll == 5)
			Morale++;

		//Does not learn anything useful with 6


		int AdvancementRoll = d6 (2) + StatBonus (EDU);

		if ((AdvancementRoll >= 7) && (rank < 6 )) { //rankup
			rank++;
			if (rank == 1)
				Skill_Leadership = Mathf.Max (Skill_Leadership, 1);
			else if (rank == 4)
				Skill_Tactics = Mathf.Max (Skill_Tactics, 1);
			else if (rank == 5)
				SOC = Mathf.Max (10, SOC+1);
			else if (rank == 6)
				SOC = Mathf.Max (12, SOC+1);
		}

		if ((d6(2)+StatBonus(INT)) >= 5 && (AdvancementRoll >= TermNumber) ) //survival + letgocheck
			this.FourYearTerm (TermNumber+1);
	}

	public int StatBonus(int Stat)
	{
		if (Stat >= 18)
			return 4;
		else if (Stat >= 15)
			return 3;
		else if (Stat >= 12)
			return 2;
		else if (Stat >= 9)
			return 1;
		else if (Stat >= 6)
			return 0;
		else if (Stat >= 3)
			return -1;
		else if (Stat >= 1)
			return -2;

		return 0;
	}

	public string GetRank()
	{
		if (rank == 1){
			return "Ensign";
		}
		else if (rank == 2){
			return "Sublieutenant";
		}	
		else if (rank == 3){
			return "Lieutenant";
		}
		else if (rank == 4){
			return "Commander";
		}
		else if (rank == 5){
			return "Captain";
		}
		else if (rank == 6){
			return "Admiral";
		}
		else if (rank == 7){
			return "Star General";	//Herzog
		}
		
		
		return "Admiral-of-Admirals";	//A-o-A
		
	}
	
	public Spaceship GetMyShip()
	{
		return GetComponent<Spaceship>();
	}
	
	public string GetStats()
	{
		return ("" + INT + EDU + SOC + "-" + Skill_Blade + Skill_Leadership + Skill_Tactics);
	}
}
