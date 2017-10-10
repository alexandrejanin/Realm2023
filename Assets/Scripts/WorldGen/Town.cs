public sealed class Town : Location {
	public readonly Race race;
	public readonly int population;
	public readonly Character[] characters;

	public Town(Tile tile, Coord size, Race race, int population) : base(race.GetPlaceName(), tile, size) {
		this.race = race;
		this.population = population;
		characters = new Character[population];

		tile.color = UnityEngine.Color.black;
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