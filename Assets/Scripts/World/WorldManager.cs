using UnityEngine;

[System.Serializable]
public class WorldManager {
    public World World { get; private set; }

    [SerializeField] private WorldGen gen;
    [SerializeField] private WorldSim sim;
    [SerializeField] private WorldVis vis;

    public Vector3 TileToVector3(Tile tile) => vis.TileToVector3(tile);

    public void GenerateWorld() {
        World = gen.GenerateWorld();
        vis.DrawMap(World);
    }

    public void SimulateWorld() {
        sim.Sim(World);
    }

    public void UpdateVis() {
        vis.Vis(World);
    }

    public void SetMapDrawMode(MapDrawMode mode) {
        vis.SetMapDrawMode(mode);
        vis.DrawMap(World);
    }
}