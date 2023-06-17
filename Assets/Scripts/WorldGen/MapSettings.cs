using System;
using UnityEngine;
using Random = System.Random;

[Serializable]
public struct MapSettings {
    public MapSize mapSize;

    [SerializeField] [Range(1, 2)] private int lodMultiplier;

    public int Size {
        get {
            switch (mapSize) {
                case MapSize.Tiny:
                    return 33;
                case MapSize.Small:
                    return 65;
                case MapSize.Medium:
                    return 129;
                case MapSize.Large:
                    return 257;
                case MapSize.Huge:
                    return 513;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public int Lod => Size / 256 * lodMultiplier;

    [Range(0, 99999)] public int seed;

    [Header("Heightmap"), SerializeField] private NoiseSettings heightSettings;

    [Range(0, 10)] public float falloffA;
    [Range(1, 10)] public float falloffB;
    [Range(0, 1)] public float falloffMultiplier;

    [Header("Temperature Map")] [Range(0, 10)]
    public float tempA;

    [Range(0.5f, 1.5f)] public float tempB;
    [Range(0, 1)] public float heightTempMultiplier;

    [Header("Humidity Map"), SerializeField]
    private NoiseSettings humiditySettings;

    [Header("Civilizations"), Range(1, 10)]
    public int civilizations;

    [Range(1, 1000)] public int years;

    public int[,] GenerateLocationHeightMap(Location location, NoiseSettings settings) {
        var floatMap = GenerateNoiseMap(location.size, seed * 3, settings);

        var heightMap = new int[location.size, location.size];
        for (var x = 0;
            x < location.size;
            x++) {
            for (var z = 0; z < location.size; z++) {
                heightMap[x, z] = (int) (floatMap[x, z] * location.steepness);
            }
        }

        return heightMap;
    }

    public float[,] GenerateHeightMap() {
        var heightMap = GenerateNoiseMap(Size, seed, heightSettings);
        var falloffMap = GenerateFalloffMap(Size, falloffA, falloffB);
        for (var y = 0; y < Size; y++) {
            for (var x = 0; x < Size; x++) {
                heightMap[x, y] = Mathf.Clamp01(heightMap[x, y] - falloffMap[x, y] * falloffMultiplier);
            }
        }

        return heightMap;
    }

    public float[,] GenerateHumidityMap() => GenerateNoiseMap(Size, seed / 2, humiditySettings);

    public float[,] GenerateTempMap(float[,] heightMap) {
        var minTemp = float.MaxValue;
        var maxTemp = float.MinValue;
        var tempMap = new float[Size, Size];
        var maxTempLatitude = Size;
        for (var j = 0; j < Size; j++) {
            for (var i = 0; i < Size; i++) {
                var height = heightMap[i, j];
                var heightTemp = Mathf.Abs(height - 0.5f) * heightTempMultiplier;
                var latitudeTemp = 1 - Mathf.Abs(maxTempLatitude - j) / (float) maxTempLatitude;
                var
                    temp = Mathf.Clamp01(latitudeTemp -
                                         heightTemp); //Mathf.Clamp01(Mathf.Clamp01(1 - Mathf.Abs(mapMiddle - j) / (float) mapMiddle) - heightTemp);
                temp = Evaluate(temp, tempA, tempB);
                tempMap[i, j] = temp;
                if (temp < minTemp) {
                    minTemp = temp;
                }

                if (temp > maxTemp) {
                    maxTemp = temp;
                }
            }
        }

        for (var y = 0; y < Size; y++) {
            for (var x = 0; x < Size; x++) {
                tempMap[x, y] = Mathf.InverseLerp(minTemp, maxTemp, tempMap[x, y]);
            }
        }

        return tempMap;
    }

    private static float[,] GenerateNoiseMap(int size, int seed, NoiseSettings noiseSettings) {
        var noiseMap = new float[size, size];
        var random = new Random(seed);
        var octaveOffsets = new Vector2[noiseSettings.octaves];
        float amplitude = 1;
        for (var i = 0; i < noiseSettings.octaves; i++) {
            float offsetX = random.Next(-99999, 99999);
            float offsetY = random.Next(-99999, 99999);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
            amplitude *= noiseSettings.persistance;
        }

        var maxNoiseHeight = float.MinValue;
        var minNoiseHeight = float.MaxValue;
        var halfSize = size / 2f;
        for (var y = 0; y < size; y++) {
            for (var x = 0; x < size; x++) {
                amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                for (var i = 0; i < noiseSettings.octaves; i++) {
                    var sampleX = (x - halfSize + octaveOffsets[i].x) / noiseSettings.scale * frequency;
                    var sampleY = (y - halfSize + octaveOffsets[i].y) / noiseSettings.scale * frequency;
                    var perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= noiseSettings.persistance;
                    frequency *= noiseSettings.lacunarity;
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

        for (var y = 0; y < size; y++) {
            for (var x = 0; x < size; x++) {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }

    private static float[,] GenerateFalloffMap(int size, float falloffA, float falloffB) {
        var map = new float[size, size];
        for (var i = 0; i < size; i++) {
            for (var j = 0; j < size; j++) {
                var x = i / (float) size * 2 - 1;
                var y = j / (float) size * 2 - 1;
                var value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                var falloff = Evaluate(value, falloffA, falloffB);
                map[i, j] = falloff;
            }
        }

        return map;
    }

    private static float Evaluate(float value, float a, float b) =>
        Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
}

public enum MapSize {
    Tiny,
    Small,
    Medium,
    Large,
    Huge
}

[Serializable]
public struct NoiseSettings {
    [Range(1, 4)] public int octaves;
    [Range(0, 1)] public float persistance;
    [Range(1, 5)] public float lacunarity;
    [Range(10, 500)] public int scale;
}