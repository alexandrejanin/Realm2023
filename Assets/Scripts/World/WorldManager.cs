using UnityEngine;

[System.Serializable]
public class WorldManager {
    public World World { get; private set; }

    [SerializeField] private WorldGenerator generator;
    [SerializeField] private WorldRenderer worldRenderer;
    
    public void GenerateWorld(string seed) {
        World = generator.GenerateWorld(seed);
        worldRenderer.DrawMap(World);
    }
}