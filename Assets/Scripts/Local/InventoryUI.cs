﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour {
    [SerializeField] private Text freeSpaceText;
    [SerializeField] private Text itemCountText;
    [SerializeField] private GridLayoutGroup inventoryGrid;
    [SerializeField] private GridLayoutGroup equipmentGrid;
    [SerializeField] private ItemSlot itemSlotPrefab;
    [SerializeField] private BodyPartSlot bodyPartSlotPrefab;
    private Character character;
    private readonly List<Item> displayedItems = new List<Item>();
    private readonly List<ItemSlot> itemSlots = new List<ItemSlot>();

    private readonly List<BodyPart> displayedBodyParts = new List<BodyPart>();

    private void Awake() {
        character = ObjectManager.playerCharacter;
    }

    private void Update() {
        freeSpaceText.text = character.inventory.FreeSpace + "/" + character.inventory.maxSize;
        itemCountText.text = character.inventory.ItemCount.ToString();

        foreach (var item in character.inventory) {
            if (!displayedItems.Contains(item)) {
                var itemSlot = Instantiate(itemSlotPrefab, Vector3.zero, Quaternion.identity, inventoryGrid.transform);
                itemSlot.Item = item;
                displayedItems.Add(item);
                itemSlots.Add(itemSlot);
            }
        }

        itemSlots.RemoveAll(item => item == null);
        foreach (var itemSlot in itemSlots) {
            if (itemSlot.Item.container != character.inventory || itemSlot.Item == null) {
                displayedItems.Remove(itemSlot.Item);
                Destroy(itemSlot.gameObject);
            }
        }

        foreach (var bodyPart in character.body) {
            if (bodyPart.slot != Slot.None && !displayedBodyParts.Contains(bodyPart)) {
                var bodyPartSlot = Instantiate(bodyPartSlotPrefab, Vector3.zero, Quaternion.identity,
                    equipmentGrid.transform);
                bodyPartSlot.bodyPart = bodyPart;
                displayedBodyParts.Add(bodyPart);
            }
        }
    }
}