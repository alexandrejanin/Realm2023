using System.Collections.Generic;

public static class CivilizationGen {
    public static List<Civilization> GenerateCivilizations(Tile[,] tileMap, int count) {
        var width = tileMap.GetLength(0);
        var height = tileMap.GetLength(1);

        var civilizations = new List<Civilization>();

        while (civilizations.Count < count) {
            var race = GameManager.Database.RandomRace();
            Tile tile;

            var attempts = 0;

            do {
                var x = GameManager.Random.Next(width);
                var y = GameManager.Random.Next(height);
                tile = tileMap[x, y];
                attempts++;
            } while (!race.IsValidTile(tile) && attempts < 100);

            if (attempts >= 100)
                continue;

            var civ = new Civilization(race);
            civilizations.Add(civ);

            var population = 5000 + (int)(race.GetTileCompatibility(tile) * 5000);
            civ.capital = new Town(tile, civ, 100, population);
        }

        return civilizations;
    }
}