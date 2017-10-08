public abstract class Entity {
	public Coord position;
	public abstract string Name { get; }

	public bool visible, seen, isInRange;

	protected Entity(Coord position) {
		this.position = position;
		ObjectManager.AddEntity(this);
	}

	public virtual UnityEngine.Vector3 WorldPosition => NodeGrid.GetWorldPosFromCoord(position, NodeGrid.NodeOffsetType.Center);

	public virtual void StartTurn() { }

	public virtual void EndTurn() {
		UpdateVisibility(ObjectManager.playerCharacter.position);
	}

	public void UpdateVisibility(Coord playerPosition) {
		int dx = position.x - playerPosition.x;
		int dz = position.z - playerPosition.z;
		isInRange = dx * dx + dz * dz <= 225f;
		visible = isInRange && NodeGrid.Visibility(position, playerPosition);
		seen = seen || visible;
	}

	public override string ToString() => Name;
}