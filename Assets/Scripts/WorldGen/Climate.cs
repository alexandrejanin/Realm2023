using System.Linq;
using UnityEngine;

// ReSharper disable UnassignedField.Global

#pragma warning disable 0649

[System.Serializable]
public class Climate {
	public string name;

	//[SerializeField] private float minTemp, maxTemp, minHeight, maxHeight, minHumidity, maxHumidity;
	[SerializeField] private FloatRange height, temp, humidity;

	public bool isWater;

	public Gradient colorGradient;

	public Color GetColor(float tileHeight) {
		Color color = colorGradient.Evaluate(Mathf.InverseLerp(height.min, height.max, tileHeight));
		return color;
	}

	public bool CorrectTile(Tile tile) => height.Contains(tile.height) && temp.Contains(tile.temp) && humidity.Contains(tile.humidity);

	public static Climate GetClimate(Tile tile) => GameController.Climates.First(climate => climate.CorrectTile(tile));

	public override string ToString() => name;
}