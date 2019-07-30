﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowController : TravellerBehaviour {

	public Fleet Fleet1;
	public Fleet Fleet2;

	private float update = 0f;
	public float Gamespeed = 1.0f;

	public bool ContinueGame = true;

	int roundNumber = 0;

	public Spaceship[] AllShips;


	// Use this for initialization
	void Start () {

		AllShips = FindObjectsOfType<Spaceship> ();

		Debug.Log ("-- NEW BATTLE --");
		Debug.Log ("Counting" + FindObjectsOfType<Spaceship>().Length + " ships! \nLet the battle commerce!!!");


		//INITIATIVEJÄRJESTYS ETC

	}
	
	// Update is called once per frame
	void Update () {

		update += Time.deltaTime;
		if ((ContinueGame==true) && update > Gamespeed)
		{
			update = 0.0f;
			NextRound ();

			if (Fleet1.DefeatCheck () | Fleet2.DefeatCheck ()) {
				ContinueGame = false;
				Fleet1.StatusReport ();
				Fleet2.StatusReport ();
			}
		}
	}


	void NextRound(){

		roundNumber++;

		Debug.Log ("-- BATTLE ROUND "+roundNumber+" --");


		foreach (Shipweapon gun in FindObjectsOfType<Shipweapon>())
		{
			gun.OkToFire = true;
		}
	
		foreach (SpaceObject objecten in FindObjectsOfType<SpaceObject>())
		{
			if (objecten.gameObject.activeSelf) {
				objecten.GameTurn (this.roundNumber);

				//this.Wait
			}
		}
	}

}
