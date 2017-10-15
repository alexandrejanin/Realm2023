public class Civilization {
	public string Name { get; }
	public readonly Race race;
	public Town capital;

	public Civilization(Race race) {
		this.race = race;
		Name = race.GetPlaceName();
	}
}