using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Tile {
    public readonly int x, y;
    public readonly float elevation, temperature, humidity;

    public Region region;
    public Location location;

    public Climate Climate { get; private set; }

    public Color customColor = Color.clear;
    private Color color;
    private readonly Color heightColor, tempColor, humidityColor;

    public bool IsWater => Climate.isWater;

    public bool regionPending;

    public Tile(int x, int y, float elevation, float temperature, float humidity) {
        this.x = x;
        this.y = y;
        this.elevation = elevation;
        this.temperature = temperature;
        this.humidity = humidity;
        Climate = GameManager.Database.Climates.First(climate => climate.CorrectTile(this));
        color = Climate.GetColor(elevation);
        heightColor = Color.Lerp(Color.black, Color.white, elevation);
        tempColor = Color.Lerp(Color.cyan, Color.red, temperature);
        humidityColor = Color.Lerp(Color.yellow, Color.blue, humidity);
    }

    public void SetRegion(Region newRegion) {
        region = newRegion;
        Climate = region.climate;
        color = Climate.GetColor(elevation);
    }

    public Color GetColor(MapDrawMode mapDrawMode) {
        switch (mapDrawMode) {
            case MapDrawMode.Normal:
                return customColor != Color.clear ? customColor : color;
            case MapDrawMode.Height:
                return heightColor;
            case MapDrawMode.Temperature:
                return tempColor;
            case MapDrawMode.Humidity:
                return humidityColor;
            case MapDrawMode.Region:
                return IsWater ? color : region.color;
            default:
                throw new ArgumentOutOfRangeException(nameof(mapDrawMode), mapDrawMode, null);
        }
    }

    public IEnumerable<Tile> GetNeighbors(float range = 1.5f) {
        var neighbors = new List<Tile>();
        for (var i = -1; i <= 1; i++) {
            for (var j = -1; j <= 1; j++) {
                var tile = GameManager.World.GetTile(x + i, y + j);
                if (tile != null && tile != this && DistanceTo(tile) <= range)
                    neighbors.Add(tile);
            }
        }

        return neighbors;
    }

    public float DistanceTo(Tile tile)
        => Mathf.Sqrt(Mathf.Pow(tile.x - x, 2) + Mathf.Pow(tile.y - y, 2));

    public override string ToString() => Climate + " tile (" + x + ", " + y + ")";
}