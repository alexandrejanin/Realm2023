public class Inventory : Container {
	public readonly Character character;

	public Inventory(Character character) : base(character.Name + "'s inventory", character.location, character.position, 100) {
		this.character = character;
	}

	public override void Update() {
		position = character.position;
		base.Update();
	}
}