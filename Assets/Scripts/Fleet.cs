using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Cameras;

public class Fleet : TravellerBehaviour {

	public Spaceship Leader;

	public string Side = "";

	//private float update = 0f;

	public Spaceship[] MyShips;

	public string Report = "";

	public bool InstantMayhem = false;

	public GameObject Destination;

	// Use this for initialization
	void Start () {

		MyShips = GetComponentsInChildren<Spaceship> ();

		Debug.Assert (MyShips.Length > 0);

		Leader = MyShips [0];

		foreach (Spaceship shippen in MyShips) {
			shippen.name = Side + "-" + Mathf.RoundToInt (Random.Range (19f, 999f)) + " " + Shipnames [(Mathf.RoundToInt (Random.value * (Shipnames.GetLength (0) - 1)))];

			shippen.Side = this.Side;

			if (shippen.CaptainName == "Haddock")
				shippen.CaptainName = LastNames [(Mathf.RoundToInt (Random.value * (LastNames.GetLength (0) - 1)))];

			shippen.UpdateBattleLog ("---Captain " + shippen.CaptainName + "s battlelog for " + shippen.HullType + " " + shippen.name);
			shippen.UpdateBattleLog (" Location: FARHO: " + shippen.transform.position);

			if (Leader == shippen) 
				shippen.UpdateBattleLog (" Commander: ME! ");
			else
				shippen.UpdateBattleLog (" Commander: " + Leader.CaptainName + " of " + Leader.name);


			if (InstantMayhem) {
				this.Alarm (shippen, "Red");
				this.Order (shippen, "Engage");
			}
		}

	}
	
	// Update is called once per frame
	void Update () {

		LeaderCheck ();

		//TODO actual FleetAI.
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
		foreach (Spaceship shippen in GetComponentsInChildren<Spaceship>())
		{
			if (shippen.gameObject.activeSelf)
				Order (shippen, WhatToOrder);
		}
	}

	public void MoveOrderAll(GameObject DestinationPoint)
	{
		if (Destination != null && Destination.tag == "MovementPoint"){
			Debug.LogWarning ("Destroying " + Destination);
			//Destroy (Destination, 1f); //there can be only one
		}
		
		Destination = DestinationPoint;

		OrderAll ("Move");

		MessageAll (" Destination: " + Destination.transform.position + ". Dist: " + Leader.DistanceTo (Destination.transform));

		foreach (Spaceship shippen in GetComponentsInChildren<Spaceship>())
		{
			if (shippen.gameObject.activeSelf)
				shippen.Destination = this.Destination.transform;
		}

	}
		
	public void MoveOrderAll(Vector3 Destination)
	{
		GameObject NuMovementPoint = (GameObject)Instantiate (new GameObject(), Destination,new Quaternion(0f,0f,0f,0f));

		NuMovementPoint.tag = "MovementPoint";

		NuMovementPoint.name = "Movementpoint of Fleet " + this.name;

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
		foreach (Spaceship shippen in GetComponentsInChildren<Spaceship> ())
		{
			if (shippen.gameObject.activeSelf)
				Alarm (shippen, WhatAlarm);
		}
	}

	private bool IsThisOurs(Spaceship question)
	{
		return (question.Side == this.Side);
	}

	public void MessageAll(string Message)
	{
		foreach (Spaceship shippen in GetComponentsInChildren<Spaceship> ())
		{
			if (shippen.gameObject.activeSelf)
				shippen.UpdateBattleLog (" Fleet: " + Message);
		}
	}

	public string GetMiniReports ()
	{
		string MiniReport = "";

		MiniReport += "Ships: " + GetComponentsInChildren<Spaceship> ().Length + "/" + MyShips.Length + "\n";

		foreach (Spaceship shippen in GetComponentsInChildren<Spaceship>())
		{
			if (shippen.gameObject.activeSelf)
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
			Report += "  Spaceships left: " + GetComponentsInChildren<Spaceship> ().Length +"\n";
		}

		Report += "  Spaceships originally: " + MyShips.Length +"\n";


		if (GetComponentsInChildren<Spaceship> ().Length > 0) {

			Report += "\n+ACTIVE+ \n";

			foreach (Spaceship shippen in GetComponentsInChildren<Spaceship>()) {
				if (shippen.gameObject.activeSelf == true) {

					Report += "----- " + shippen.name
					+ "\n CPT:     " + shippen.CaptainName
					+ "\n TYPE:    " + shippen.HullType
					+ "\n STATUS:  " + shippen.Status + "\n";
				}
			}
		}

		if (GetComponentsInChildren<Spaceship> ().Length < MyShips.Length) {
			
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
			Leader = GetComponentsInChildren<Spaceship> () [0];
			MessageAll ("Commander KIA, acting commander: " + Leader.CaptainName + " of " + Leader.name);
		}

		return false;
	}

	public void CamToFollowLeader(int DisplayNumber)
	{
		FindObjectOfType<FreeLookCam>().SetTarget(Leader.transform);
	}



	/// <summary>
	/// Checks if no ships left. If none, fleet defeated
	/// </summary>
	/// <returns><c>true</c> when Defeated <c>false</c> if still alive</returns>
	public bool DefeatCheck()
	{
		//foreach (Spaceship shippen in GetComponentsInChildren<Spaceship>()) {		}

		if (GetComponentsInChildren<Spaceship> ().Length == 0) {
			return true;
		}

		return false;
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
		"Devoted",	"Oracle",
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
