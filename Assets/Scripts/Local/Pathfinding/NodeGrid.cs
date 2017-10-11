using System.Collections.Generic;
using UnityEngine;

public class NodeGrid {
	private static NodeGrid Current => GameController.NodeGrid;

	public int Width => size.x;
	public int Height => size.y;
	public int Length => size.z;
	public readonly Coord size;

	public int GridCount => Width * Height * Length;

	public Coord RandomPoint => Coord.RandomRange(Coord.Zero, size);

	public Node[,,] grid;

	public NodeGrid(Coord size) {
		this.size = size;
		CreateGrid();
	}

	private void CreateGrid() {
		grid = new Node[Width, Height, Length];

		for (int y = 0; y < Height; y++)
			for (int z = 0; z < Length; z++)
				for (int x = 0; x < Width; x++) {
					Dictionary<Coord, bool> directionOpen = new Dictionary<Coord, bool> {
						{Coord.Zero, true},
						{Coord.Down, y > 0},
						{Coord.Up, y < Height - 1},
						{Coord.Left, x > 0},
						{Coord.Right, x < Width - 1},
						{Coord.Back, z > 0},
						{Coord.Forward, z < Length - 1}
					};

					grid[x, y, z] = new Node(new Coord(x, y, z), 0, directionOpen);
				}
	}

	public void BlockPassage(Coord position, Coord direction) {
		SetPassage(position, direction, false);
	}

	public void OpenPassage(Coord position, Coord direction) {
		SetPassage(position, direction, true);
	}

	private void SetPassage(Coord position, Coord direction, bool open) {
		direction = direction.Normalize;
		Node a = GetNode(position);
		Node b = GetNode(position + direction);

		a?.SetDirection(direction, open);
		b?.SetDirection(-direction, open);
	}

	public List<Coord> GetLine(Coord start, Coord end) {
		List<Coord> positions = new List<Coord>();

		Vector3 startVector3 = start;
		Vector3 endVector3 = end;
		Vector3 direction = endVector3 - startVector3;

		bool vertical = direction.z > direction.x;

		Vector3 step = vertical ? direction / direction.z : direction / direction.x;
		Vector3 current = startVector3;

		for (;;) {
			positions.Add(new Coord(current));
			current += step;
			if ((current - startVector3).magnitude > (endVector3 - startVector3).magnitude) {
				break;
			}
		}

		return positions;
	}

	public bool IsVisible(Coord a, Coord b) {
		Vector3 posA = GetWorldPosFromCoord(a, NodeOffsetType.Center);
		Vector3 posB = GetWorldPosFromCoord(b, NodeOffsetType.Center);
		return !Physics.Linecast(posA, posB, ObjectManager.TerrainMask);
	}

	public bool IsInGrid(Coord coord) => coord.x >= 0 && coord.x < Width && coord.y >= 0 && coord.y < Height && coord.z >= 0 && coord.z < Length;

	public enum NodeOffsetType {
		None,
		Center,
		CenterNoY
	}

	public static Vector3 NodeOffset(NodeOffsetType nodeOffsetType) {
		switch (nodeOffsetType) {
			case NodeOffsetType.Center:
				return new Vector3(0.5f, 0.5f, 0.5f);
			case NodeOffsetType.CenterNoY:
				return new Vector3(0.5f, 0, 0.5f);
			case NodeOffsetType.None:
				return Vector3.zero;
			default:
				return Vector3.zero;
		}
	}

	public static Vector3 GetWorldPosFromCoord(int x, int y, int z, NodeOffsetType nodeOffsetType) => new Vector3(x, y, z) + NodeOffset(nodeOffsetType);

	public static Vector3 GetWorldPosFromCoord(Coord coord, NodeOffsetType nodeOffsetType) => coord + NodeOffset(nodeOffsetType);

	public static Coord GetCoordFromWorldPos(Vector3 worldPos) => new Coord(
		Mathf.FloorToInt(worldPos.x),
		Mathf.FloorToInt(worldPos.y + 0.05f),
		Mathf.FloorToInt(worldPos.z)
	);

	public Node GetNode(Coord coord) => IsInGrid(coord) ? grid[coord.x, coord.y, coord.z] : null;

	public IEnumerable<Node> GetNeighbors(Node node) {
		List<Node> neighbors = new List<Node>();

		for (int y = -1; y <= 1; y++) {
			for (int z = -1; z <= 1; z++) {
				for (int x = -1; x <= 1; x++) {
					Coord direction = new Coord(x, y, z);
					if (direction != Coord.Zero) {
						Node neighbor = GetNeighbor(node, direction);
						if (neighbor != null) {
							neighbors.Add(neighbor);
						}
					}
				}
			}
		}

		return neighbors;
	}

	private Node GetNeighbor(Node startNode, Coord direction, bool aerial = false) {
		Node neighbor = GetNode(startNode.position + direction);
		if (neighbor == null || !aerial && !neighbor.IsWalkable || direction.MaxDimension > 1 || direction == Coord.Zero) return null;

		if (direction.y == 0) {
			if (startNode.DirectionIsOpen(direction) && neighbor.DirectionIsOpen(-direction)) return neighbor;
		}

		if (direction.y > 0) {
			direction.y = 0;
			if (!startNode.DirectionIsOpen(direction) && startNode.DirectionIsOpen(Coord.Up) && neighbor.DirectionIsOpen(-direction)) return neighbor;
		}

		if (direction.y < 0) {
			direction.y = 0;
			if (startNode.DirectionIsOpen(direction) && neighbor.DirectionIsOpen(Coord.Up)) return neighbor;
		}
		return null;
	}

	public Node GetNeighbor(Coord startCoord, Coord direction, bool aerial = false) => GetNeighbor(GetNode(startCoord), direction, aerial);
}