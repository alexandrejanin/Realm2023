using UnityEngine;
using UnityEngine.Rendering;

public static class MeshGenerator {
    public static Mesh GenerateTerrainMesh(World world, float multiplier) {
        var topLeftX = (world.width - 1) / -2f;
        var topLeftZ = (world.height - 1) / 2f;

        var meshData = new MeshData(world.height, world.width);
        var vertexIndex = 0;

        for (var y = 0; y < world.height; y++) {
            for (var x = 0; x < world.width; x++) {
                var elevation = world.GetTile(x, y).elevation * multiplier;
                
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, elevation, topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)world.width, y / (float)world.height);

                if (x < world.width - 1 && y < world.height - 1) {
                    meshData.AddTriangle(vertexIndex, vertexIndex + world.width + 1, vertexIndex + world.width);
                    meshData.AddTriangle(vertexIndex + world.width + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData.CreateMesh();
    }

    private struct MeshData {
        public readonly Vector3[] vertices;
        private readonly int[] triangles;
        public readonly Vector2[] uvs;

        private int triangleIndex;

        public MeshData(int height, int width) {
            vertices = new Vector3[height * width];
            uvs = new Vector2[height * width];
            triangles = new int[(height - 1) * (width - 1) * 6];
            triangleIndex = 0;
        }

        public void AddTriangle(int a, int b, int c) {
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
        }

        public Mesh CreateMesh() {
            var mesh = new Mesh {
                indexFormat = IndexFormat.UInt32,
                vertices = vertices,
                triangles = triangles,
                uv = uvs,
            };
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}