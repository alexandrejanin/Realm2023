public sealed class Town : Location {
	public readonly Race race;
	public int population;

	public Town(Tile tile, Coord size, Race race, int population) : base(race.GetPlaceName(), tile, size) {
		this.race = race;
		this.population = population;

		tile.color = UnityEngine.Color.black;
	}

	private string GetSize() {
		if (population > 5000) return "city";

		if (population > 1000) return "town";

		if (population > 500) return "village";

		return "settlement";
	}

	public void Tick() {
		population = GameController.Random.Next((int) (population * 0.95), (int) (population * 1.05));
	}

	public override string ToString() => $"{Name}, {race.adjective} {GetSize()}";
}