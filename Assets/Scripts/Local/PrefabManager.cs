using System;
using UnityEngine;

public class PrefabManager : MonoBehaviour {
	[Header("Objects")] public ItemObject itemObjectPrefab;
	public CharacterObject characterObjectPrefab;
	public DoorwayObject doorObjectPrefab;

	[Header("Blocks")] [SerializeField] private WallObject grassPrefab;
	[SerializeField] private WallObject sandPrefab;
	[SerializeField] private WallObject snowPrefab;
	[SerializeField] private WallObject stonePrefab;
	[SerializeField] private WallObject woodPrefab;

	[Header("UI")] [SerializeField] public ButtonsFrame buttonsFramePrefab;
	[SerializeField] public UnityEngine.UI.Button buttonPrefab;

	public WallObject GetWallObject(WallType wallType) {
		switch (wallType) {
			case WallType.Grass: return grassPrefab;
			case WallType.Sand: return sandPrefab;
			case WallType.Snow: return snowPrefab;
			case WallType.Wood: return woodPrefab;
			case WallType.Stone: return stonePrefab;
			default:
				throw new ArgumentOutOfRangeException(nameof(wallType), wallType, null);
		}
	}
}