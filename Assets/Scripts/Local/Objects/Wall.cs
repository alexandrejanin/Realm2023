using UnityEngine;

public class Wall : Entity {
    public override string Name => "Wall";

    public readonly Coord direction;

    public readonly WallType wallType;

    public override Vector3 WorldPosition => base.WorldPosition + (Vector3) direction / 2;

    public Wall(Location location, Coord position, Coord direction, WallType wallType) : base(location, position) {
        this.direction = direction;
        NodeGrid.BlockPassage(position, direction);
        this.wallType = wallType;
    }

    public override bool CanBeSeenFrom(Coord from) => NodeGrid.IsVisible(from, position, -direction) || NodeGrid.IsVisible(from, position + direction, direction);
    public override bool CanSeeTo(Coord to) => NodeGrid.IsVisible(position, to, -direction) || NodeGrid.IsVisible(position + direction, to, direction);

    public WallCoordinate WallCoordinate => new WallCoordinate(position, direction.ToDirectionIndex);
}

public enum WallType {
    Grass,
    Sand,
    Snow,
    Wood,
    Stone
}

public struct WallCoordinate {
    public Coord position;
    public int direction;

    public WallCoordinate(Coord position, int direction) {
        this.position = position;
        this.direction = direction;
    }
}