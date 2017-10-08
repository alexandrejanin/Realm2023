public abstract class Entity {
	public Coord position;
	public abstract string Name { get; }

	public bool visible, seen, isInViewRange, isInSeenRange;

	public bool displayed;

	private const int maxViewDistanceSquared = 225;
	private const int maxSeenDistanceSquared = 1600;

	protected Entity(Coord position) {
		this.position = position;
		ObjectManager.AddEntity(this);
	}

	public virtual UnityEngine.Vector3 WorldPosition => NodeGrid.GetWorldPosFromCoord(position, NodeGrid.NodeOffsetType.Center);

	protected virtual Coord[] VisiblePositions => new[] {position};

	public virtual void StartTurn() { }

	public virtual void EndTurn() {
		UpdateVisibility(ObjectManager.playerCharacter.position);
	}

	public void UpdateVisibility(Coord playerPosition) {
		int dx = position.x - playerPosition.x;
		int dz = position.z - playerPosition.z;
		int d = dx * dx + dz * dz;
		isInViewRange = d <= maxViewDistanceSquared && !(dx * dx == maxViewDistanceSquared || dz * dz == maxViewDistanceSquared);
		isInSeenRange = d <= maxSeenDistanceSquared;
		visible = isInViewRange; // && VisiblePositions.Any(pos => NodeGrid.IsVisible(pos, playerPosition));
		seen = seen || visible;
	}

	public override string ToString() => Name;
}