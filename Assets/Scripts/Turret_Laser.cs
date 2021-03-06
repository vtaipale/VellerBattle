using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret_Laser : Shipweapon {

	public int WeaponMod = 4;	//modification to hit based on wweapon type
	public string GunType = "Beam Laser";
	public int DamageDice = 1;
	public int MaxRange = 9;	//Maxium range

	//Default is Beam Laser, Pulse laser is a prefab

	public override string Attacklogic(Spaceship target)
	{
		if (MyShip.IncomingMissiles.Count > 0) {
			foreach (MissileSalvo a in MyShip.IncomingMissiles) {
				if (a != null) {
					if ( a.roundsToTarget < 2) {
						return (this.PointDefence (a));
					}
				}
			}
		}
		
		return (LaserAttack (target));

	}

	private string LaserAttack(Spaceship target)
	{
		if (target == null)
			return (" " + target.name + " already destroyed");
		else if (MyShip.DistanceTo(target) > this.MaxRange)
			return (" Target out of " +this.GunType+  " range!");

		//TODO actual range modifier, should be relative easy
		int RangeMod = target.Skill_Pilot * -1; //for now

		int AttackCheck = 0;

		if (MyShip.Targetlock == target)
			AttackCheck = d6boon (2); 
		else
			AttackCheck = d6 (2); 
		
		AttackCheck += Skill_Gunnery + RangeMod + WeaponMod;

		if (AttackCheck >= 8){ //HIT!
			if (Random.Range (0, 10) > 8) 
				Debug.DrawLine(this.transform.position,target.transform.position);

			int Effect = AttackCheck - 8; //calculating Effect

			return (" HIT " + target.Damage(d6(DamageDice)+TurretAmount+Effect, MyShip.name));

		}

		if (Random.Range (0, 10) > 8)
			this.MyShip.SeekNewEnemy ();

		return (" Missed " + target.name);

	}

	private string PointDefence (MissileSalvo Target)
	{
		//MissileSalvo Target = MyShip.IncomingMissiles[0]; //should priorize moreeee but not now

		if (Target == null){
			Debug.LogWarning (this.MyShip.name + " Tried pointdefend without a target!");
			return (" Tried to pointdefend without a target!");
		}

		int DefenceCheck = d6 (2) + Skill_Gunnery + (TurretAmount - 1);

		if ((DefenceCheck >= 8) && Target != null ) {	//only if defence works and if there is smth to defend from
			if (Random.Range (0, 10) > 7) {	//not all lasers show
				Debug.DrawLine(this.transform.position,Target.transform.position);
			}

			int Effect = DefenceCheck - 7; //aka effect

			Target.ReduceMissiles (Effect);
			//Debug.Log(this.MyShip.name + " Successfully pointdefended against " + Target.name +"!");
			return (" Successfully pointdefended against " + Target.name +"!");

				
		}	

		//Debug.Log(this.MyShip.name + " Failed pointdefence!");
		return (" Failed pointdefence! ");


	}

}
