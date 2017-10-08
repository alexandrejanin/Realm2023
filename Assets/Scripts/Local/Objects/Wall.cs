using UnityEngine;

public class Wall : Entity {
	public override string Name => "Wall";

	protected override Coord[] VisiblePositions { get; }

	public readonly Coord direction;

	public override Vector3 WorldPosition => base.WorldPosition + (Vector3) direction / 2;

	public Wall(Coord position, Coord direction) : base(position) {
		this.direction = direction;
		VisiblePositions = new[] {position, position + direction};
		NodeGrid.BlockPassage(position, direction);
	}
}