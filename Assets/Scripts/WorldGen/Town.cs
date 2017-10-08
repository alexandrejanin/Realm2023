using UnityEngine;

public class Town : Place {
	public override string Name { get; }

	public readonly Race race;
	public readonly int population;
	public readonly Coord size;
	public Character[] characters;

	public int X => tile.x;
	public int Y => tile.y;

	public readonly Tile tile;
	public override Tile[] Tiles => new[] {tile};

	public Town(Tile tile, Race race, int population, Coord size) : base(tile.Climate) {
		this.race = race;
		Name = race.GetPlaceName();
		this.population = population;
		this.size = size;
		characters = new Character[population];

		this.tile = tile;
		tile.color = Color.black;
		tile.places.Add(this);
	}

	private string GetSize() {
		if (population > 5000) return "city";

		if (population > 1000) return "town";

		if (population > 200) return "village";

		return "settlement";
	}

	public override string ToString() => $"{Name}, {race.adjective} {GetSize()}";
}