using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Equipment : Container, IEnumerable<Equipable> {
	private readonly Character character;

	private Equipable waitingEquipable;
	private List<BodyPart> waitingBodyParts = new List<BodyPart>();
	private readonly List<BodyPart> chosenBodyParts = new List<BodyPart>();

	protected override List<Item> Items => Equipables.Cast<Item>().ToList();

	private IEnumerable<Equipable> Equipables => character.body.Equipables;

	public bool Contains(Equipable equipable) => Equipables.Contains(equipable);

	public Equipment(Character character) : base(character.Name + "'s equipment", character.location, character.position, 1000) {
		this.character = character;
	}

	public override bool AddItem(Item item) {
		if (!(item is Equipable)) {
			Debug.LogError("Trying to equip non-equipable item (" + item + ")");
			return false;
		}

		Equipable equipable = (Equipable) item;

		if (Contains(equipable)) {
			Debug.LogError("Trying to equip already equipped item (" + item + ")");
			return false;
		}

		equipable.container?.RemoveItem(equipable);
		List<BodyPart> validParts = (from bodyPart in character.body where bodyPart.slot == equipable.slot select bodyPart).ToList();

		if (validParts.Count < equipable.slotSize) return false;

		if (validParts.Count == equipable.slotSize) {
			EquipItem(equipable, validParts);
			return true;
		}

		List<BodyPart> freeParts = validParts.Where(bodyPart => bodyPart.equipable == null).ToList();

		if (freeParts.Count == equipable.slotSize) {
			EquipItem(equipable, validParts);
			return true;
		}

		chosenBodyParts.Clear();
		waitingEquipable = equipable;
		waitingBodyParts = validParts;
		DisplayParts();

		return false;
	}

	private void DisplayParts() {
		List<Interaction> interactions = waitingBodyParts.Select(validPart => new Interaction(validPart.name, () => validPart.OnChosen(this), false)).ToList();

		Player.DisplayInteractions("Equip where?", interactions);
	}

	public void ChooseBodyPart(BodyPart bodyPart) {
		chosenBodyParts.Add(bodyPart);
		waitingBodyParts.Remove(bodyPart);
		if (chosenBodyParts.Count == waitingEquipable.slotSize) {
			EquipItem(waitingEquipable, chosenBodyParts);
		} else {
			DisplayParts();
		}
	}

	private void EquipItem(Equipable equipable, IEnumerable<BodyPart> bodyParts) {
		foreach (BodyPart chosenPart in bodyParts) {
			if (chosenPart.equipable != null) RemoveItem(chosenPart);
			chosenPart.equipable = equipable;
			equipable.container = this;
		}
	}

	public override void RemoveItem(Item item) {
		if (!(item is Equipable)) {
			Debug.LogError("Trying to remove non-equipable item (" + item + ")");
			return;
		}

		Equipable equipable = (Equipable) item;

		if (!Contains(equipable)) {
			Debug.LogError("Trying to remove non-equipped item (" + item + ")");
			return;
		}

		foreach (BodyPart bodyPart in character.body) {
			if (bodyPart.equipable == equipable) RemoveItem(bodyPart);
		}
	}

	private void RemoveItem(BodyPart bodyPart) {
		Equipable equipable = bodyPart.equipable;
		bodyPart.equipable = null;

		if (equipable.container == this) equipable.container = null;
		if (equipable.container != character.inventory) character.inventory.AddItem(equipable);
	}

	public override void Update() {
		position = character.position;
		base.Update();
	}

	public new IEnumerator<Equipable> GetEnumerator() => Equipables.GetEnumerator();
}