using UnityEngine;

public class GameManager : MonoBehaviour {
    private static GameManager instance;

    private DatabaseManager databaseManager;
    public static DatabaseManager Database => instance.databaseManager;

    private PrefabManager prefabManager;
    public static PrefabManager PrefabManager => instance.prefabManager;

    [SerializeField] private WorldManager worldManager;
    public static WorldManager WorldManager => instance.worldManager;

    [SerializeField] private LocalManager localManager;
    public static LocalManager LocalManager => instance.localManager;

    public static System.Random Random { get; private set; }

    private void Awake() {
        instance = this;
        prefabManager = GetComponent<PrefabManager>();

        databaseManager = new DatabaseManager();
        databaseManager.LoadDatabase();
    }

    public void GenerateWorld(string seed) {
        Random = new System.Random(seed.GetHashCode());
        WorldManager.GenerateWorld(seed);
    }
}