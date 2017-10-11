using System.Collections.Generic;
using System.Linq;

public abstract class Location : Place {
	public override string Name { get; }

	public int X => tile.x;
	public int Y => tile.y;

	protected readonly Tile tile;
	public override Tile[] Tiles => new[] {tile};

	public Coord Size { get; }

	protected float[,] heightMap;

	public readonly List<Character> characters = new List<Character>();
	public readonly List<Item> items = new List<Item>();
	public readonly List<Wall> walls = new List<Wall>();

	public IEnumerable<Entity> Entities => characters.Cast<Entity>().Union(items.Cast<Entity>()).Union(walls.Cast<Entity>());

	protected Location(string name, Tile tile, Coord size) : base(tile.Climate) {
		Name = name;
		this.tile = tile;
		Size = size;
	}
}