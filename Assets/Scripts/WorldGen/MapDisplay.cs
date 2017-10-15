using UnityEngine;

public class MapDisplay : MonoBehaviour {

	[SerializeField] private MeshFilter meshFilter;
	[SerializeField] private MeshRenderer meshRenderer;
	[SerializeField] private MeshCollider meshCollider;
	[SerializeField] private AnimationCurve heightCurve;
	[SerializeField] private float heightMultiplier = 10;
	[SerializeField] [Range(1, 512)] private int meshWorldSize;

	public static Texture2D mapTexture;

	private static Map Map => GameController.Map;

	public void DrawMap() {
		Mesh mapMesh = MeshGenerator.GenerateTerrainMesh(Map.HeightMap, Map.settings.Lod, meshWorldSize, heightMultiplier, heightCurve);
		meshFilter.sharedMesh = mapMesh;
		meshCollider.sharedMesh = mapMesh;
		DrawTexture();
	}

	public void DrawTexture() {
		mapTexture = GameController.Map.GetTexture(WorldGenUI.DrawMode);
		meshRenderer.sharedMaterial.mainTexture = mapTexture;
	}
}