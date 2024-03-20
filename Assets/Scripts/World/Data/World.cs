using System.Collections.Generic;
using UnityEngine;

public class World {
    public readonly int width, height;

    public int MinDimension => Mathf.Min(width, height);
    public int MaxDimension => Mathf.Max(width, height);

    private readonly Tile[,] tileMap;

    private readonly List<Region> regions;
    public IEnumerable<Region> Regions => regions;

    private readonly List<Civilization> civilizations;
    public IEnumerable<Civilization> Civilizations => civilizations;

    private readonly List<Town> towns;
    public IEnumerable<Town> Towns => towns;

    private readonly List<Unit> units;
    public IEnumerable<Unit> Units => units;

    public World(int width, int height, Tile[,] tileMap, List<Region> regions, List<Civilization> civilizations, List<Town> towns, List<Unit> units) {
        this.width = width;
        this.height = height;
        this.tileMap = tileMap;
        this.regions = regions;
        this.civilizations = civilizations;
        this.towns = towns;
        this.units = units;
    }

    public Tile GetTile(int x, int y) => IsInMap(x, y) ? tileMap[x, y] : null;

    public bool IsInMap(int x, int y) => x >= 0 && x < width && y >= 0 && y < height;

    public void SpawnUnit(Unit unit) {
        units.Add(unit);
    }
}