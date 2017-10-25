using System.Collections.Generic;

public abstract class Item : Interactable {
	public int size;
	public Container container;

	protected Item(Coord position, int size = 0, Container container = null) : base(position) {
		this.size = size;
		this.container = container;
	}

	public override List<Interaction> GetInteractions(Character character) {
		List<Interaction> interactions = GetBasicInteractions(character);

		if (ValidPosition(character.position)) {
			interactions.Add(character.HasItem(this) ? new Interaction("Drop", () => Drop(character), false) : new Interaction("Pick Up", () => PickUp(character), false));
		}
		return interactions;
	}

	protected override string InspectText() => Name + "\nSize: " + size;

	public void PickUp(Character character) {
		if (character.inventory.AddItem(this)) {
			position = character.position;
		}
	}

	public virtual void Drop(Character character) => container.RemoveItem(this);
}