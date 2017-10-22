using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Wall : Interactable {
	public override string Name => "Wall";

	public readonly Coord direction;

	public readonly WallType wallType;

	public override Vector3 WorldPosition => base.WorldPosition + (Vector3) direction / 2;

	public Wall(Coord position, Coord direction, WallType wallType) : base(position) {
		this.direction = direction;
		NodeGrid.BlockPassage(position, direction);
		this.wallType = wallType;
	}

	public override bool CanBeSeenFrom(Coord from) => NodeGrid.IsVisible(from, position, -direction) || NodeGrid.IsVisible(from, position + direction, direction);

	public override List<Interaction> GetInteractions(Character character) => null;

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