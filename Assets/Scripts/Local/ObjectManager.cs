using System.Collections.Generic;
using UnityEngine;

public static class ObjectManager {
    private static PrefabManager PrefabManager => GameManager.PrefabManager;
    public static Character playerCharacter;
    public static CharacterObject playerCharacterObject;

    public static readonly int HiddenTerrainLayer = 8;
    public static readonly int VisibleTerrainLayer = 9;
    public static readonly int HiddenTerrainMask = 1 << HiddenTerrainLayer;
    public static readonly int VisibleTerrainMask = 1 << VisibleTerrainLayer;
    public static readonly int TerrainMask = HiddenTerrainMask | VisibleTerrainMask;

    public static readonly List<EntityObject> EntityObjects = new List<EntityObject>();
    public static readonly HashSet<Entity> DisplayedEntities = new HashSet<Entity>();

    private static Location Location => GameManager.LocalManager.Location;

    private const int renderRange = 900;

    private const string terrainParentName = "Terrain";
    private static Transform TerrainParent => terrainParent ?? (terrainParent = GameObject.Find(terrainParentName).transform);
    private static Transform terrainParent;

    public static void RefreshObjects() {
        foreach (var character in Location.characters) {
            if (DisplayedEntities.Contains(character)) continue;

            var characterObject = Object.Instantiate(PrefabManager.characterObjectPrefab, character.WorldPosition, Quaternion.identity);
            characterObject.SetCharacter(character);
            if (character.isPlayer) playerCharacterObject = characterObject;
            DisplayedEntities.Add(character);
        }

        foreach (var item in Location.items) {
            if (DisplayedEntities.Contains(item)) continue;

            if (item.container == null) {
                var itemObject = Object.Instantiate(PrefabManager.itemObjectPrefab, item.WorldPosition, Quaternion.identity);
                itemObject.item = item;
                DisplayedEntities.Add(item);
            }
        }

        foreach (var wall in Location.walls.Values) {
            if (DisplayedEntities.Contains(wall)) continue;

            var doorway = wall as Doorway;
            if (doorway != null) {
                var doorwayObject = Object.Instantiate(PrefabManager.doorObjectPrefab, doorway.WorldPosition, Quaternion.identity, TerrainParent);
                doorwayObject.doorway = doorway;
                doorwayObject.GetComponentInChildren<DoorObject>().door = doorway.door;
                doorwayObject.transform.up = doorway.direction;
            }
            else {
                var wallObject = Object.Instantiate(PrefabManager.GetWallObject(wall.wallType), wall.WorldPosition, Quaternion.identity, TerrainParent);
                wallObject.Wall = wall;
                wallObject.transform.up = wall.direction;
            }

            DisplayedEntities.Add(wall);
        }

        foreach (var entityObject in EntityObjects) {
            playerCharacter.visible = true;
            if (entityObject) entityObject.UpdateDisplay();
        }
    }

    public static void TakeTurn() {
        foreach (var character in Location.characters) {
            character.TakeTurn();
        }

        foreach (var entity in Location.Entities) {
            UpdateVisibility(entity);
        }

        RefreshObjects();
    }

    public static void UpdateVisibility(Entity entity) {
        entity.inRenderRange = (playerCharacter.position - entity.position).SquaredMagnitude < renderRange;
        entity.visible = entity == playerCharacter || entity.CanBeSeenFrom(playerCharacter.position);
        entity.seen = entity.seen || entity.visible;
    }
}