using UnityEngine;

[System.Serializable]
public class WorldManager {
    public World World { get; private set; }

    [SerializeField] private WorldGen gen;
    [SerializeField] private WorldSim sim;
    [SerializeField] private WorldVis vis;

    public void GenerateWorld() {
        World = gen.GenerateWorld();
        vis.DrawMap(World);
    }

    public void SetMapDrawMode(MapDrawMode mode) {
        vis.SetMapDrawMode(mode);
        vis.DrawMap(World);
    }
}