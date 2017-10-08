using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

[Serializable]
public class Climate : Importable {
	public readonly string name;

	public readonly float minTemp, maxTemp, minHeight, maxHeight, minHumidity, maxHumidity;

	public readonly bool isRegion, isLandmark, isWater;

	public readonly int minSize;
	public readonly string subClimate;
	public readonly Gradient colorGradient;

	public Climate(IDictionary<string, string> d) : base(d) {
		name = d["Name"];

		minTemp = float.Parse(d["MinTemp"], CultureInfo.InvariantCulture);
		maxTemp = float.Parse(d["MaxTemp"], CultureInfo.InvariantCulture);
		minHeight = float.Parse(d["MinHeight"], CultureInfo.InvariantCulture);
		maxHeight = float.Parse(d["MaxHeight"], CultureInfo.InvariantCulture);
		minHumidity = float.Parse(d["MinHumidity"], CultureInfo.InvariantCulture);
		maxHumidity = float.Parse(d["MaxHumidity"], CultureInfo.InvariantCulture);

		if (minHeight >= maxHeight || minTemp >= maxTemp || minHumidity >= maxHumidity) {
			Debug.LogError("Error: Invalid value for climate " + name);
		}

		isRegion = bool.Parse(d["IsRegion"]);
		isLandmark = bool.Parse(d["IsLandmark"]);
		isWater = bool.Parse(d["IsWater"]);

		minSize = int.Parse(d["MinSize"]);
		subClimate = d["SubClimate"];

		int colorsAmount = int.Parse(d["ColorsAmount"]);
		GradientColorKey[] colorKeys = new GradientColorKey[colorsAmount];
		GradientAlphaKey[] alphaKeys = {new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 1)};

		for (int i = 0; i < colorsAmount; i++) {
			string hexColor = d["Color" + i];
			Color color;
			ColorUtility.TryParseHtmlString(hexColor, out color);
			colorKeys[i] = new GradientColorKey(color, (float) i / (colorsAmount - 1));
		}

		colorGradient = new Gradient();
		colorGradient.SetKeys(colorKeys, alphaKeys);
	}

	public Color GetColor(float height) {
		Color color = colorGradient.Evaluate(Mathf.InverseLerp(minHeight, maxHeight, height));
		return color;
	}

	public bool CorrectTile(Tile tile) {
		float height = tile.height;
		float temp = tile.temp;
		float humidity = tile.humidity;
		return isRegion && height >= minHeight && height <= maxHeight && temp >= minTemp && temp <= maxTemp && humidity >= minHumidity && humidity <= maxHumidity;
	}

	public override string ToString() => name;
}