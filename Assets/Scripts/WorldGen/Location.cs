public abstract class Location : Place {
	public override string Name { get; }

	public int X => tile.x;
	public int Y => tile.y;

	protected readonly Tile tile;
	public override Tile[] Tiles => new[] {tile};

	public readonly NodeGrid nodeGrid;
	public Coord Size { get; }

	protected float[,] heightMap;

	protected Location(string name, Tile tile, Coord size) : base(tile.Climate) {
		Name = name;
		this.tile = tile;
		Size = size;
		nodeGrid = new NodeGrid(size);
	}
}