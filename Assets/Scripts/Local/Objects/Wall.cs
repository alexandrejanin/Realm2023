using UnityEngine;

public class Wall : Entity {
	public override string Name => "Wall";

	public override Vector3 WorldPosition { get; }

	public Wall(Vector3 position) : base(NodeGrid.GetCoordFromWorldPos(position)) {
		WorldPosition = position;
	}
}