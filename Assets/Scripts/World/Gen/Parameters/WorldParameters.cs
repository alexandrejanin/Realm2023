using UnityEngine;

[System.Serializable]
public struct WorldParameters {
    [Range(32, 512)] public int width;
    [Range(32, 512)] public int height;

    public int MaxDimension => Mathf.Max(width, height);
    public int MinDimension => Mathf.Min(width, height);

    public ElevationParameters elevationParameters;
    public TemperatureParameters temperatureParameters;
    public NoiseParameters humidityParameters;


    public int civilizations;


    public float[,] GenerateElevation() => elevationParameters.Generate(width, height);

    public float[,] GenerateTemperature(float[,] elevation) => temperatureParameters.Generate(width, height, elevation);

    public float[,] GenerateHumidity() => humidityParameters.Generate(width, height);
}