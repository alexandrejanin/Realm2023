using System.Collections.Generic;
using System.Linq;

public abstract class Location {
	public int buildingsAmount = 50;

	public readonly Map map;
	public readonly Region region;
	protected readonly Tile tile;

	public readonly int size;
	public readonly int height;

	public int[,] heightMap;
	public int steepness = 5;

	public readonly List<Character> characters = new List<Character>();
	public readonly List<Item> items = new List<Item>();
	public readonly List<Wall> walls = new List<Wall>();

	public IEnumerable<Entity> Entities => characters.Cast<Entity>().Union(items.Cast<Entity>()).Union(walls.Cast<Entity>());

	private readonly bool[,,] freeTiles;

	public void SetTileFree(Coord coord, bool free) => SetTileFree(coord.x, coord.y, coord.z, free);
	public void SetTileFree(int x, int y, int z, bool free) => freeTiles[x, y, z] = free;
	public bool GetTileFree(Coord coord) => IsInMap(coord) && freeTiles[coord.x, coord.y, coord.z];

	public bool IsInMap(Coord coord) => coord.x >= 0 && coord.x < size && coord.y >= 0 && coord.y < height && coord.z >= 0 && coord.z < size;

	protected Location(Map map, Tile tile, int size, int height) {
		this.map = map;
		this.size = size;
		this.height = height;
		this.tile = tile;
		region = tile.region;
		tile.location = this;
		freeTiles = new bool[size, height, size];
		for (int x = 0; x < size; x++) {
			for (int y = 0; y < height; y++) {
				for (int z = 0; z < size; z++) {
					freeTiles[x, y, z] = true;
				}
			}
		}
	}
}