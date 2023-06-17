using System.Collections.Generic;

public abstract class Item : Interactable {
    public readonly int size;
    public Container container;

    protected Item(Location location, Coord position, int size = 0, Container container = null) : base(location, position) {
        this.size = size;
        this.container = container;
    }

    public override List<Interaction> GetInteractions(Character character) {
        var interactions = GetBasicInteractions(character);

        if (ValidPosition(character.position)) {
            interactions.Add(character.HasItem(this) ? new Interaction("Drop", () => Drop(character), false) : new Interaction("Pick Up", () => PickUp(character), false));
        }

        return interactions;
    }

    protected override string InspectText() => Name + "\nSize: " + size;

    private void PickUp(Character character) {
        if (character.inventory.AddItem(this)) {
            position = character.position;
        }
    }

    protected virtual void Drop(Character character) => container.RemoveItem(this);
}