using UnityEngine;

[System.Serializable]
public struct ElevationParameters {
    public NoiseParameters2 noiseParameters;

    [Range(0, 10)] public float falloffA;
    [Range(1, 10)] public float falloffB;
    [Range(0, 1)] public float falloffMultiplier;

    public float[,] Generate(int width, int height) {
        var elevation = noiseParameters.Generate(width, height);

        var falloff = new float[width, height];
        for (var i = 0; i < width; i++) {
            for (var j = 0; j < height; j++) {
                var x = i / (float)width * 2 - 1;
                var y = j / (float)height * 2 - 1;
                var value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                falloff[i, j] = Evaluate(value, falloffA, falloffB);
            }
        }

        for (var x = 0; x < width; x++) {
            for (var y = 0; y < height; y++) {
                elevation[x, y] = Mathf.Clamp01(elevation[x, y] - falloff[x, y] * falloffMultiplier);
            }
        }

        return elevation;
    }

    private static float Evaluate(float value, float a, float b) =>
        Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
}