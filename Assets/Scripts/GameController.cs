using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour {
	private static GameController Instance => instance ?? (instance = FindObjectOfType<GameController>());
	private static GameController instance;

	[SerializeField] private bool randomSeed;
	[SerializeField] public bool autoUpdate;

	[SerializeField] private MapSettings mapSettings;

	[Header("Database")] [SerializeField] private TextAsset racesDatabase;
	[SerializeField] private TextAsset climatesDatabase;

	public static Location Location { get; private set; }

	public static Race[] Races {
		get {
			if (races == null || races.Length == 0) Instance.LoadDatabase();

			return races;
		}
	}

	public static Climate[] Climates {
		get {
			if (climates == null || climates.Length == 0) Instance.LoadDatabase();

			return climates;
		}
	}

	private static Race[] races;
	private static Climate[] climates;

	public static Map Map { get; private set; }

	private static LocationManager locationManager;
	public static PrefabManager prefabManager;

	private static AsyncOperation loadingLevel;

	private void Awake() {
		DontDestroyOnLoad(this);

		LoadDatabase();

		locationManager = GetComponent<LocationManager>();
		prefabManager = GetComponent<PrefabManager>();

		GenerateMap();
	}

	public void GenerateMap() {
		if (randomSeed) mapSettings.seed = Random.Range(0, 99999);
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
	}

	public void LoadDatabase() {
		races = DatabaseController.LoadXML<Race>(racesDatabase, "Race");
		climates = DatabaseController.LoadXML<Climate>(climatesDatabase, "Climate");
	}

	public static Race RandomRace() => Races.RandomItem();
}