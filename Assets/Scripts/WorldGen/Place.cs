using System;

public abstract class Place {
	public abstract string Name { get; }
	public Climate climate;

	public bool IsWater => climate.isWater;

	public abstract Tile[] Tiles { get; }
	public Tile RandomTile(Random random) => Tiles.RandomItem(random);

	public int TileCount => Tiles.Length;

	protected Place(Climate climate) {
		this.climate = climate;
	}

	public override string ToString() => Name;
}