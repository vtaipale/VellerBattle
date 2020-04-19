using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fleet : TravellerBehaviour {

	public Spaceship Leader;

	public string Side = "";

	private float update = 0f;

	public Spaceship[] MyShips;

	public string Report = "";

	// Use this for initialization
	void Start () {

		MyShips = GetComponentsInChildren<Spaceship> ();

		foreach (Spaceship shippen in MyShips)
		{
			shippen.name = Side + "-" + Mathf.RoundToInt (Random.Range (19f, 999f)) + " " + Shipnames [(Mathf.RoundToInt (Random.value * (Shipnames.GetLength (0) - 1)))];

			shippen.Side = this.Side;

			if (shippen.CaptainName == "Haddock") 
				shippen.CaptainName = LastNames [(Mathf.RoundToInt (Random.value * (LastNames.GetLength (0) - 1)))];

			shippen.UpdateBattleLog ("---Captain " + shippen.CaptainName + "s battlelog for " + shippen.HullType + " " + shippen.name);

		}

	}
	
	// Update is called once per frame
	void Update () {
		
		update += Time.deltaTime;
		if (update > 1.0f)
		{
			update = 0.0f;
			DefeatCheck ();
		}	
	}

	public string StatusReport ()
	{
		Report = "";

		if (this.DefeatCheck () == true)
			Report += "++DEFEAT++";
		else {
			Report += "++VICTORY++";
			Report += "  Spaceships left: " + GetComponentsInChildren<Spaceship> ().Length;
		}

		foreach (Spaceship shippen in MyShips)
		{
			Report += "----- " + shippen.name
			+ "\n CPT:     " + shippen.CaptainName
			+ "\n TYPE:    " + shippen.HullType
			+ "\n STATUS:  " + shippen.Status + "\n";
		}

		return Report;

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
