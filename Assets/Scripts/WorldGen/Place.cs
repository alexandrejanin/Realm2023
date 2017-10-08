[System.Serializable]
public abstract class Place {
	public abstract string Name { get; }
	public Climate climate;

	public bool IsWater => climate.isWater;

	public abstract Tile[] Tiles { get; }
	public Tile RandomTile() => Tiles.RandomItem();

	public int Size => Tiles.Length;

	protected Place(Climate climate) {
		this.climate = climate;
	}

	public override string ToString() => Name;
}