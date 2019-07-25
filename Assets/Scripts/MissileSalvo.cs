using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSalvo : SpaceObject {

	public Spaceship Target;
	public Spaceship source;
	public int roundsToTarget;
	public int Thrust = 10;
	public int DamageDice = 4;
	public int AmountOfMissiles = 1;

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
		if ((Target != null) && (Target.gameObject.activeSelf == true)) {

			Fly ();
		
		} else
			Destroy (this.gameObject);
	}

	public void Launch(Spaceship Enemy, int Distance, int HowManyMissiles, Spaceship SourceInject)
	{
		Target = Enemy;
		this.source = SourceInject;

		this.transform.SetParent (Enemy.transform);

		AmountOfMissiles = HowManyMissiles;

		if (this.AmountOfMissiles == 1)
			this.name = this.Type + " from " + this.source.name;
		else
			this.name = this.Type + " salvo from " + this.source.name;

		roundsToTarget = (Distance / 10) +1;

		if (Distance > 44) {	//no launches futhrer!
			LowOnFuel = true; //couldbechecked with fly but meh.
		}

		Enemy.MissileLaunchDetectCheck (this);

		Fly();
	}

	public void Fly()
	{
		roundsToTarget--;

		if (roundsToTarget == 0) {
			ImpactCheck ();
			Destroy (this.gameObject);
		} else if (roundsToTarget == 1)
			this.Target.IncomingMissiles += AmountOfMissiles;
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
				this.Target.IncomingMissiles -= AmountOfMissiles;
				Destroy (this.gameObject); //MISS!
		}
		else if (finalEffect == 0 | AmountOfMissiles == 1) {
			int Damage = d6 (DamageDice);

			Target.Damage(Damage,this.name);
			Debug.Log (this.name + " hit " + Target);
			source.UpdateBattleLog( " " + this.Type  + " hit " + Target.name +" for " + Damage + " Damage!");

		}
		else if (finalEffect > 0) {

			if (finalEffect > AmountOfMissiles)
				finalEffect = AmountOfMissiles;

			int Damage = ( d6(DamageDice) - Target.Armour) * finalEffect;

			source.UpdateBattleLog( " " + this.Type  + " salvo hit " + Target.name +" for " + Damage + " Damage!");

			Target.Damage(Damage, (this.name));
			//Debug.Log (this.name + " hit " + Target +" for " + Damage + " Damage!");

		}

		this.Target.IncomingMissiles -= AmountOfMissiles;

	}

	public void ReduceMissiles(int amount)
	{
		AmountOfMissiles -= amount;

		if (AmountOfMissiles <= 0)
			Destroy (this.gameObject);
	}




}
