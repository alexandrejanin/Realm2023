﻿using System.Collections.Generic;
using UnityEngine;

public class NodeGrid : MonoBehaviour {
	private static NodeGrid instance;

	[SerializeField] private bool drawGizmos;

	[SerializeField] private int gridWidth, gridHeight, gridLength;

	public static int GridWidth => instance.gridWidth;

	public static int GridHeight => instance.gridHeight;

	public static int GridLength => instance.gridLength;

	public static Coord GridSize => new Coord(GridWidth, GridHeight, GridLength);

	public static int GridCount => GridWidth * GridHeight * GridLength;

	public static Coord RandomPoint => Coord.RandomRange(Coord.Zero, GridSize);

	public Node[,,] grid;

	private void Awake() {
		instance = this;
	}

	public static void CreateGrid() {
		instance.CreateInstanceGrid();
	}

	private void CreateInstanceGrid() {
		grid = new Node[gridWidth, gridHeight, gridLength];

		for (int y = 0; y < gridHeight; y++)
			for (int z = 0; z < gridLength; z++)
				for (int x = 0; x < gridWidth; x++) {
					Dictionary<Coord, bool> directionOpen = new Dictionary<Coord, bool> {
						{Coord.Zero, true},
						{Coord.Down, y > 0},
						{Coord.Up, y < gridHeight - 1},
						{Coord.Left, x > 0},
						{Coord.Right, x < gridWidth - 1},
						{Coord.Back, z > 0},
						{Coord.Forward, z < gridLength - 1}
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

	public static bool IsInGrid(Coord coord) => coord.x >= 0 && coord.x < GridWidth && coord.y >= 0 && coord.y < GridHeight && coord.z >= 0 && coord.z < GridLength;

	public enum NodeOffsetType {
		None,
		Center,
		CenterNoY
	}

	public static Vector3 GetWorldPosFromCoord(int x, int y, int z, NodeOffsetType nodeOffsetType) => new Vector3(x, y, z) + NodeOffset(nodeOffsetType);

	public static Vector3 GetWorldPosFromCoord(Coord coord, NodeOffsetType nodeOffsetType) => coord + NodeOffset(nodeOffsetType);

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

	public static Coord GetCoordFromWorldPos(Vector3 worldPos) {
		if (worldPos.y % 1 == 0.5f) {
			worldPos.y = worldPos.y - 0.5f;
		}
		return new Coord(
			Mathf.RoundToInt((worldPos.x - Coord.One.x / 2) / Coord.One.x),
			Mathf.RoundToInt((worldPos.y - Coord.One.y / 2 + 0.01f) / Coord.One.y),
			Mathf.RoundToInt((worldPos.z - Coord.One.z / 2) / Coord.One.z));
	}

	public static Node GetNode(Coord coord) => IsInGrid(coord) ? instance.grid[coord.x, coord.y, coord.z] : null;

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

	private void OnDrawGizmos() {
		if (grid != null && drawGizmos) {
			foreach (Node node in grid) {
				if (node.IsWalkable) {
					foreach (KeyValuePair<Coord, bool> kvp in node.directionOpen) {
						Vector3 direction = kvp.Key;
						bool passable = kvp.Value;
						if (passable) {
							Gizmos.DrawLine(node.WorldPositionCenter, node.WorldPositionCenter + direction / 2);
						}
					}
				}
			}
		}
	}
}