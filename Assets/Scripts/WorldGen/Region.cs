using System;
using System.Collections.Generic;

public sealed class Region {
	public string Name => name + " " + climate;
	private readonly string name;

	public readonly Map map;
	public readonly Climate climate;
	public bool IsWater => climate.isWater;

	private readonly List<Tile> tiles;
	public int Size => tiles.Count;
	public Tile RandomTile(Random random) => tiles.RandomItem(random);

	public Region(Map map, Climate climate, List<Tile> tiles) {
		this.map = map;
		this.climate = climate;
		name = GameController.RandomRace().GetPlaceName();
		foreach (Tile tile in tiles) {
			Add(tile);
		}
		this.tiles = tiles;
	}

	public bool Contains(Tile tile) => tiles != null && tiles.Contains(tile);

	public void Add(Tile tile) {
		if (Contains(tile)) return;

		tile.region?.Remove(tile);
		tile.SetRegion(this);
	}

	public void Remove(Tile tile) {
		if (!Contains(tile)) return;

		tiles.Remove(tile);
	}

	public void Add(IEnumerable<Tile> tilesToAdd) {
		foreach (Tile tile in tilesToAdd) {
			Add(tile);
		}
	}

	public void Remove(IEnumerable<Tile> tilesToAdd) {
		foreach (Tile tile in tilesToAdd) {
			Remove(tile);
		}
	}

	public string GetSize() => tiles.Count > 10000 ? "Huge" : (tiles.Count > 5000 ? "Large" : (tiles.Count > 2000 ? "Medium" : (tiles.Count > 500 ? "Small" : "Tiny")));

	public override string ToString() => Name;
}