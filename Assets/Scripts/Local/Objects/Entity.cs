public abstract class Entity {
	public Location location;
	public Coord position;
	public abstract string Name { get; }

	public bool visible, seen, inRenderRange;

	private const int renderRange = 400;

	public bool displayed;

	protected Entity(Coord position) {
		this.position = position;
	}

	public virtual UnityEngine.Vector3 WorldPosition => NodeGrid.GetWorldPosFromCoord(position, NodeGrid.NodeOffsetType.Center);

	private Coord lastPos;
	private Coord[] positions;

	public virtual void StartTurn() { }

	public virtual void EndTurn() {
		UpdateVisibility(ObjectManager.playerCharacter.position);
	}

	protected virtual bool CanBeSeenFrom(Coord from) => NodeGrid.IsVisible(from, position);
	protected virtual bool CanSeeTo(Coord to) => NodeGrid.IsVisible(position, to);

	public void UpdateVisibility(Coord playerPosition) {
		inRenderRange = (playerPosition - position).SquaredMagnitude < renderRange;
		visible = CanBeSeenFrom(playerPosition);
		seen = seen || visible;
	}

	public static implicit operator Coord(Entity entity) => entity.position;

	public override string ToString() => Name;
}