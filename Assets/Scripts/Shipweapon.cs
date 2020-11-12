using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shipweapon : TravellerBehaviour {

	public Spaceship MyShip;
	public int TurretAmount = 1;
	public int Skill_Gunnery; // invidinual gunners??
    public bool Damaged = false;

	/// <summary>
	/// Has this gun been fired this turn?
	/// </summary>
	public bool OkToFire = true;

	// Use this for initialization
	void Start () {
		MyShip = this.GetComponentInParent<Spaceship> ();	
		this.Skill_Gunnery = Mathf.RoundToInt (Random.Range (1f, 2f)) + Mathf.RoundToInt (Random.Range (1f, 2f)) - 2;

	}

	/// <summary>
	/// Attack the specified Target. 
	/// if Target == self, dont fire aka safety
	/// </summary>
	/// <param name="Target">Target.</param>
	public string Attack (Spaceship Target) {
		if (Target == null){
			return (" No target!");
		}
		else if (Target == MyShip) 
		{
			Debug.LogWarning (this.MyShip.name + " trying shoot herself!!");
			return (" Trying Touch selfshoot WTF");
		}
		else if (OkToFire == true )
		{
			if (Target == this.MyShip) {
				OkToFire = true;
				return (" No target");
			}
			else if (Target.gameObject.activeSelf == false) {
				MyShip.UpdateBattleLog (" Target is destroyed, seeking new target");
				OkToFire = false;
				return (this.Attacklogic(MyShip.SeekNewEnemy()));
			}

			OkToFire = false;
			return Attacklogic (Target);
		}

		Debug.LogWarning (this.MyShip.name + " trying to attack twice in the same round!!");
		return " Barrels hot - Cannot fire twice during the same turn!";
	}


	public virtual string Attacklogic (Spaceship Target) {
		return "lol";
	}

}
