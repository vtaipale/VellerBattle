using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret_Missile : Shipweapon {

	public int MissileRack = 12;

	public GameObject MissileToShoot; //okay well just this type for now..

	public override string Attacklogic(Spaceship target)
	{

		if (target.DistanceTo (this.MyShip) > 100) 
		{
			return (" " + target.name + " out of Missile range!");
		}
		else if (MissileRack > 0) {
			return LaunchMissile (target);
		}
		
		return " No missiles left in "+this.name +"!"; //reloading later
	}

	private string LaunchMissile(Spaceship target)
	{
		GameObject NewMissile = (GameObject)Instantiate (MissileToShoot, this.transform.position,this.transform.rotation);

		MissileSalvo NewSalvo = NewMissile.GetComponent <MissileSalvo> ();

		int HowManyToShoot = Mathf.Min (MissileRack, this.TurretAmount); //if remaining missiles < Turretamount

		//Debug.Log ("MissileLaunch: " + this.gameObject.name + "| " + target + "|" + HowManyToShoot + "|" + MyShip  + "|" + NewSalvo);;

		NewSalvo.Launch (target, 15, HowManyToShoot, this.MyShip);
	
		MissileRack -= HowManyToShoot;

		return (" Launched " + HowManyToShoot + " missiles towards " + target.name);
	}
}
