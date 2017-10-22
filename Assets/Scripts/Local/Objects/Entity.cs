public abstract class Entity {
	public Location location;
	public Coord position;
	public abstract string Name { get; }

	public bool visible, seen, inRenderRange;

	public bool displayed;

	protected Entity(Coord position) {
		this.position = position;
	}

	public virtual UnityEngine.Vector3 WorldPosition => NodeGrid.GetWorldPosFromCoord(position, NodeGrid.NodeOffsetType.Center);

	private Coord lastPos;
	private Coord[] positions;

	public virtual bool CanBeSeenFrom(Coord from) => NodeGrid.IsVisible(from, position);
	protected virtual bool CanSeeTo(Coord to) => NodeGrid.IsVisible(position, to);

	public static implicit operator Coord(Entity entity) => entity.position;

	public override string ToString() => Name;
}