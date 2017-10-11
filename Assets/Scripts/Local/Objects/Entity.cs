using System.Linq;

public abstract class Entity {
	public Location location;
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

	public virtual Coord[] VisiblePositions {
		get {
			if (position != lastPos) {
				positions = new[] {position};
				lastPos = position;
			}
			return positions;
		}
	}

	private Coord lastPos;
	private Coord[] positions;

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
		visible = isInViewRange && VisiblePositions.Any(pos => location.nodeGrid.IsVisible(playerPosition, pos));
		seen = seen || visible;
	}

	public static implicit operator Coord(Entity entity) => entity.position;

	public override string ToString() => Name;
}