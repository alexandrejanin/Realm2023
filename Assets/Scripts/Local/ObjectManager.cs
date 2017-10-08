using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public static class ObjectManager {
	private static readonly PrefabManager PrefabManager = Object.FindObjectOfType<PrefabManager>();
	private static readonly List<Entity> Entities = new List<Entity>();

	public static Character playerCharacter;
	public static CharacterObject playerCharacterObject;

	public static readonly List<InteractableObject> InteractableObjects = new List<InteractableObject>();

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
			InstantiateObjects();
			UpdateVisibility();
		}
	}

	public static void SetPlayer(Character player) {
		playerCharacter = player;

		if (Entities.Contains(playerCharacter) && Entities[0] != playerCharacter) Entities.Remove(playerCharacter);
		Entities.Insert(0, playerCharacter);
	}

	public static void InstantiateObjects() {
		foreach (Entity entity in Entities) {
			Interactable interactable = entity as Interactable;
			if (interactable == null || InteractableObjects.Any(itemObject => itemObject.Interactable == interactable)) continue;

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
		}
	}

	public static void UpdateVisibility() {
		foreach (HideableObject hideableObject in Hideables) {
			if (hideableObject != null) hideableObject.UpdateDisplay();
		}
	}

	public static void TakeTurn() {
		InstantiateObjects();
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