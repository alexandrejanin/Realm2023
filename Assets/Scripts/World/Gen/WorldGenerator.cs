using UnityEngine;

[System.Serializable]
public class WorldGenerator {
    [SerializeField] private WorldParameters parameters;

    public World GenerateWorld(string seed) {
        return new World(parameters, seed);
    }
}