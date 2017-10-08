public class ItemObject : InteractableObject {
	public Item item;
	public override Interactable Interactable => item;

	protected void Update() {
		if (item.container != null) {
			Destroy();
		}
		UpdatePosition();
	}
}