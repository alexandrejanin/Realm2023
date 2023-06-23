using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class World {
    public readonly string seed;
    public readonly int width, height;

    public int MinDimension { get; }
    public int MaxDimension { get; }

    private readonly Tile[,] tileMap;

    private readonly List<Region> regions;
    public IEnumerable<Region> Regions => regions;

    private readonly List<Civilization> civilizations;
    public IEnumerable<Civilization> Civilizations => civilizations;

    private readonly List<Town> towns;
    public IEnumerable<Town> Towns => towns;

    public World(WorldParameters parameters, string seed) {
        this.seed = seed;

        width = parameters.width;
        height = parameters.height;

        MinDimension = Mathf.Min(width, height);
        MaxDimension = Mathf.Max(width, height);

        // Generate initial geography
        var elevation = parameters.GenerateElevation();
        var temperature = parameters.GenerateTemperature(elevation);
        var humidity = parameters.GenerateHumidity();

        tileMap = new Tile[width, height];

        // Generate tiles
        for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
                tileMap[x, y] = new Tile(
                    x,
                    y,
                    elevation[x, y],
                    temperature[x, y],
                    humidity[x, y]
                );

        // Generate regions
        regions = RegionGenerator.GenerateRegions(tileMap);

        // Generate civilizations
        civilizations = CivilizationGenerator.GenerateCivilizations(tileMap, parameters.civilizations);
        towns = civilizations.Select(c => c.capital).ToList();
    }

    public Tile GetTile(int x, int y) => IsInMap(x, y) ? tileMap[x, y] : null;

    public bool IsInMap(int x, int y) => x >= 0 && x < width && y >= 0 && y < height;
}