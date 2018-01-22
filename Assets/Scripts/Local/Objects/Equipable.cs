using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Equipable : Item {
	public readonly Slot slot;
	public readonly int slotSize;
	public readonly StatModifier[] modifiers;

	public sealed override string Name {
		get {
			switch (slot) {
				case Slot.Feet:
					return "Boots";
				case Slot.Hand:
					return slotSize > 1 ? "Longsword" : "Sword";
				case Slot.Head:
					return "Helmet";
				case Slot.Legs:
					return "Pants";
				case Slot.Neck:
					return "Necklace";
				case Slot.Torso:
					return "Shirt";
				case Slot.None:
					return "You shouldn't be seeing this.";
				default: return "Error: invalid slot";
			}
		}
	}

	public Equipable(Location location, Coord position, Container container = null) : base(location, position, Random.Range(1, 21), container) {
		slot = Utility.RandomValue<Slot>(1);
		slotSize = slot == Slot.Feet || slot == Slot.Legs || slot == Slot.Hand && Utility.RandomBool ? 2 : 1;
		modifiers = new[] {new StatModifier(Name, "Stat boost from " + Name, Utility.RandomValue<Stat>(), Random.Range(1, 10))};
	}

	protected override string InspectText() => base.InspectText() + "\nSlot: " + slot + (slotSize > 1 ? " (x" + slotSize + ")" : "") +
	                                           modifiers.Aggregate("", (current, equipableModifier) => "\n" + current + equipableModifier);

	public override List<Interaction> GetInteractions(Character character) {
		List<Interaction> interactions = base.GetInteractions(character);

		if (ValidPosition(character.position)) {
			interactions.Add(character.equipment.Contains(this) ? new Interaction("Unequip", () => Unequip(character), false) : new Interaction("Equip", () => Equip(character), false));
		}

		return interactions;
	}

	private void Equip(Character character) {
		if (character.equipment.Contains(this)) return;
		character.equipment.AddItem(this);
	}

	private void Unequip(Character character) {
		if (!character.equipment.Contains(this)) return;
		character.equipment.RemoveItem(this);
	}

	protected override void Drop(Character character) {
		Unequip(character);
		base.Drop(character);
	}
}

public enum Slot {
	None,
	Head,
	Torso,
	Legs,
	Feet,
	Hand,
	Neck
}