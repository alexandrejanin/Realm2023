using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class GameController : MonoBehaviour {
    private static GameController Instance => instance ??= FindAnyObjectByType<GameController>();
    private static GameController instance;

    private DatabaseManager database;
    public static DatabaseManager Database => Instance.database;

    private static int Seed => Instance.randomSeed ? Instance.seed = UnityEngine.Random.Range(0, 9999) : Instance.seed;
    [SerializeField] private bool randomSeed;
    [SerializeField] private int seed;

    [SerializeField] private bool screenshots;

    [Header("Map Settings"), SerializeField]
    private bool randomMapSeed;

    [SerializeField] public bool autoUpdate;

    [SerializeField] private MapSettings mapSettings;

    public static Map Map { get; private set; }

    public static Location Location { get; private set; }

    public static Blueprint[] Blueprints => Instance.blueprints;
    [SerializeField] private Blueprint[] blueprints;

    public static LocationManager LocationManager => locationManager ??= new LocationManager();
    public static MapDisplay MapDisplay => mapDisplay ??= Instance.GetComponent<MapDisplay>();
    public static WorldGenUI WorldGenUI => worldGenUI ??= Instance.GetComponent<WorldGenUI>();
    public static PrefabManager PrefabManager => prefabManager ??= Instance.GetComponent<PrefabManager>();

    public static WorldCamera WorldCamera => worldCamera ??= FindAnyObjectByType<WorldCamera>();
    public static DialogueManager DialogueManager => dialogueManager ??= FindAnyObjectByType<DialogueManager>();

    private static LocationManager locationManager;
    private static MapDisplay mapDisplay;
    private static WorldGenUI worldGenUI;
    private static PrefabManager prefabManager;

    private static WorldCamera worldCamera;
    private static DialogueManager dialogueManager;

    private static AsyncOperation loadingLevel;

    public static Random Random => random ??= new Random(Seed);
    private static Random random;

    private void Awake() {
        DontDestroyOnLoad(this);

        database = new DatabaseManager();

        GenerateMap();
    }

    public void OnMapSizeChanged(int i) {
        mapSettings.mapSize = (MapSize)i;
        GenerateMap();
    }

    public void GenerateMap() {
        if (randomMapSeed) mapSettings.seed = Random.Next(0, 99999);
        Map = new Map(mapSettings);
        OnMapUpdated();
    }

    private static void OnMapUpdated() {
        MapDisplay.DrawMap();
        WorldGenUI.OnMapChanged();
        WorldCamera.targetPos = new Vector3(Map.size / 2, Map.size / 2, Map.size / 2);
        if (Instance.screenshots) {
            ScreenCapture.CaptureScreenshot("Screenshots/" + Map.settings.seed + ".png");
        }
    }

    public static IEnumerator LoadLocation(Location location) {
        if (loadingLevel != null && !loadingLevel.isDone) yield break;
        Location = location;
        loadingLevel = SceneManager.LoadSceneAsync("Local", LoadSceneMode.Single);

        while (!loadingLevel.isDone) {
            yield return null;
        }

        NodeGrid.CreateGrid(Location);
        LocationManager.LoadLocation(Location);
        ObjectManager.TakeTurn();
    }
}
