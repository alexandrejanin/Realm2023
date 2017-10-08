public class WallObject : HideableObject {
	private Wall wall;

	protected override Entity Entity => wall;

	protected override void Awake() {
		wall = new Wall(transform.position);
		base.Awake();
	}
}