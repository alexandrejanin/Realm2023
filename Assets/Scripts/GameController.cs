using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class GameController : MonoBehaviour {
	private static GameController Instance => instance ?? (instance = FindObjectOfType<GameController>());
	private static GameController instance;

	[Header("Map Settings"), SerializeField] private bool randomSeed;
	[SerializeField] public bool autoUpdate;

	[SerializeField] private MapSettings mapSettings;

	public static Location Location { get; private set; }

	public static Race[] Races {
		get {
			if (Instance.races == null || Instance.races.Length == 0) Instance.LoadDatabase();

			return Instance.races;
		}
	}

	public static Climate[] Climates {
		get {
			if (Instance.climates == null || Instance.climates.Length == 0) Instance.LoadDatabase();

			return Instance.climates;
		}
	}

	[Header("Database"), SerializeField] private Race[] races;
	[SerializeField] private Climate[] climates;

	private static string RacesPath => Application.persistentDataPath + "/Races/";
	private static string ClimatesPath => Application.persistentDataPath + "/Climates/";

	public static Map Map { get; private set; }

	private static LocationManager locationManager;
	public static PrefabManager prefabManager;

	private static AsyncOperation loadingLevel;

	public static Random Random => random ?? (random = new Random());
	private static Random random;

	private void Awake() {
		DontDestroyOnLoad(this);

		LoadDatabase();

		locationManager = GetComponent<LocationManager>();
		prefabManager = GetComponent<PrefabManager>();

		GenerateMap();
	}

	public void GenerateMap() {
		if (randomSeed) mapSettings.seed = Random.Next(0, 99999);
		Map = new Map(mapSettings);
		GetComponent<MapDisplay>().DrawMap();
		GetComponent<WorldGenUI>().OnMapChanged();
	}

	public static IEnumerator LoadLocation(Location location) {
		if (loadingLevel != null && !loadingLevel.isDone) yield break;
		Location = location;
		loadingLevel = SceneManager.LoadSceneAsync("Local", LoadSceneMode.Single);
		while (!loadingLevel.isDone) {
			yield return null;
		}
		OnSceneLoaded();
	}

	private static void OnSceneLoaded() {
		NodeGrid.CreateGrid(Location);
		locationManager.LoadLocation(Location);
		ObjectManager.TakeTurn();
	}

	public void LoadDatabase() {
		races = Directory.GetFiles(RacesPath).Select(file => JsonUtility.FromJson<Race>(File.ReadAllText(file))).ToArray();
		climates = Directory.GetFiles(ClimatesPath).Select(file => JsonUtility.FromJson<Climate>(File.ReadAllText(file))).ToArray();
	}

	public void SaveDatabase() {
		foreach (Race race in races) {
			File.WriteAllText(RacesPath + race + ".json", JsonUtility.ToJson(race, true));
		}
		foreach (Climate climate in climates) {
			File.WriteAllText(ClimatesPath + climate + ".json", JsonUtility.ToJson(climate, true));
		}
	}

	public static Race RandomRace() => Races.RandomItem();
}