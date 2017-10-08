using System.Collections.Generic;
using System.Xml.Linq;

public static class RiverPathfinding {

	public static List<Tile> FindRiverPath(Map mapData, Tile firstTile) {
		List<Tile> allTiles = FindRiverTiles(mapData, firstTile);
		Tile goal = allTiles[allTiles.Count - 1];


		List<Tile> open = new List<Tile>();
		List<Tile> closed = new List<Tile>();

		open.Add(firstTile);

		/*while (open.Count > 0) {
			//Find tile with lowest F
			Tile q = open[0];
			open.Remove(q);
			float minF = float.MaxValue;
			foreach (Tile tile in open) {
				if (tile.f < minF) {
					q = tile;
					minF = tile.f;
				}
			}
			List<Tile> children = new List<Tile>();
			foreach (Tile tile in allTiles) {
				if (tile.DistanceTo(q) < 2) {
					//If is adjacent
					tile.parent = q;
					children.Add(tile);
				}
			}

			foreach (Tile child in children) {
				if (child == goal) {
					break;
				}

				float tempG = q.g + child.DistanceTo(q);
				float tempF = tempG + child.DistanceTo(goal);
				if (tempF < child.f) {
					child.g = tempG;
					child.f = tempF;
				}

				if (!open.Contains(child) && !closed.Contains(child)) {
					open.Add(child);
				}
			}

			if (q.height > q.parent.height) {
				q.height = q.parent.height;
			}

			closed.Add(q);
		}*/
		return closed;
	}

	private static List<Tile> FindRiverTiles(Map mapData, Tile firstTile) {
		List<Tile> tiles = new List<Tile>();

		List<Tile> scannedTiles = new List<Tile>();

		Tile currentTile = firstTile;
		bool endLoop = false;
		const float roundFactor = 1000;


		while (!endLoop) {
			tiles.Add(currentTile);

			Tile nextTile = null;
			float maxHeight = float.MaxValue;
			int range = 1;

			while (nextTile == null) {
				for (int j = -range; j <= range; j++) {
					for (int i = -range; i <= range; i++) {
						int x = currentTile.x + i;
						int y = currentTile.y + j;
						Tile testTile = mapData.GetTile(x, y);
						if (testTile != null && !scannedTiles.Contains(testTile)) {
							if (testTile.height < maxHeight && !tiles.Contains(testTile)) {
								nextTile = testTile;
								maxHeight = nextTile.height;
							}
							scannedTiles.Add(testTile);
						}
					}
				}
				range++;
			}
			/*
			if (nextTile.height > currentTile.height) {
				nextTile.height = currentTile.height;
			}*/

			tiles.Add(nextTile);

			if (nextTile.IsWater) {
				endLoop = true;
			}

			currentTile = nextTile;
		}

		return tiles;
	}
}