using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;

public class Fleet : TravellerBehaviour {

	public Spaceship Leader;

    public string OfficialName;

	public string Side = "";
	public Material SideFlag;
    public Material SideMetal;

	//private float update = 0f;

	public Spaceship[] MyShips;

	public Fleet MyEnemies; //TODO: Myenemies shold be more dynamic!

	public string Report = "";

    public string CurrentOrder = "Move";
    public string CurrentAlarm = "Green";
	public bool InstantMayhem = false;

	public GameObject Destination;

    /*
    void Update()
    {

        Debug.Log(this.Side + " " + this.MyShips.Length + this.MyEnemies)

    }*/

    // Use this for initialization
    void Awake () {

        MyShips = GetMyCurrentShips();

		Debug.Assert (MyShips.Length > 0);
		
		foreach (Spaceship shippen in MyShips) {
			GenerateCommander(shippen);
		}

        this.NewFleetCommander();
		
		Commander Ourboss = GetMyFleetCommander();
        Debug.Log(this.Side + " Commanders OK " + Time.time);

        int TacticsInitiativeRoll = d6(2) + Ourboss.Skill_Tactics + Ourboss.StatBonus(Mathf.Max(Ourboss.EDU,Ourboss.INT)) - 8;
		
		foreach (Spaceship shippen in MyShips) {
			shippen.name = Side + "-" + Mathf.RoundToInt (Random.Range (19f, 999f)) + " " + Shipnames [(Mathf.RoundToInt (Random.value * (Shipnames.GetLength (0) - 1)))];

			shippen.Side = this.Side;

			shippen.Initiative = d6(2) + shippen.Thrust + TacticsInitiativeRoll;

			if (shippen.CaptainName == "Haddock")
				shippen.CaptainName = LastNames [(Mathf.RoundToInt (Random.value * (LastNames.GetLength (0) - 1)))];

			shippen.UpdateBattleLog ("---" + shippen.GetComponent<Commander>().ToString() + "s battlelog for " + shippen.HullType + " " + shippen.name);
			shippen.UpdateBattleLog (" Location: FARHO: " + shippen.transform.position);

			if (Leader == shippen) 
				shippen.UpdateBattleLog (" FLEETCOM: ME! ");
			else
				shippen.UpdateBattleLog (" FLEETCOM: " + GetMyFleetCommander() + " of " + Leader.name);

            
		}
        if (InstantMayhem == true)
        {
            MessageAll("Battlestations!");
            this.AlarmAll("Red");
            this.OrderAll("Engage");
        }
        else
        {
            this.AlarmAll(CurrentAlarm);
            this.OrderAll(CurrentOrder);
        }

        Debug.Log(this.Side + " Ships OK " + Time.time);

        //PAINT
        foreach (MeshRenderer Hully in GetComponentsInChildren<MeshRenderer>())
        {
            if (Hully.tag == "Hull")
                Hully.material = this.SideMetal;
            else if (Hully.name.Contains("Flag_"))
                Hully.material = this.SideFlag; ;
        }

        Debug.Log(this.Side + " Paint OK " + Time.time);

        //TODO Smarter enemy designation!
        if (this.Side != "Neutral")
        {
            this.ScanForEnemyFleets();
        }

        if (OfficialName == null)
            OfficialName = Side + " Fleet";

    }
	
	// Update is called once per frame
	void Update () {

		LeaderCheck ();

		//Should not be here, but based on observations!
//		foreach (Spaceship PotentialEnemy in FindObjectOfType<Spaceship>()) 
//		{
//			if (PotentialEnemy.Side != this.Side && PotentialEnemy.Side != "Neutral")
//				this.MyEnemies
//		}


		//TODO actual FleetAI.
	}

	public Spaceship GiveRandomEnemy()
	{
		Spaceship[] CurrentEnemies = MyEnemies.GetMyCurrentShips ();

		if (CurrentEnemies.Length == 0){
			Debug.Log ("ConstructingDummyShip");
			Spaceship NoEnemiesLeft = new Spaceship ();
			NoEnemiesLeft.name = "lolz";
			return NoEnemiesLeft;
		}

		int Returnoitava = Mathf.RoundToInt (Random.Range (0f, CurrentEnemies.Length - 1f));
		return MyEnemies.GetMyCurrentShips()[Returnoitava];
	}

	public Spaceship[] GetMyCurrentShips()
	{
        //Debug.Log(GetComponentsInChildren<Spaceship>());
		return GetComponentsInChildren<Spaceship>();
	}

    public void AllEngageRandom()
    {
        Spaceship[] MyCurrentShips = this.GetMyCurrentShips();

        foreach (Spaceship Ourship in MyCurrentShips) { 
            Engage(Ourship, GiveRandomEnemy());
        }
        Debug.Log(this.name + " All " + MyCurrentShips.Length + " ships Engage Random!");

	}

	public void Engage(Spaceship OurShip, Spaceship EnemyShip)
	{
		if (IsThisOurs (OurShip) && EnemyShip.Side != this.Side) {
		
			OurShip.Engage (EnemyShip);

		}
		
	}

	public void Order(Spaceship WhoToOrder, string WhatToOrder)
	{
		if (IsThisOurs(WhoToOrder)) {
			WhoToOrder.Order = WhatToOrder;
			WhoToOrder.UpdateBattleLog (" Fleet Order: " + WhatToOrder.ToUpper ());
		}
	}

	public void OrderAll(string WhatToOrder)
	{
        CurrentOrder = WhatToOrder;

        Spaceship[] MyCurrentShips = this.GetMyCurrentShips();

        foreach (Spaceship shippen in MyCurrentShips)
		{
           // Debug.Log(shippen.name + ": " + WhatToOrder + "!");
            Order (shippen, WhatToOrder);
		}

        Debug.Log(this.name + " All " + MyCurrentShips.Length + " ships " + WhatToOrder + "!");

    }

    public void MoveOrderAll(GameObject DestinationPoint)
	{
		if (Destination != null && Destination.tag == "MovementPoint" && DestinationPoint != Destination){
			Debug.LogWarning ("Destroying " + Destination);
			Destroy (Destination.gameObject, 1f); //there can be only one
		}
		
		Destination = DestinationPoint;

		OrderAll ("Move");

		MessageAll ("Destination: " + Destination.transform.position + ". Dist: " + Leader.DistanceTo (Destination.transform));

        Spaceship[] MyCurrentShips = this.GetMyCurrentShips();

        foreach (Spaceship shippen in MyCurrentShips)
		{
			if (shippen.gameObject.activeSelf)
				shippen.Destination = this.Destination.transform;
		}


        Debug.Log(this.name + " All " + MyCurrentShips.Length + " moving to " +DestinationPoint.transform + "!");
    }
		
	public void MoveOrderAll(Vector3 Destination)
	{
        Destination = Destination ; //a bit of randomness

        GameObject NuMovementPoint = (GameObject)Instantiate (new GameObject(), Destination,new Quaternion(0f,0f,0f,0f));

		NuMovementPoint.tag = "MovementPoint";

		NuMovementPoint.name = "MP | x:" + Mathf.RoundToInt(NuMovementPoint.transform.position.x) + " y:" + Mathf.RoundToInt(NuMovementPoint.transform.position.y) + " z:" + Mathf.RoundToInt(NuMovementPoint.transform.position.z);

		this.MoveOrderAll (NuMovementPoint);

	}

	public void Alarm(Spaceship WhoToAlarm, string WhatAlarm)
	{
		if (IsThisOurs(WhoToAlarm)) 
		{
			if (WhoToAlarm.ChangeAlarm (WhatAlarm) == false) {
			}	//TODO somethin here.
		}
	}
	public void AlarmAll(string WhatAlarm)
	{
        CurrentAlarm = WhatAlarm;

        if (WhatAlarm == "Green")
            this.MyEnemies = null;

        Spaceship[] MyCurrentShips = this.GetMyCurrentShips();
        
        foreach (Spaceship shippen in MyCurrentShips)
		{
            //Debug.Log(shippen + " ships: " + WhatAlarm + " Alarm!");
            if (shippen.gameObject.activeSelf)
				Alarm (shippen, WhatAlarm);
		}

        Debug.Log(this.name + " All " + MyCurrentShips.Length + " ships: " + WhatAlarm + " Alarm!");
    }














	//TODO MissileSalvoDetector!





	private bool IsThisOurs(Spaceship question)
	{
		return (question.Side == this.Side);
	}

	public void MessageAll(string Message)
	{
		foreach (Spaceship shippen in GetMyCurrentShips())
		{
			if (shippen.gameObject.activeSelf)
				shippen.UpdateBattleLog (" Fleet: " + Message);
		}
	}

	public string GetMiniReports ()
	{
		string MiniReport = "";

        if (GetMyCurrentShips().Length == 0)
        {
            MiniReport += "Defeat!\n\n";
        }
        else
            MiniReport += "Ships: " + GetMyCurrentShips().Length + "/" + MyShips.Length + "\n\n";
        
		foreach (Spaceship shippen in FindObjectsOfType<Spaceship>())
		{
            if (shippen.Side == this.Side)
                MiniReport += shippen.MiniReport () + "\n";
		}

		return MiniReport;

	}

	public string StatusReport ()
	{
		Report = "";

		if (this.DefeatCheck () == true)
			Report += "++DEFEAT++\n";
		else {
			Report += "++VICTORY++\n";
			Report += "  Spaceships left: " + GetMyCurrentShips().Length +"\n";
		}

		Report += "  Spaceships originally: " + MyShips.Length +"\n";
        

		if (GetMyCurrentShips().Length > 0) {

			Report += "\n+ACTIVE+ \n";

			foreach (Spaceship shippen in GetMyCurrentShips()) {
				if (shippen.gameObject.activeSelf == true) {

					Report += "----- " + shippen.name
					+ "\n CPT:     " + shippen.CaptainName
					+ "\n TYPE:    " + shippen.HullType
					+ "\n STATUS:  " + shippen.Status + "\n";
				}
			}
		}

		if (GetMyCurrentShips().Length < MyShips.Length) {
			
			Report += "\n+LOST+ \n";

			foreach (Spaceship shippen in MyShips) {
				if (shippen.gameObject.activeSelf == false) {

					//shippen.gameObject.SetActive (true);

					Report += "----- " + shippen.name
					+ "\n CPT:     " + shippen.CaptainName
					+ "\n TYPE:    " + shippen.HullType
					+ "\n STATUS:  " + shippen.Status + "\n";

					//shippen.gameObject.SetActive (false);
					//Destroy (shippen, 2f);
				}
			}


		}

		return Report;

	}

	/// <summary>
	/// Is leader alive
	/// </summary>
	/// <returns><c>true</c>, if leader is OK <c>false</c> if dead and assigns nuboss.</returns>
	public bool LeaderCheck()
	{
		if (Leader.gameObject.activeSelf) {
			return true;
		}

		if (DefeatCheck() == false) 
		{
			NewFleetCommander();
			
			MessageAll ("Commander lost, acting commander: " + GetMyFleetCommander() + " of " + Leader.name);
		}

		return false;
	}

	public void CamToFollowLeader(int DisplayNumber)
	{
        FreeLookCam OurCam = FindObjectOfType<FreeLookCam>();

        OurCam.SetTarget(Leader.transform);
        OurCam.ManualUpdate();

    }



    /// <summary>
    /// Checks if no ships left. If none, fleet defeated
    /// </summary>
    /// <returns><c>true</c> when Defeated <c>false</c> if still alive</returns>
    public bool DefeatCheck()
	{
		//foreach (Spaceship shippen in GetComponentsInChildren<Spaceship>()) {		}

		if (GetMyCurrentShips().Length == 0) {
			return true;
		}

		return false;
	}

    public void SetEnemyFleet(Fleet NewEnemy)
    {
        if ((NewEnemy != null) | (NewEnemy.MyShips.Length > 0))
        { 
           MyEnemies = NewEnemy;
           this.MessageAll("New Enemy Contact:" + NewEnemy.name + " | Dist: " + Leader.DistanceTo(NewEnemy.Leader.transform));
        }
    }

    public void ScanForEnemyFleets()
    {
        if (FindObjectsOfType<Fleet>().Length > 0)

        {
            //bool returnoitava = false;
            foreach (Fleet PotentialEnemy in FindObjectsOfType<Fleet>())
            {
                if ((PotentialEnemy.Side != this.Side) && (PotentialEnemy.Side != "Neutral") && (Leader.DistanceTo(PotentialEnemy.Leader) < 300000)) //Up to Distant Edge 
                {
                    //returnoitava = true;
                    if (this.MyEnemies == null)
                        SetEnemyFleet(PotentialEnemy);
                }

            }
            //return returnoitava;
        }
        else
        {
            //No enemies found!
        }
    }

	public void GenerateCommander(Spaceship shippen){
		
		Commander NewCommander = shippen.gameObject.AddComponent<Commander>();

		if (shippen.CaptainName == "Haddock")
			shippen.CaptainName = LastNames [(Mathf.RoundToInt (Random.value * (LastNames.GetLength (0) - 1)))];
		
		NewCommander.LastName = shippen.CaptainName;
		
		NewCommander.CreateChar();
		//Debug.Log("New Char: " + NewCommander + " " +NewCommander.GetStats() + " of " + shippen.name);
	}
	
	public Commander FindTopCommander()
	{
		Commander theBest = null;
		
		foreach (Commander juub in this.GetComponentsInChildren<Commander>())
		{
			if (theBest == null)
			{
				theBest = juub;
			}
			else if (juub.rank > theBest.rank)
				theBest = juub;
		}
		
		return theBest;
	}
	
	public void NewFleetCommander()
	{
		Commander FleetCom = this.FindTopCommander();
		
		if (FleetCom == null)
		{
			Leader = GetMyCurrentShips()[0];
			GenerateCommander(Leader);
		}
		else
			Leader = FleetCom.GetMyShip();
		
		Debug.Log("New " +this.Side + " Fleetcom: " + FleetCom + " " +FleetCom.GetStats());

		
	}
	public Commander GetMyFleetCommander()
	{
		return Leader.GetComponent<Commander>();
	}

	string[] LastNames = new string[] {
		"Barrow",
		"Care",
		"Lien",
		"Hamrond",
		"Berren",
		"Fury",
		"Mestos",
		"Cotton",
		"Bener",
		"Fulgrimo",
		"Zrobsson",
		"Spielman",
		"Vindictus",
		"Tanwaukar",
		"Swartz",
		"Delifus",
		"Grimm",
		"Fermen",
		"Perho",
		"Bortsson",
		"Langred",
		"Smith",
		"Legerd",
		"Fromeo",
		"King", 
		"Orrala",
		"Nukkula",
		"Muno",
		"Wazabi",
		"Kurosava",
		"Majora",
		"Capu",
		"Oligarki",
		"Van Saarek",
		"Piggi",
		"McKilligan",
		"Kerrigan",
		"Muro",
		"Keksi",
		"Dinner",
		"Sharpe",
		"Wulf",
		"Xero",
		"Ikina",
		"Muro",
		"Konari",
		"Worry",
		"Clock",
		"Battery",
		"Agisson",
		"Borzsson",
		"Pommi",
		"Kranu",
		"Pisla",
		"Voi",
		"Lake",
		"Soldat",
		"Marsh",
		"Dinner",
		"Dinker",
		"Blovinsky",

		"Kalasnikov",
		"Rifler",
		"Barrelsson",
		"Cage",
		"Svensson",
		"Bell",
		"Dino",		
		"Baron",	//lots of most used Surnames from EU, mostly from wikipedia
		"Williams",
		"Perez",
		"Watson",
		"Wilson",
		"Taylor",
		"Journey",
		"Steward",
		"Roberts",
		"Andersson",
		"Johansson",
		"Bianchi",
		"Bernasconi",
		"Muller",
		"Meier",
		"Melnyk",
		"Shevchenko",
		"Boyko",
		"Garcia",
		"Fernandes",
		"Gonzales",
		"Novak",
		"Kovacic",
		"Nagy",
		"Jovanovic",
		"Ivanov",
		"Kuznetsov",
		"Popov",
		"Popa",
		"Radu",
		"Silva",
		"Santos",
		"Ferreira",
		"Nowak",
		"Kovalski",
		"Kaminski",
		"Hansen",
		"Olsen",
		"Lund",
		"Jensen",
		"De Jong",
		"De Vries",
		"Van den Berg",
		"Ceban",
		"Cebotari",
		"Andov",
		"Borg",
		"Camilleri",
		"Vella",
		"Schmit",
		"Muller",
		"Weber",
		"Hoffman",
		"Kazlauskas",
		"Jankauskas",
		"Petrauskas",
		"Berzins",
		"Kalnins",
		"Ozolins",
		"Zogaj",
		"Gashi",
		"Rossi",
		"Esposito",
		"Bianchi",
		"Ricci",
		"Marino",
		"Moretti",
		"Murphy",
		"O'Kelly",
		"O'Sullivan",
		"Szabo",
		"Varga",
		"Angelpoulos",
		"Nikolaidis",
		"Georgiou",
		"Petridis",
		"Martin",
		"Bernard",
		"Dubois",
		"Petit",
		"Durand",
		"Leroy",
		"Bertrand",
		"Korhonen",
		"Makinen",
		"Lehtonen",
		"Rasmussen",
		"Tamm",
		"Saar",
		"Nielsen",
		"Pedersen",
		"Babic",
		"Katzarov",
		"Peeters",
		"Maes",
		"Mammadov",
		"Gruber",
		"Huber",
		"Bauer",
		"Wagner",
		"Steiner",
		"Prifti",
		"Stehu"

	};

	string[] Shipnames = new string[] {
		"Barrow",
		"Care",
		"Warlock",
		"Silent Watcher",
		"Warrior",
		"Nightfall",
		"Juggernaut",
		"Black Cloud",
		"Galatea",
		"Cossack",
		"Desire",
		"Intervention",
		"Dominus",
		"Stormherald",
		"Intrepid",
		"Fanobano",
		"Aberdeen",
		"Menelaus",
		"Fawn",
		"Auricula",
		"Falcon",
		"Broaderschap",
		"Huntley",
		"Surge",
		"Paragon",
		"Pinnacle",
		"Devoted",	
		"Oracle",
		"Spectacle",
		"Eternal",
		"Last Squid",
		"Buccaneers",
		"Servant",
		"Mad Rift",
		"Delight",
		"Dragon Squid",
		"Deceit",
		"Cordoba",
		"Centipede",
		"Faulknor",
		"Ronaves",
		"Florizel",
		"Tigress",
		"Chignecto",
		"Highflyer",
		"Mimic",
		"Rother",
		"Pursuer",
		"Rara Avis",
		"Aetherwing",
		"Sunbird",
		"Little Miracle",
		"Euphoric Bolt",
		"Barren Muse",
		"Soft Sentinel",
		"Angry Mercy",
		"Myrmidon",
		"Templar",
		"Isabella",
		"Renault",
		"Pandora",
		"Cyclops",
		"Manticore",
		"Javelin",
		"Berserk",
		"Globetrotter",
		"Intelligence",
		"Troyness",
		"Woodpecker",
		"Patience",
		"Defiant"
	};
}
