using UnityEngine;

[System.Serializable]
public struct TemperatureParameters {
    [Range(0, 10)] public float tempA;
    [Range(0.5f, 1.5f)] public float tempB;
    [Range(0, 1)] public float heightTempMultiplier;

    public float[,] Generate(int width, int height, float[,] elevation) {
        var minTemp = float.MaxValue;
        var maxTemp = float.MinValue;
        var tempMap = new float[width, height];

        var maxTempLatitude = height;

        for (var i = 0; i < width; i++) {
            for (var j = 0; j < height; j++) {
                var heightTemp = Mathf.Abs(elevation[i, j] - 0.5f) * heightTempMultiplier;
                var latitudeTemp = 1 - Mathf.Abs(maxTempLatitude - j) / (float)maxTempLatitude;
                var temp = Mathf.Clamp01(latitudeTemp - heightTemp);
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

        for (var x = 0; x < width; x++) {
            for (var y = 0; y < height; y++) {
                tempMap[x, y] = Mathf.InverseLerp(minTemp, maxTemp, tempMap[x, y]);
            }
        }

        return tempMap;
    }

    private static float Evaluate(float value, float a, float b) =>
        Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
}