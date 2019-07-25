using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret_Missile : Shipweapon {

	public int MissileRack = 12;

	public GameObject MissileToShoot; //okay well just this type for now..

	public override string Attacklogic(Spaceship target)
	{
		if (MissileRack > 0) {
			return LaunchMissile (target);
		}
		
		return " No missiles left in "+this.name +"!"; //reloading later
	}

	private string LaunchMissile(Spaceship target)
	{
		GameObject NewMissile = (GameObject)Instantiate (MissileToShoot, this.transform.position,this.transform.rotation);

		MissileSalvo NewSalvo = NewMissile.GetComponent <MissileSalvo> ();

		int HowManyToShoot = Mathf.Min (MissileRack, this.TurretAmount); //if remaining missiles < Turretamount

		NewSalvo.Launch (target, 15, HowManyToShoot, MyShip);
	
		MissileRack -= HowManyToShoot;

		return (" Launched " + HowManyToShoot + " missiles towards " + target.name);
	}
}
