using System.Collections.Generic;
using System.Linq;

public abstract class Location : Place {
	public override string Name { get; }

	public int X => tile.x;
	public int Y => tile.y;
	public int Width => Size.x;
	public int Height => Size.y;
	public int Length => Size.z;

	protected readonly Tile tile;
	public override Tile[] Tiles => new[] {tile};

	public Coord Size { get; }

	protected float[,] heightMap;

	public readonly List<Character> characters = new List<Character>();
	public readonly List<Item> items = new List<Item>();
	public readonly List<Wall> walls = new List<Wall>();

	public IEnumerable<Entity> Entities => characters.Cast<Entity>().Union(items.Cast<Entity>()).Union(walls.Cast<Entity>());

	private readonly bool[,,] freeTiles;

	public void SetTileFree(Coord coord, bool free) => freeTiles[coord.x, coord.y, coord.z] = free;
	public bool GetTileFree(Coord coord) => IsInMap(coord) && freeTiles[coord.x, coord.y, coord.z];

	public bool IsInMap(Coord coord) => coord.x >= 0 && coord.x < Width && coord.y >= 0 && coord.y < Height && coord.z >= 0 && coord.z < Length;

	protected Location(string name, Tile tile, Coord size) : base(tile.Climate) {
		Name = name;
		Size = size;
		this.tile = tile;
		tile.location = this;
		freeTiles = new bool[Width, Height, Length];
		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				for (int z = 0; z < Length; z++) {
					freeTiles[x, y, z] = true;
				}
			}
		}
	}
}