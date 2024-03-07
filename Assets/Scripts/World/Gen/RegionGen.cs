using System.Collections.Generic;

public static class RegionGen {
    public static List<Region> GenerateRegions(Tile[,] tileMap) {
        var width = tileMap.GetLength(0);
        var height = tileMap.GetLength(1);

        var regions = new List<Region>();

        for (var x = 0; x < width; x++) {
            for (var y = 0; y < height; y++) {
                var tile = tileMap[x, y];
                if (tile.region != null)
                    continue;

                var tiles = FindRegion(tileMap, tile, 1);
                var region = new Region(tile.Climate, tiles);
                regions.Add(region);
            }
        }

        return regions;
    }

    private static List<Tile> FindRegion(Tile[,] tileMap, Tile startingTile, int range) {
        var width = tileMap.GetLength(0);
        var height = tileMap.GetLength(1);

        var tiles = new List<Tile>();
        var queue = new Queue<Tile>();

        startingTile.regionPending = true;
        queue.Enqueue(startingTile);

        while (queue.Count > 0) {
            var tile = queue.Dequeue();
            tiles.Add(tile);

            for (var j = -range; j <= range; j++) {
                for (var i = -range; i <= range; i++) {
                    var x = tile.x + i;
                    var y = tile.y + j;
                    if (x < 0 || x >= width || y < 0 || y >= height)
                        continue;

                    var newTile = tileMap[x, y];

                    if (newTile == null || newTile.regionPending || newTile.Climate != tile.Climate ||
                        newTile.region != null) continue;

                    queue.Enqueue(newTile);
                    newTile.regionPending = true;
                }
            }
        }

        return tiles;
    }
}