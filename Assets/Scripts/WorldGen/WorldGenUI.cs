using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class WorldGenUI : MonoBehaviour {
	[SerializeField] public Dropdown drawModeDropdown;
	[SerializeField] private Text tileInfo;
	[SerializeField] private Text mapInfo;

	private Map map;
	private MapDisplay mapDisplay;
	private WorldGenUtility worldGenUtility;

	private Tile tile;

	private int x, y;

	public static MapDrawMode DrawMode { get; private set; }
	private int mapDrawModes;

	private new Camera camera;

	private void Awake() {
		drawModeDropdown.onValueChanged.AddListener(OnDrawModeChanged);
		camera = Camera.main;
		mapDisplay = GetComponent<MapDisplay>();
		worldGenUtility = GetComponent<WorldGenUtility>();
		mapDrawModes = Enum.GetValues(typeof(MapDrawMode)).Length;
	}

	private void OnDrawModeChanged(int value) {
		DrawMode = value < mapDrawModes ? (MapDrawMode) value : 0;
		mapDisplay.DrawTexture();
	}

	public void OnMapChanged() {
		map = GameController.Map;

		string mapText = $"Seed: {map.settings.seed}\nPopulation: {map.towns.Sum(t => t.population)}";

		string regionsText = "\nRegions:";

		int totalTiles = 0;

		foreach (Climate climate in GameController.Climates) {
			if (climate.isRegion) {
				int regionsCount = map.regions.Count(region => region.climate == climate);
				if (regionsCount == 0) continue;
				int tilesCount = map.regions.Where(region => region.climate == climate).Sum(region => region.Size);

				regionsText += $"\n{regionsCount} {climate.name}s ({tilesCount} tiles)";
				totalTiles += tilesCount;
			}
		}

		mapText += regionsText;
		mapText += "\n" + totalTiles + " tiles";

		mapInfo.text = mapText;
	}

	private void Update() {
		if (GameController.Location != null || map == null) return;

		RaycastHit hit;
		if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit)) {
			int tilesPerLine = (map.Size - 1) / (map.settings.Lod + 1);
			int tileHit = hit.triangleIndex / 2;
			x = tileHit % tilesPerLine;
			y = tileHit / tilesPerLine;

			Tile newTile = map.GetTile(x, y);

			if (newTile == null) return;

			if (newTile != tile) {
				tile = newTile;
				string text = $"x: {x} y: {y}" +
				              $"\nHeight: {worldGenUtility.WorldHeightToMeters(tile.height)}m ({tile.height:F2})" +
				              $"\nTemp: {worldGenUtility.TemperatureToCelsius(tile.temp)}°C ({tile.temp:F2})" +
				              $"\nRegion: {tile.region}";
				if (tile.location != null) text += $"\n{tile.location}";
				Town town = tile.location as Town;
				if (town != null) text += $"\nPopulation: {town.population}";

				tileInfo.text = text;
			}

			if (Input.GetMouseButtonDown(0) && tile.location != null) {
				StartCoroutine(GameController.LoadLocation(tile.location));
			}
		}
	}
}