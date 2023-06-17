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

    public const int maxViewDistanceSquared = 225;

    public static void CreateGrid(Location location) {
        size = new Coord(location.size, location.height, location.size);
        grid = new Node[Width, Height, Length];

        for (var y = 0; y < Height; y++) {
            for (var z = 0; z < Length; z++) {
                for (var x = 0; x < Width; x++) {
                    bool[] directionOpen = {
                        x < Width - 1,
                        x > 0,
                        y < Height - 1,
                        y > 0,
                        z < Length - 1,
                        z > 0
                    };

                    grid[x, y, z] = new Node(new Coord(x, y, z), 0, directionOpen);
                }
            }
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

        GetNode(position)?.SetDirection(direction, open);
        GetNode(position + direction)?.SetDirection(-direction, open);
    }

    public static List<Coord> GetLine(Coord start, Coord end) {
        var line = new List<Coord>();

        var dist = new Coord(end.x - start.x, end.y - start.y, end.z - start.z);
        var abs = new Coord(dist.x.Abs() << 1, dist.y.Abs() << 1, dist.z.Abs() << 1);
        var sign = new Coord(dist.x.Sign(), dist.y.Sign(), dist.z.Sign());

        var current = start;

        var max = abs.MaxDimension;
        var maxX = max == abs.x;
        var maxY = !maxX && max == abs.y;
        var maxZ = !maxX && !maxY && max == abs.z;

        var xd = abs.x - (max >> 1);
        var yd = abs.y - (max >> 1);
        var zd = abs.z - (max >> 1);

        for (;;) {
            line.Add(current);
            if (maxX && current.x == end.x ||
                maxY && current.y == end.y ||
                maxZ && current.z == end.z) break;

            if (!maxX && xd >= 0) {
                current.x += sign.x;
                xd -= max;
            }

            if (!maxY && yd >= 0) {
                current.y += sign.y;
                yd -= max;
            }

            if (!maxZ && zd >= 0) {
                current.z += sign.z;
                zd -= max;
            }

            if (maxX) {
                current.x += sign.x;
                yd += abs.y;
                zd += abs.z;
            }
            else if (maxY) {
                current.y += sign.y;
                xd += abs.x;
                zd += abs.z;
            }
            else if (maxZ) {
                current.z += sign.z;
                xd += abs.x;
                yd += abs.y;
            }
        }

        return line;
    }

    public static bool IsVisible(Coord start, Coord end, Vector3 normal = new Vector3(), int viewDist = maxViewDistanceSquared) {
        if (start == end)
            return true;

        if ((start.x - end.x) * (start.x - end.x) + (start.z - end.z) * (start.z - end.z) > viewDist)
            return false;

        var line = GetLine(start, end);

        if (Vector3.Angle(end - start, normal) < 90f)
            return false;

        for (var i = 0; i < line.Count - 1; i++) {
            var current = GetNode(line[i]);
            var next = GetNode(line[i + 1]);
            if (current == null || next == null || !CanWalkBetweenNodes(current, next, true))
                return false;
        }

        return true;
    }

    private static bool CanWalkBetweenNodes(Node a, Node b, bool permissive) => a.DirectionIsOpen(b.position - a.position, permissive) && b.DirectionIsOpen(a.position - b.position, permissive);

    public enum NodeOffsetType {
        Center,
        CenterNoY
    }

    public static Vector3 NodeOffset(NodeOffsetType nodeOffsetType) {
        switch (nodeOffsetType) {
            case NodeOffsetType.Center:
                return new Vector3(0.5f, 0.5f, 0.5f);
            case NodeOffsetType.CenterNoY:
                return new Vector3(0.5f, 0, 0.5f);
            default:
                return Vector3.zero;
        }
    }

    public static Vector3 GetWorldPosFromCoord(Coord coord, NodeOffsetType nodeOffsetType) => coord + NodeOffset(nodeOffsetType);

    public static Coord GetCoordFromWorldPos(Vector3 worldPos) => new Coord(
        Mathf.FloorToInt(worldPos.x),
        Mathf.FloorToInt(worldPos.y + 0.05f),
        Mathf.FloorToInt(worldPos.z)
    );


    public static bool IsInGrid(Coord coord) => coord.x >= 0 && coord.x < Width && coord.y >= 0 && coord.y < Height && coord.z >= 0 && coord.z < Length;
    public static Node GetNode(Coord coord) => IsInGrid(coord) ? grid[coord.x, coord.y, coord.z] : null;

    public static IEnumerable<Node> GetNeighbors(Node node) {
        var neighbors = new List<Node>();

        for (var y = -1; y <= 1; y++) {
            for (var z = -1; z <= 1; z++) {
                for (var x = -1; x <= 1; x++) {
                    if (y == 0 && z == 0 && x == 0)
                        continue;

                    var neighbor = GetNeighbor(node, new Coord(x, y, z));
                    if (neighbor != null) {
                        neighbors.Add(neighbor);
                    }
                }
            }
        }

        return neighbors;
    }

    private static Node GetNeighbor(Node startNode, Coord direction, bool aerial = false) {
        var neighbor = GetNode(startNode.position + direction);
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