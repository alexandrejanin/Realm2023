using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldVis {
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshCollider meshCollider;
    [SerializeField] private float heightMultiplier;

    [SerializeField] private UnitObject unitPrefab;

    private readonly Dictionary<Unit, UnitObject> unitObjects = new();

    private MapDrawMode mapDrawMode = MapDrawMode.Normal;

    public void Vis(World world) {
        foreach (var unit in world.Units) {
            if (!unitObjects.ContainsKey(unit)) {
                var unitObject = Object.Instantiate(unitPrefab, meshFilter.transform);

                unitObject.SetUnit(unit);
                unitObjects.Add(unit, unitObject);
            }

            unitObjects[unit].Vis();
        }
    }

    public Vector3 TileToVector3(Tile tile) {
        return new Vector3(
            tile.x + 0.5f,
            tile.elevation * heightMultiplier,
            tile.y + 0.5f
        );
    }

    public void SetMapDrawMode(MapDrawMode mode) => mapDrawMode = mode;

    public void DrawMap(World world) {
        var mapMesh = MeshGenerator.GenerateTerrainMesh(world, heightMultiplier);

        meshFilter.sharedMesh = mapMesh;
        meshCollider.sharedMesh = mapMesh;

        DrawTexture(world);

        Object.FindAnyObjectByType<WorldCamera>()
            .MoveTo(new Vector3(world.width / 2f, world.MinDimension / 2f, world.height / 2f));
    }

    public void DrawTexture(World world) {
        meshRenderer.sharedMaterial.mainTexture = GenerateTexture(world);
    }

    private Texture2D GenerateTexture(World world) {
        var texture = new Texture2D(world.width, world.height) {
            filterMode = FilterMode.Point
        };

        var colors = new Color[world.width * world.height];

        for (var x = 0; x < world.width; x++) {
            for (var y = 0; y < world.height; y++) {
                colors[x + world.width * y] = world.GetTile(x, y).GetColor(mapDrawMode);
            }
        }

        texture.SetPixels(colors);
        texture.Apply();
        return texture;
    }
}