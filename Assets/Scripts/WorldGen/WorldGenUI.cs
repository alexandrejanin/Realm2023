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

	private Tile tile;

	private Plane plane = new Plane(Vector3.up, Vector3.zero);

	private int x, y;

	public static MapDrawMode DrawMode { get; private set; }
	private int mapDrawModes;

	private void Awake() {
		drawModeDropdown.onValueChanged.AddListener(OnDrawModeChanged);
		mapDisplay = GetComponent<MapDisplay>();
		mapDrawModes = Enum.GetValues(typeof(MapDrawMode)).Length;
	}

	private void OnDrawModeChanged(int value) {
		DrawMode = value < mapDrawModes ? (MapDrawMode) value : 0;
		mapDisplay.DrawTexture();
	}

	public void OnMapChanged() {
		map = GameController.Map;

		string mapText = "Seed: " + map.settings.seed;

		string regionsText = "\nRegions:";

		int totalTiles = 0;

		foreach (Climate climate in GameController.Climates) {
			if (climate.isRegion) {
				int regionsCount = map.regions.Count(region => region.climate == climate);
				int tilesCount = map.regions.Where(region => region.climate == climate).Sum(region => region.Size);

				regionsText += $"\n{regionsCount} {climate.name}s {tilesCount} tiles)";
				totalTiles += tilesCount;
			}
		}

		mapText += regionsText;
		mapText += "\n" + totalTiles + " tiles";

		mapInfo.text = mapText;
	}

	private void Update() {
		if (map == null) return;

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float dist;

		plane.Raycast(ray, out dist);
		Vector3 pos = ray.GetPoint(dist);

		x = Mathf.FloorToInt(pos.x / 256 * map.Size);
		y = -Mathf.CeilToInt(pos.z / 256 * map.Size);

		Tile newTile = map.GetTile(x, y);
		if (newTile == null || newTile == tile) return;

		tile = newTile;

		string places = tile.places.Aggregate("\n", (current, place) => current + place + "\n");

		string tileText = $"x: {x}\n" +
		                  $"y: {y}" +
		                  $"\nHeight: {GetComponent<WorldGenUtility>().WorldHeightToMeters(tile.height)}m ({tile.height:F2})" +
		                  $"\nTemp: {GetComponent<WorldGenUtility>().TemperatureToCelsius(tile.temp)}° ({tile.temp:F2})" +
		                  $"\nRegion: {tile.region}" +
		                  places;

		tileInfo.text = tileText;
	}
}