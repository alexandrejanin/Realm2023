using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ObjectManager {
	private static readonly PrefabManager PrefabManager = Object.FindObjectOfType<PrefabManager>();

	public static Character playerCharacter;
	public static CharacterObject playerCharacterObject;

	public static readonly int HiddenTerrainLayer = 8;
	public static readonly int VisibleTerrainLayer = 9;
	public static readonly int HiddenTerrainMask = 1 << HiddenTerrainLayer;
	public static readonly int VisibleTerrainMask = 1 << VisibleTerrainLayer;
	public static readonly int TerrainMask = HiddenTerrainMask | VisibleTerrainMask;

	public static readonly List<EntityObject> Hideables = new List<EntityObject>();

	private static Location Location => GameController.Location;

	private static void RefreshObjects() {
		/*foreach (Entity entity in GameController.Location.entities) {
			if (entity.displayed) continue;
			
			Item item = entity as Item;
			

			entity.displayed = true;
		}*/
		foreach (Character character in Location.characters) {
			if (!character.displayed) {
				CharacterObject characterObject = Object.Instantiate(PrefabManager.characterObjectPrefab, character.WorldPosition, Quaternion.identity);
				characterObject.SetCharacter(character);
				if (character.isPlayer) playerCharacterObject = characterObject;
			}
		}

		foreach (Item item in Location.items) {
			if (!item.displayed && item.container == null) {
				ItemObject itemObject = Object.Instantiate(PrefabManager.itemObjectPrefab, item.WorldPosition, Quaternion.identity);
				itemObject.item = item;
			}
		}

		foreach (Wall wall in Location.walls) {
			if (wall.displayed) continue;

			Door door = wall as Door;
			if (door != null) {
				GameObject doorObject = Object.Instantiate(PrefabManager.doorObjectPrefab, door.WorldPosition, Quaternion.identity);
				doorObject.GetComponentInChildren<DoorObject>().door = door;
				doorObject.transform.forward = door.direction;
			} else {
				WallObject wallObject = Object.Instantiate(PrefabManager.GetWallObject(wall.wallType), wall.WorldPosition, Quaternion.identity);
				wallObject.wall = wall;
				wallObject.transform.up = wall.direction;
			}
		}
	}

	public static void UpdateVisibility() {
		foreach (EntityObject hideableObject in Hideables) {
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
		foreach (Entity entity in Location.Entities) {
			entity.StartTurn();
		}
	}

	private static void EndTurn() {
		foreach (Entity entity in Location.Entities) {
			entity.EndTurn();
		}
	}
}