using UnityEngine;

[System.Serializable]
public struct NoiseParameters2 {
    [Range(1, 4)] public int octaves;
    [Range(0, 1)] public float persistance;
    [Range(1, 5)] public float lacunarity;
    [Range(10, 500)] public int scale;

    public float[,] Generate(int width, int height) {
        var noiseMap = new float[width, height];
        var octaveOffsets = new Vector2[octaves];

        float amplitude = 1;

        for (var i = 0; i < octaves; i++) {
            float offsetX = GameManager.Random.Next(-99999, 99999);
            float offsetY = GameManager.Random.Next(-99999, 99999);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
            amplitude *= persistance;
        }

        var maxNoiseHeight = float.MinValue;
        var minNoiseHeight = float.MaxValue;
        var halfSize = Mathf.Min(width, height) / 2f;
        for (var x = 0; x < width; x++) {
            for (var y = 0; y < height; y++) {
                amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                for (var i = 0; i < octaves; i++) {
                    var sampleX = (x - halfSize + octaveOffsets[i].x) / scale * frequency;
                    var sampleY = (y - halfSize + octaveOffsets[i].y) / scale * frequency;
                    var perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight) {
                    maxNoiseHeight = noiseHeight;
                }

                if (noiseHeight < minNoiseHeight) {
                    minNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;
            }
        }

        for (var x = 0; x < width; x++) {
            for (var y = 0; y < height; y++) {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}