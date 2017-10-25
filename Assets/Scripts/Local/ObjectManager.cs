using System.Collections.Generic;
using UnityEngine;

public class ObjectManager {
	private static PrefabManager PrefabManager => GameController.PrefabManager;
	public static Character playerCharacter;
	public static CharacterObject playerCharacterObject;

	public static readonly int HiddenTerrainLayer = 8;
	public static readonly int VisibleTerrainLayer = 9;
	public static readonly int HiddenTerrainMask = 1 << HiddenTerrainLayer;
	public static readonly int VisibleTerrainMask = 1 << VisibleTerrainLayer;
	public static readonly int TerrainMask = HiddenTerrainMask | VisibleTerrainMask;

	public static readonly List<EntityObject> Hideables = new List<EntityObject>();

	private static Location Location => GameController.Location;

	private const int renderRange = 900;

	public static void RefreshObjects() {
		foreach (Character character in Location.characters) {
			if (!character.displayed) {
				CharacterObject characterObject = Object.Instantiate(PrefabManager.characterObjectPrefab, character.WorldPosition, Quaternion.identity);
				characterObject.SetCharacter(character);
				if (character.isPlayer) playerCharacterObject = characterObject;
				character.displayed = true;
			}
		}

		foreach (Item item in Location.items) {
			if (!item.displayed && item.container == null) {
				ItemObject itemObject = Object.Instantiate(PrefabManager.itemObjectPrefab, item.WorldPosition, Quaternion.identity);
				itemObject.item = item;
				item.displayed = true;
			}
		}

		foreach (Wall wall in Location.walls.Values) {
			if (wall == null || wall.displayed) continue;

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

			wall.displayed = true;
		}
		foreach (EntityObject entityObject in Hideables) {
			if (entityObject != null) entityObject.UpdateDisplay();
		}
	}

	public static void TakeTurn() {
		foreach (Character character in Location.characters) {
			character.TakeTurn();
		}

		Debug.Log("turn ended");

		foreach (Entity locationEntity in Location.Entities) {
			UpdateVisibility(locationEntity);
		}

		RefreshObjects();
	}

	public static void UpdateVisibility(Entity entity) {
		entity.inRenderRange = (playerCharacter.position - entity.position).SquaredMagnitude < renderRange;
		entity.visible = entity.CanBeSeenFrom(playerCharacter.position);
		entity.seen = entity.seen || entity.visible;
	}
}