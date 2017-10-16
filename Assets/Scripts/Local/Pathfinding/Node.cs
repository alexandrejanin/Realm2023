using System;

public class Node : IHeapItem<Node> {
	public Coord position;

	public int hCost, gCost;

	public int FCost => hCost + gCost;

	public Node parent;

	public bool IsWalkable => !directionOpen[Coord.Down.ToDirectionIndex];

	private readonly bool[] directionOpen;

	public Node(Coord position, int movementPenalty, bool[] directionOpen) {
		this.position = position;
		this.directionOpen = directionOpen;
	}

	public bool DirectionIsOpen(Coord direction, bool cutCorners = false) {
		if (direction.x == 0 && direction.y == 0 && direction.z == 0) return true;

		if (direction.IsDirection) return directionOpen[direction.ToDirectionIndex];

		if (DirectionIsOpen(new Coord(direction.x, 0, 0)) &&
		    DirectionIsOpen(new Coord(0, direction.y, 0)) &&
		    DirectionIsOpen(new Coord(0, 0, direction.z))) {
			return true;
		}

		if (cutCorners) {
			Coord x = new Coord(direction.x, 0, 0);
			Coord y = new Coord(0, direction.y, 0);
			Coord z = new Coord(0, 0, direction.z);
			return DirectionIsOpen(x) && NodeGrid.GetNode(position + x).DirectionIsOpen(y + z) ||
			       DirectionIsOpen(y) && NodeGrid.GetNode(position + y).DirectionIsOpen(x + z) ||
			       DirectionIsOpen(z) && NodeGrid.GetNode(position + z).DirectionIsOpen(x + y);
		}

		return false;
	}

	public void SetDirection(Coord direction, bool open) {
		if (!direction.IsDirection) throw new ArgumentException(direction.ToString());
		directionOpen[direction.ToDirectionIndex] = open;
	}

	public override string ToString() => $"Node {position}";

	public int HeapIndex { get; set; }

	public int CompareTo(Node other) {
		int compare = FCost.CompareTo(other.FCost);
		return -(compare == 0 ? hCost.CompareTo(other.hCost) : compare);
	}
}