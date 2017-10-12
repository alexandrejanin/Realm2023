using System.Collections.Generic;
using UnityEngine;

public static class NodeGrid {
	public static int Width => size.x;
	public static int Height => size.y;
	public static int Length => size.z;
	public static Coord size;

	public static int GridCount => Width * Height * Length;

	public static Coord RandomPoint => Coord.RandomRange(Coord.Zero, size);

	public static Node[,,] grid;

	public static void CreateGrid(Location location) {
		size = location.Size;
		grid = new Node[Width, Height, Length];

		for (int y = 0; y < Height; y++)
			for (int z = 0; z < Length; z++)
				for (int x = 0; x < Width; x++) {
					bool[] directionOpen = {
						y < Height - 1,
						y > 0,
						x > 0,
						x < Width - 1,
						z > 0,
						z < Length - 1
					};

					grid[x, y, z] = new Node(new Coord(x, y, z), 0, directionOpen);
				}
	}

	public static void BlockPassage(Coord position, Coord direction) {
		SetPassage(position, direction, false);
	}

	public static void OpenPassage(Coord position, Coord direction) {
		SetPassage(position, direction, true);
	}

	private static void SetPassage(Coord position, Coord direction, bool open) {
		direction = direction.Normalize;
		Node a = GetNode(position);
		Node b = GetNode(position + direction);

		a?.SetDirection(direction, open);
		b?.SetDirection(-direction, open);
	}

	public static List<Coord> GetLine(Coord start, Coord end) {
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

	public static bool IsVisible(Coord a, Coord b) {
		Vector3 posA = GetWorldPosFromCoord(a, NodeOffsetType.Center);
		Vector3 posB = GetWorldPosFromCoord(b, NodeOffsetType.Center);
		return !Physics.Linecast(posA, posB, ObjectManager.TerrainMask);
	}

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


	public static bool IsInGrid(Coord coord) => coord.x >= 0 && coord.x < Width && coord.y >= 0 && coord.y < Height && coord.z >= 0 && coord.z < Length;
	public static Node GetNode(Coord coord) => IsInGrid(coord) ? grid[coord.x, coord.y, coord.z] : null;

	public static IEnumerable<Node> GetNeighbors(Node node) {
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

	private static Node GetNeighbor(Node startNode, Coord direction, bool aerial = false) {
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

	public static Node GetNeighbor(Coord startCoord, Coord direction, bool aerial = false) => GetNeighbor(GetNode(startCoord), direction, aerial);
}