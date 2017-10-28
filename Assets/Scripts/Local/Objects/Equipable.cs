using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class Equipable : Item {
	public readonly Slot slot;
	public readonly int slotSize;
	public readonly StatModifier[] modifiers;

	public override string Name {
		get {
			switch (slot) {
				case Slot.Feet:
					return "Boots";
				case Slot.Hand:
					return "Sword";
				case Slot.Head:
					return "Helmet";
				case Slot.Legs:
					return "Pants";
				case Slot.Neck:
					return "Necklace";
				case Slot.Torso:
					return "Shirt";
				default: return "Error: invalid slot";
			}
		}
	}

	public Equipable(Location location, Coord position, Container container = null) : this(location, position, Utility.RandomValue<Slot>(1), Random.Range(1, 21),
		new[] {new StatModifier("", "", Utility.RandomValue<Stat>(), Random.Range(1, 10))}, container) { }

	public Equipable(Location location, Coord position, Slot slot, int size, StatModifier[] modifiers = null, Container container = null) : base(location, position, size, container) {
		this.slot = slot;
		this.modifiers = modifiers;
		slotSize = slot == Slot.Feet || slot == Slot.Legs || slot == Slot.Hand && Utility.RandomBool ? 2 : 1;
	}

	protected override string InspectText() => base.InspectText() + "\nSlot: " + Enum.GetName(typeof(Slot), slot) + " (x" + slotSize + ")" +
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

	public override void Drop(Character character) {
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