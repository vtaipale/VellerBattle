using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSalvo : SpaceObject {

	public Spaceship Target;
	public Spaceship source;
	public string sourceName;
	public int roundsToTarget;
	public int DamageDice = 4;
	public int AmountOfMissiles = 1;
	public int Fuel = 10;

	public string Type = "Missile";

	/*

	Standard Missile
	4d6 thrust 10

	Nuclear
	6d6, thrust 10

	multi-warhead missile HOLY SHIT amount of missiles
	3d6, thrust 10, 3x AmountofMissiles before attack :O

	Ortillery aka orbital bombardment
	1DD, thrust 10

	Long range missile
	4d6, thrust 15

	fragmentation etc...

	*/

	public bool LowOnFuel = false;
		
	// This used to be in update dammit
	public override void GameTurn(int turnNumber)
	{
		Fuel--;

		if (Fuel <= 0)
			this.EndOfLife();
		else if (Fuel == 5 ) { //half of the starting one
			LowOnFuel = true;
			this.AmountOfMissiles = Mathf.RoundToInt (this.AmountOfMissiles / 2);
		}

		if ((Target != null) && (Target.gameObject.activeSelf == true)) {

			Fly ();
		
		} else //TODO: missiles keep on flying if miss and explode after fuel runs out.
			FlyStraight();
	}

	public void Launch(Spaceship Enemy, int Distance, int HowManyMissiles, Spaceship SourceInject)
	{
		Target = Enemy;
		this.source = SourceInject;

		Enemy.IncomingMissiles.Add (this);

		this.AmountOfMissiles = HowManyMissiles;

		if (this.AmountOfMissiles == 1)
			this.name = this.Type + " from " + this.source.name;
		else
			this.name = this.Type + " salvo from " + this.source.name;

		roundsToTarget = (this.DistanceTo(Enemy) / Thrust) +1;



		Enemy.MissileLaunchDetectCheck (this);

		Fly();
	}

	public void Fly()
	{
		this.Move (this.Thrust, Target.transform.position);

		roundsToTarget = (this.DistanceTo(Target) / Thrust);

		if (roundsToTarget == 0) {
			ImpactCheck ();
			this.Target.IncomingMissiles.Remove (this);
			this.EndOfLife ();
		}
	}
	public void FlyStraight()
	{
		this.Move (this.Thrust, this.transform.position+this.transform.forward*Thrust);
		//Debug.Log ("FLYSTRHAIGHT: " + this.transform.position+this.transform.forward);
	}

	public void ImpactCheck ()
	{
		int ImpactCheck = d6(2) + AmountOfMissiles; //2d6

		if (LowOnFuel)
			ImpactCheck -= 6;


		int finalEffect = ImpactCheck - 8;

		//Smart Trait
		//finalEffect += Mathf.Min(this.TL-Target.TL, 6) //commented out because everthing is tl 12

		if (finalEffect < 0)
		{
			//MISSED! (can attack again next turn?
		}
		else if (finalEffect == 0 | AmountOfMissiles == 1) {	//straight damage is done if Effect is 0
			int Damage = d6 (DamageDice);

			if (source.gameObject.activeSelf) {	//report back to homeship, if it is alive!
					source.UpdateBattleLog (" " + this.Type + " hit " + Target.name + " for " + Damage + " Damage!");
			}
			Target.IncomingMissiles.Remove (this);

			Target.Damage(Damage,this.name);
			//Debug.Log (this.name + " hit " + Target);

		}
		else if (finalEffect > 0) { // Damage is way larger if Effect > 0

			if (finalEffect > AmountOfMissiles)
				finalEffect = AmountOfMissiles;

			int Damage = ( d6(DamageDice) - Target.Armour) * finalEffect;

			if (source.gameObject.activeSelf) {//report back to homeship, if it is alive!
					source.UpdateBattleLog (" " + this.Type + " salvo hit " + Target.name + " for " + Damage + " Damage!");
			}
			Target.IncomingMissiles.Remove (this);

			Target.Damage(Damage, this.name);
			//Debug.Log (this.name + " hit " + Target +" for " + Damage + " Damage!");


		}
			
	}

	public void ReduceMissiles(int amount)
	{
		AmountOfMissiles -= amount;

		if (AmountOfMissiles <= 0) {
			Target.IncomingMissiles.Remove (this);
			this.EndOfLife ();
		}
	}



	public void EndOfLife()
	{
		//TODO explosion for missile?
		Destroy (this.gameObject,0.4f);
	}
}
