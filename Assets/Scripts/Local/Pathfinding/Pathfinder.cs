using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Pathfinder {
	public static Coord[] FindPath(Coord startPos, Coord goalPos, bool useWeights) {
		bool success = false;

		Node startNode = NodeGrid.GetNode(startPos);
		Node goalNode = NodeGrid.GetNode(goalPos);
		if (startNode != null && goalNode != null && goalNode.IsWalkable && goalNode != startNode) {
			Heap<Node> open = new Heap<Node>(NodeGrid.GridCount);
			HashSet<Node> closed = new HashSet<Node>();

			open.Add(startNode);

			while (open.Count > 0) {
				Node current = open.RemoveFirst();
				closed.Add(current);

				if (current == goalNode) {
					success = true;

					break;
				}

				foreach (Node neighbor in NodeGrid.GetNeighbors(current)) {
					if (!neighbor.IsWalkable || closed.Contains(neighbor)) {
						continue;
					}

					int newMovementCost = current.gCost + GetDistance(current, neighbor) + (useWeights ? neighbor.movementPenalty : 0);

					if (newMovementCost < neighbor.gCost || !open.Contains(neighbor)) {
						neighbor.gCost = newMovementCost;
						neighbor.hCost = GetDistance(neighbor, goalNode);
						neighbor.parent = current;

						if (!open.Contains(neighbor)) {
							open.Add(neighbor);
						} else {
							open.UpdateItem(neighbor);
						}
					}
				}
			}
		}

		Coord[] waypoints = null;
		if (success) {
			waypoints = RetracePath(startNode, goalNode);
		}

		return waypoints == null || waypoints.Length == 0 ? null : waypoints;
	}

	private static Coord[] RetracePath(Node start, Node goal) {
		List<Node> nodePath = new List<Node>();
		Node current = goal;

		while (current != start) {
			nodePath.Add(current);
			current = current.parent;
		}
		nodePath.Reverse();

		Coord[] waypoints = nodePath.Select(node => node.position).ToArray();

		return waypoints;
	}

	public static int GetDistance(Coord a, Coord b) {
		int x = Mathf.Abs(a.x - b.x);
		int y = Mathf.Abs(a.y - b.y);
		int z = Mathf.Abs(a.z - b.z);

		int distance = x > z ? 14 * z + 10 * (x - z) + 14 * y : 14 * x + 10 * (z - x) + 14 * y;

		return distance;
	}

	public static int GetDistance(Node a, Node b) => GetDistance(a.position, b.position);
}