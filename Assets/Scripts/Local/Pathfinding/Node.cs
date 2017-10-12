using System;
using UnityEngine;

public class Node : IHeapItem<Node> {
	public Coord position;

	public int X => position.x;

	public int Y => position.y;

	public int Z => position.z;

	public Vector3 WorldPositionCenter => NodeGrid.GetWorldPosFromCoord(position, NodeGrid.NodeOffsetType.Center);

	public int hCost, gCost;

	public int FCost => hCost + gCost;

	public int movementPenalty;

	public Node parent;

	public bool IsWalkable => !directionOpen[Coord.Down.ToDirectionIndex];

	private readonly bool[] directionOpen;

	public Node(Coord position, int movementPenalty, bool[] directionOpen) {
		this.position = position;
		this.movementPenalty = movementPenalty;
		this.directionOpen = directionOpen;
	}

	public bool DirectionIsOpen(Coord direction) {
		if (direction == Coord.Zero) return true;

		return direction.ToDirectionIndex == -1
			? DirectionIsOpen(new Coord(direction.x, 0, 0)) && DirectionIsOpen(new Coord(0, direction.y, 0)) && DirectionIsOpen(new Coord(0, 0, direction.z))
			: directionOpen[direction.ToDirectionIndex];
	}

	public void SetDirection(Coord direction, bool open) {
		if (!direction.IsDirection) throw new ArgumentException();
		directionOpen[direction.ToDirectionIndex] = open;
	}

	public override string ToString() => $"Node {position}";

	public int HeapIndex { get; set; }

	public int CompareTo(Node other) {
		int compare = FCost.CompareTo(other.FCost);
		if (compare == 0) {
			compare = hCost.CompareTo(other.hCost);
		}
		return -compare;
	}
}