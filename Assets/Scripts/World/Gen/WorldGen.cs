using System.Linq;
using UnityEngine;

[System.Serializable]
public class WorldGen {
    [SerializeField] private WorldParameters parameters;

    public World GenerateWorld() {
        var elevation = parameters.GenerateElevation();
        var temperature = parameters.GenerateTemperature(elevation);
        var humidity = parameters.GenerateHumidity();

        var tileMap = new Tile[parameters.width, parameters.height];
        for (var x = 0; x < parameters.width; x++)
        for (var y = 0; y < parameters.height; y++)
            tileMap[x, y] = new Tile(x, y, elevation[x, y], temperature[x, y], humidity[x, y]);

        var regions = RegionGen.GenerateRegions(tileMap);

        var civilizations = CivilizationGen.GenerateCivilizations(tileMap, parameters.civilizations);
        var towns = civilizations.Select(c => c.capital).ToList();

        return new World(parameters.width, parameters.height, tileMap, regions, civilizations, towns);
    }
}