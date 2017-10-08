using System;
using UnityEngine;

public class MapDisplay : MonoBehaviour {

	[SerializeField] private MeshFilter meshFilter;
	[SerializeField] private MeshRenderer meshRenderer;
	[SerializeField] private AnimationCurve heightCurve;
	[SerializeField] private float heightMultiplier = 10;
	[SerializeField] [Range(1, 512)] private int meshWorldSize;

	public static Texture2D mapTexture;

	private static Map Map => GameController.Map;

	public void DrawMap() {
		meshFilter.sharedMesh = MeshGenerator.GenerateTerrainMesh(Map.HeightMap, Map.settings.Lod, meshWorldSize, heightMultiplier, heightCurve).CreateMesh();
		DrawTexture();
	}

	public void DrawTexture() {
		mapTexture = GameController.Map.GetTexture(WorldGenUI.DrawMode);
		meshRenderer.sharedMaterial.mainTexture = mapTexture;
	}

	/*private void DrawPixel(int x, int y, Color color) {
		mapTexture.SetPixel(x, y, color);
	}

	public void RevertTile(Tile tile) {
		DrawTile(tile, MapData.instance.GetColor(tile));
	}

	public void DrawTile(Tile tile, Color color) {
		DrawPixel(tile.x, tile.y, color);
		ApplyTexture();
	}

	public void DrawPlace(Place place, Color color, float intensity = 1) {
		Color[] colors = mapTexture.GetPixels(place.MinX, place.MinY, place.Width, place.Height);

		foreach (Tile tile in place.tiles) {
			try {
				colors[(tile.y - place.MinY) * place.Width + (tile.x - place.MinX)] = Color.Lerp(tile.color, color, intensity);
			}
			catch {
				throw new ArgumentOutOfRangeException(place + ", " + tile);
			}
		}

		mapTexture.SetPixels(place.MinX, place.MinY, place.Width, place.Height, colors);
		ApplyTexture();
	}

	public void RevertPlace(Place place) {
		Color[] colors = mapTexture.GetPixels(place.MinX, place.MinY, place.Width, place.Height);

		foreach (Tile tile in place.tiles) {
			colors[(tile.y - place.MinY) * place.Width + (tile.x - place.MinX)] = tile.color;
		}

		mapTexture.SetPixels(place.MinX, place.MinY, place.Width, place.Height, colors);
		ApplyTexture();
	}

	public void DrawAllTiles() {
		mapTexture.SetPixels(MapData.instance.colorMap);
		ApplyTexture();
	}*/

	private void ApplyTexture() {
		mapTexture.Apply(false);
	}
}