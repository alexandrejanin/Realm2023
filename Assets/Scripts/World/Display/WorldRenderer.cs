using UnityEngine;

[System.Serializable]
public class WorldRenderer {
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshCollider meshCollider;
    [SerializeField] private float heightMultiplier;

    public MapDrawMode mapDrawMode = MapDrawMode.Normal;

    public void DrawMap(World world) {
        var mapMesh = MeshGenerator.GenerateTerrainMesh(world, heightMultiplier);

        meshFilter.sharedMesh = mapMesh;
        meshCollider.sharedMesh = mapMesh;

        DrawTexture(world);

        Object.FindAnyObjectByType<WorldCamera>().targetPos = new Vector3(
            0,
            world.MinDimension / 2f,
            0);
    }

    public void DrawTexture(World world) {
        meshRenderer.sharedMaterial.mainTexture = GetTexture(world);
    }

    private Texture2D GetTexture(World world) {
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