using System.Collections.Generic;
using System.Linq;
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

	public bool IsWalkable => !directionOpen[Coord.down];

	public readonly Dictionary<Coord, bool> directionOpen;

	public Node(Coord position, int movementPenalty, Dictionary<Coord, bool> directionOpen) {
		this.position = position;
		this.movementPenalty = movementPenalty;
		this.directionOpen = directionOpen;
	}

	public bool DirectionIsOpen(Coord direction) {
		try {
			if (directionOpen.ContainsKey(direction)) return directionOpen[direction];

			return directionOpen[new Coord(direction.x, 0, 0)] && directionOpen[new Coord(0, direction.y, 0)] && directionOpen[new Coord(0, 0, direction.z)];
		}
		catch {
			Debug.Log("Error with " + this + "," + direction);
			return true;
		}
	}

	public void SetDirection(Coord direction, bool open) {
		if (directionOpen.ContainsKey(direction)) {
			directionOpen[direction] = open;
		} else {
			Debug.Log("Contains:" + directionOpen.Keys.Aggregate("", (current, key) => current + (key + ", ")) + "adding " + direction);
			directionOpen.Add(direction, open);
		}
	}

	public override string ToString() => "Node (" + X + ", " + Y + ", " + Z + ")";

	public int HeapIndex { get; set; }

	public int CompareTo(Node other) {
		int compare = FCost.CompareTo(other.FCost);
		if (compare == 0) {
			compare = hCost.CompareTo(other.hCost);
		}
		return -compare;
	}
}