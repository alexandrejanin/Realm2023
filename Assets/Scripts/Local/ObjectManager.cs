using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ObjectManager {
	private static readonly PrefabManager PrefabManager = Object.FindObjectOfType<PrefabManager>();
	private static readonly List<Entity> Entities = new List<Entity>();

	public static Character playerCharacter;
	public static CharacterObject playerCharacterObject;

	public static readonly int HiddenTerrainLayer = LayerMask.NameToLayer("HiddenTerrain");
	public static readonly int VisibleTerrainLayer = LayerMask.NameToLayer("VisibleTerrain");
	public static readonly LayerMask HiddenTerrainMask = 1 << HiddenTerrainLayer;
	public static readonly LayerMask VisibleTerrainMask = 1 << VisibleTerrainLayer;
	public static readonly LayerMask TerrainMask = HiddenTerrainMask | VisibleTerrainMask;

	public static readonly List<HideableObject> Hideables = new List<HideableObject>();

	public static void AddEntity(Entity entity) {
		if (Entities.Contains(entity)) return;
		Entities.Add(entity);
	}

	public static void Start() {
		if (SceneManager.GetActiveScene().name == "Local") {
			RefreshObjects();
			UpdateVisibility();
		}
	}

	public static void SetPlayer(Character player) {
		playerCharacter = player;

		if (Entities.Contains(playerCharacter) && Entities[0] != playerCharacter) Entities.Remove(playerCharacter);
		Entities.Insert(0, playerCharacter);
	}

	private static void RefreshObjects() {
		foreach (Entity entity in Entities) {
			if (entity.displayed) continue;

			Character character = entity as Character;
			if (character != null) {
				CharacterObject characterObject = Object.Instantiate(PrefabManager.characterObjectPrefab, character.WorldPosition, Quaternion.identity);
				characterObject.SetCharacter(character);
				if (character.isPlayer) playerCharacterObject = characterObject;
			}

			Item item = entity as Item;
			if (item != null && item.container == null) {
				ItemObject itemObject = Object.Instantiate(PrefabManager.itemObjectPrefab, item.WorldPosition, Quaternion.identity);
				itemObject.item = item;
			}

			Door door = entity as Door;
			if (door != null) {
				GameObject doorObject = Object.Instantiate(PrefabManager.doorObjectPrefab, door.WorldPosition, Quaternion.identity);
				doorObject.GetComponentInChildren<DoorObject>().door = door;
				doorObject.transform.forward = door.direction;
			}

			Wall wall = entity as Wall;
			if (wall != null) {
				WallObject wallObject = Object.Instantiate(PrefabManager.wallObjectPrefab, wall.WorldPosition, Quaternion.identity);
				wallObject.wall = wall;
				wallObject.transform.up = wall.direction;
			}

			entity.displayed = true;
		}
	}

	public static void UpdateVisibility() {
		foreach (HideableObject hideableObject in Hideables) {
			if (hideableObject != null) hideableObject.UpdateDisplay();
		}
	}

	public static void TakeTurn() {
		RefreshObjects();
		StartTurn();
		EndTurn();
		UpdateVisibility();
	}

	private static void StartTurn() {
		foreach (Entity entity in Entities) {
			entity.StartTurn();
		}
	}

	private static void EndTurn() {
		foreach (Entity entity in Entities) {
			entity.EndTurn();
		}
	}
}