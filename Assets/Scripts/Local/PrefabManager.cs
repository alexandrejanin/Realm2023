using System;
using UnityEngine;

public class PrefabManager : MonoBehaviour {
	[Header("Objects")] public ItemObject itemObjectPrefab;
	public CharacterObject characterObjectPrefab;
	public GameObject doorObjectPrefab;

	[Header("Blocks")] [SerializeField] private WallObject grassPrefab;
	[SerializeField] private WallObject woodPrefab;
	[SerializeField] private WallObject stonePrefab;

	[Header("UI")] [SerializeField] public ButtonsFrame buttonsFramePrefab;
	[SerializeField] public UnityEngine.UI.Button buttonPrefab;

	public WallObject GetWallObject(WallType wallType) {
		switch (wallType) {
			case WallType.Grass: return grassPrefab;
			case WallType.Wood: return woodPrefab;
			case WallType.Stone: return stonePrefab;
			default:
				throw new ArgumentOutOfRangeException(nameof(wallType), wallType, null);
		}
	}
}