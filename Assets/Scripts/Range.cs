using System;

[Serializable]
public struct IntRange {
	public int min;
	public int max;
	public int Average => (max - min) / 2;
	public int Random => GameController.Random.Next(min, max + 1);
	public bool Contains(int i) => min <= i && i <= max;
}

[Serializable]
public struct FloatRange {
	public float min;
	public float max;
	public float Average => (max - min) / 2;
	public bool Contains(float f) => min <= f && f <= max;
}