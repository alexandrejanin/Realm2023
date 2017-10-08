public class ItemObject : InteractableObject {
	public Item item;
	public override Interactable Interactable => item;

	private void Update() {
		if (item.container != null) {
			Destroy();
		}
		UpdateStatus();
	}
}