using System.Linq;
using UnityEngine;
// ReSharper disable UnassignedField.Global

[System.Serializable]
public class Climate {
	public string name;

	[SerializeField] private float minTemp, maxTemp, minHeight, maxHeight, minHumidity, maxHumidity;

	public bool isRegion, isWater;

	public Gradient colorGradient;

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

	public static Climate GetClimate(string climateName) => GameController.Climates.First(climate => climate.name == climateName);
	public static Climate GetClimate(Tile tile) => GameController.Climates.First(climate => climate.CorrectTile(tile));

	public override string ToString() => name;
}