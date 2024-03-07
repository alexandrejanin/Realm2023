using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Pathfinder {
    public static Coord[] FindPath(Coord startPos, IList<Coord> goalPositions) {
        var singleTarget = goalPositions.Count == 1;
        var success = false;

        var goals = new HashSet<Node>(goalPositions.Select(NodeGrid.GetNode));

        var startNode = NodeGrid.GetNode(startPos);
        var goalNode = NodeGrid.GetNode(goalPositions[0]);
        
        if (startNode != null && goalNode != null && goalNode.IsWalkable && goalNode != startNode) {
            var open = new Heap<Node>(NodeGrid.GridCount);
            var closed = new HashSet<Node>();

            open.Add(startNode);

            while (open.Count > 0) {
                var current = open.RemoveFirst();
                closed.Add(current);

                if (singleTarget && current == goalNode ||
                    !singleTarget && goals.Contains(current) /*current == goalNode*/) {
                    goalNode = current;
                    success = true;
                    break;
                }

                foreach (var neighbor in NodeGrid.GetNeighbors(current)) {
                    if (!neighbor.IsWalkable || closed.Contains(neighbor)) {
                        continue;
                    }

                    var newMovementCost = current.gCost + GetDistance(current, neighbor);

                    if (newMovementCost < neighbor.gCost || !open.Contains(neighbor)) {
                        neighbor.gCost = newMovementCost;
                        neighbor.hCost = GetDistance(neighbor, goalNode);
                        neighbor.parent = current;

                        if (!open.Contains(neighbor)) {
                            open.Add(neighbor);
                        }
                        else {
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
        var nodePath = new List<Node>();
        var current = goal;

        while (current != start) {
            nodePath.Add(current);
            current = current.parent;
        }

        nodePath.Reverse();

        var waypoints = nodePath.Select(node => node.position).ToArray();

        return waypoints;
    }

    public static int GetDistance(Coord a, Coord b) {
        var x = Mathf.Abs(a.x - b.x);
        var y = Mathf.Abs(a.y - b.y);
        var z = Mathf.Abs(a.z - b.z);

        var distance = x > z 
            ? 14 * z + 10 * (x - z) + 14 * y
            : 14 * x + 10 * (z - x) + 14 * y;

        return distance;
    }

    public static int GetDistance(Node a, Node b) => GetDistance(a.position, b.position);
}