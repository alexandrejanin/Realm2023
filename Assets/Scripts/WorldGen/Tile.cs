﻿using System.Collections.Generic;
using UnityEngine;

public class Tile {
	public readonly int x, y;
	public readonly float height, temp, humidity;

	public Region region;
	public Location location;

	public Climate Climate { get; private set; }

	public Color color;
	private readonly Color heightColor;
	private readonly Color tempColor;
	private readonly Color humidityColor;

	public bool IsWater => Climate.isWater;

	#region Pathfinding

	public float f, g;
	public bool regionPending;
	public Tile parent;

	#endregion

	public Tile(int x, int y, float height, float temp, float humidity) {
		this.x = x;
		this.y = y;
		this.height = height;
		this.temp = temp;
		this.humidity = humidity;
		Climate = Map.GetClimate(this);
		color = Climate.GetColor(height);
		heightColor = Color.Lerp(Color.black, Color.white, height);
		tempColor = Color.Lerp(Color.cyan, Color.red, temp);
		humidityColor = Color.Lerp(Color.yellow, Color.blue, humidity);
	}

	public void SetRegion(Region newRegion) {
		region = newRegion;
		Climate = region.climate;
		color = Climate.GetColor(height);
	}

	public Color GetColor(MapDrawMode mapDrawMode) {
		switch (mapDrawMode) {
			case MapDrawMode.Normal:
				return color;
			case MapDrawMode.Height:
				return heightColor;
			case MapDrawMode.Temperature:
				return tempColor;
			case MapDrawMode.Humidity:
				return humidityColor;
		}
		return color;
	}

	public override string ToString() => Climate + " tile (" + x + ", " + y + ")";
}