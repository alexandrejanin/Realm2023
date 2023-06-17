public class ItemObject : InteractableObject {
    public Item item;
    protected override Interactable Interactable => item;

    protected void Update() {
        if (item.container != null) {
            Destroy();
        }

        UpdatePosition();
    }
}