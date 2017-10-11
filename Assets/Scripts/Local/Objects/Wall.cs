using UnityEngine;

public class Wall : Entity {
	public override string Name => "Wall";

	public readonly Coord direction;

	public readonly WallType wallType;
	public override Vector3 WorldPosition => base.WorldPosition + (Vector3) direction / 2;
	public override Coord[] VisiblePositions { get; }

	public Wall(Coord position, Coord direction, WallType wallType) : base(position) {
		this.direction = direction;
		VisiblePositions = new[] {position, position + direction};
		location.nodeGrid.BlockPassage(position, direction);
		this.wallType = wallType;
	}
}

public enum WallType {
	Grass,
	Wood,
	Stone
}