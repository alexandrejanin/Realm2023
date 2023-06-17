using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WorldGenUI : MonoBehaviour {
    [SerializeField] private Text tileInfo, mapInfo;

    private Map map;
    private MapDisplay mapDisplay;
    private WorldGenUtility worldGenUtility;

    private Tile tile;

    public static MapDrawMode DrawMode { get; private set; }
    private int mapDrawModesCount;

    private new Camera camera;

    private void Awake() {
        camera = Camera.main;
        mapDisplay = GetComponent<MapDisplay>();
        worldGenUtility = GetComponent<WorldGenUtility>();
        mapDrawModesCount = Enum.GetValues(typeof(MapDrawMode)).Length;
    }

    public void OnDrawModeChanged(int value) {
        DrawMode = value < mapDrawModesCount ? (MapDrawMode) value : 0;
        mapDisplay.DrawTexture();
    }

    public void OnMapChanged() {
        map = GameController.Map;

        var mapText = $"Seed: {map.settings.seed}\nPopulation: {map.towns.Sum(t => t.population)}";

        foreach (var climate in GameController.Climates) {
            var validRegions = map.regions.Where(region => region.climate == climate).ToList();
            var regionsCount = validRegions.Count;
            if (regionsCount == 0) continue;
            var tilesCount = validRegions.Sum(region => region.Size);

            mapText += $"\n{regionsCount} {climate.name}s ({tilesCount} tiles)";
        }

        if (mapInfo != null) mapInfo.text = mapText;
    }

    private void Update() {
        if (GameController.Location != null || map == null || GameController.WorldCamera.dragged) return;

        RaycastHit hit;
        if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit)) {
            var x = Mathf.FloorToInt(hit.point.x);
            var y = GameController.Map.size - Mathf.CeilToInt(hit.point.z) - 1;

            var newTile = map.GetTile(x, y);

            if (newTile == null) return;

            if (newTile != tile) {
                tile = newTile;
                var text = $"x: {x} y: {y}" +
                           $"\nHeight: {worldGenUtility.WorldHeightToMeters(tile.height)}m ({tile.height:F2})" +
                           $"\nTemp: {worldGenUtility.TemperatureToCelsius(tile.temp)}°C ({tile.temp:F2})" +
                           $"\nRegion: {tile.region}";
                if (tile.location != null) text += $"\n{tile.location}";
                var town = tile.location as Town;
                if (town != null) text += $"\nPopulation: {town.population}";

                tileInfo.text = text;
            }

            if (Input.GetMouseButtonDown(0) && tile.location != null) {
                StartCoroutine(GameController.LoadLocation(tile.location));
            }

            if (Input.GetMouseButtonDown(1)) {
                var point = hit.point;
                point.y = GameController.WorldCamera.targetPos.y;
                GameController.WorldCamera.targetPos = point;
            }
        }
    }
}