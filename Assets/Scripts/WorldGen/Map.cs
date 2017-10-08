using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Map {
	public int Size => settings.Size;

	public readonly MapSettings settings;

	public float[,] HeightMap { get; private set; }
	private Tile[,] tileMap;

	public readonly List<Region> regions = new List<Region>();
	public readonly List<Town> towns = new List<Town>();

	public Map(MapSettings settings) {
		this.settings = settings;
		GenerateTileMap();
		GenerateRegions();
		GenerateTowns();
	}

	public Tile GetTile(int x, int y) => IsInMap(x, y) ? tileMap[x, y] : null;

	public bool IsInMap(int x, int y) => x >= 0 && x < Size && y >= 0 && y < Size;

	public Region RandomRegion() {
		Region region = null;
		while (region == null || region.IsWater) {
			region = regions.RandomItem();
		}
		return region;
	}

	public Tile RandomTile() => RandomRegion().RandomTile();

	private void GenerateTileMap() {
		tileMap = new Tile[Size, Size];

		HeightMap = settings.GenerateHeightMap();
		float[,] tempMap = settings.GenerateTempMap(HeightMap);
		float[,] humidityMap = settings.GenerateHumidityMap();

		for (int y = 0; y < Size; y++) {
			for (int x = 0; x < Size; x++) {
				tileMap[x, y] = new Tile(x, y, HeightMap[x, y], tempMap[x, y], humidityMap[x, y]);
			}
		}
	}

	private void GenerateRegions() {
		for (int y = 0; y < Size; y++) {
			for (int x = 0; x < Size; x++) {
				Tile tile = tileMap[x, y];
				if (tile.region != null) continue;

				List<Tile> tiles = FindConnectedTilesOfSameType(tile, 1);
				Region region = new Region(tile.Climate, tiles);
				regions.Add(region);
			}
		}
	}

	private void GenerateTowns() {
		foreach (Race race in GameController.Races) {
			Town capital = new Town(RandomTile(), race, 1000, new Coord(100, 10, 100));
		}
	}

	private List<Tile> FindConnectedTilesOfSameType(Tile firstTile, int range) {
		List<Tile> tiles = new List<Tile>();
		Queue<Tile> queue = new Queue<Tile>();
		firstTile.regionPending = true;
		queue.Enqueue(firstTile);

		while (queue.Count > 0) {
			Tile tile = queue.Dequeue();
			tiles.Add(tile);

			for (int j = -range; j <= range; j++) {
				for (int i = -range; i <= range; i++) {
					Tile newTile = GetTile(tile.x + i, tile.y + j);

					if (newTile == null || newTile.Climate != tile.Climate || newTile.region != null || newTile.regionPending) continue;

					queue.Enqueue(newTile);
					newTile.regionPending = true;
				}
			}
		}

		return tiles;
	}

	public Texture2D GetTexture(MapDrawMode mapDrawMode) {
		Texture2D texture = new Texture2D(Size, Size) {filterMode = FilterMode.Point};
		Color[] colors = new Color[Size * Size];
		for (int x = 0; x < Size; x++) {
			for (int y = 0; y < Size; y++) {
				colors[x + Size * y] = GetTile(x, y).GetColor(mapDrawMode);
			}
		}
		texture.SetPixels(colors);
		texture.Apply();
		return texture;
	}
}

public enum MapDrawMode {
	Normal,
	Height,
	Temperature,
	Humidity
}