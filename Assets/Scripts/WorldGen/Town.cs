public class Town : Location {
	public string Name { get; }
	public Civilization civ;
	public Race Race => civ.race;
	public int population;

	public Town(Map map, Tile tile, Civilization civ, int size, int population) : base(map, tile, size, 20) {
		this.civ = civ;
		this.population = population;

		Name = Race.GetPlaceName();

		tile.customColor = UnityEngine.Color.black;
	}

	private string GetSize() {
		if (population > 5000) return "city";

		if (population > 1000) return "town";

		if (population > 500) return "village";

		return "settlement";
	}

	public void Tick() {
		population = GameController.Random.Next((int) (population * 0.98f), (int) (population * 1.02f));
	}

	public override string ToString() => $"{Name}, {Race.adjective} {GetSize()}";
}