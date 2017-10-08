using System.Collections.Generic;
using UnityEngine;

public sealed class Region : Place {
	public override string Name => name + " " + climate;
	private readonly string name;

	private readonly List<Tile> tiles;
	public override Tile[] Tiles => tiles.ToArray();

	public Region(Climate climate, List<Tile> tiles) : base(climate) {
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

	public string GetSize() {
		if (tiles.Count > 10000) return "Huge";

		if (tiles.Count > 5000) return "Large";

		if (tiles.Count > 2000) return "Medium";

		if (tiles.Count > 500) return "Small";

		return "Tiny";
	}

	public override string ToString() => Name;
}