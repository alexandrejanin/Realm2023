﻿using System.Collections.Generic;
using System.Linq;

public abstract class Location {
    public int buildingsAmount = 50;

    public readonly Tile tile;
    public Region Region => tile.region;
    public Climate Climate => Region.climate;

    public readonly int size;
    public readonly int height;

    public int[,] heightMap;
    public int steepness = 5;

    public readonly List<Character> characters = new();
    public readonly List<Item> items = new();
    public readonly List<Interactable> interactables = new();
    public readonly Dictionary<WallCoordinate, Wall> walls = new();

    public IEnumerable<Entity> Entities => characters.Cast<Entity>().Union(items).Union(interactables).Union(walls.Values);

    private readonly bool[,,] freeTiles;

    protected Location(Tile tile, int size, int height) {
        this.size = size;
        this.height = height;
        this.tile = tile;
        tile.location = this;
        freeTiles = new bool[size, height, size];
        for (var x = 0; x < size; x++) {
            for (var y = 0; y < height; y++) {
                for (var z = 0; z < size; z++) {
                    freeTiles[x, y, z] = true;
                }
            }
        }
    }

    public void SetTileFree(Coord coord, bool free) => SetTileFree(coord.x, coord.y, coord.z, free);
    public void SetTileFree(int x, int y, int z, bool free) => freeTiles[x, y, z] = free;
    public bool GetTileFree(Coord coord) => IsInMap(coord) && freeTiles[coord.x, coord.y, coord.z];

    public int GetHeight(Coord position) => heightMap[position.x, position.z];

    public bool IsInMap(Coord coord) => coord.x >= 0
                                        && coord.x < size
                                        && coord.y >= 0
                                        && coord.y < height
                                        && coord.z >= 0
                                        && coord.z < size;

    public void AddWall(Wall wall) => walls[wall.WallCoordinate] = wall;

    public virtual void Sim() { }
}