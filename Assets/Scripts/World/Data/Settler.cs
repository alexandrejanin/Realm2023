using UnityEngine;

public class Settler : Unit {
    private readonly Town startingTown;

    public Settler(Tile tile, Town startingTown) : base(tile) {
        this.startingTown = startingTown;
    }

    public override void Sim() {
        var compatibility = GetCompatibility(Tile);
        Tile bestNeighbor = null;

        foreach (var neighbor in Tile.GetNeighbors(3)) {
            var neighborCompatibility = GetCompatibility(neighbor);

            if (neighborCompatibility > compatibility) {
                compatibility = neighborCompatibility;
                bestNeighbor = neighbor;
            }
        }

        if (bestNeighbor != null) {
            Tile = bestNeighbor;
        }
    }

    private float GetCompatibility(Tile tile) {
        var compatibility = startingTown.Race.GetTileCompatibility(tile);

        // Subtract compatibility based on distance to nearest
        Town nearestTown = null;
        foreach (var town in GameManager.World.Towns) {
            var distance = town.tile.DistanceTo(tile);
            if (nearestTown == null || distance < nearestTown.tile.DistanceTo(tile)) {
                nearestTown = town;
            }
        }

        if (nearestTown != null) {
            var t = Mathf.InverseLerp(0, 10, nearestTown.tile.DistanceTo(tile));
            compatibility = Mathf.Lerp(0, compatibility, t);
        }

        return compatibility;
    }
}