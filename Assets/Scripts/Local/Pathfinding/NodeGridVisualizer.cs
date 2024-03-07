using UnityEngine;

public class NodeGridVisualizer : MonoBehaviour {
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        for (var y = 0; y < NodeGrid.Height; y++)
            for (var x = 0; x < NodeGrid.Width; x++)
                for (var z = 0; z < NodeGrid.Length; z++) {
                    var position = new Coord(x, y, z);
                    var node = NodeGrid.GetNode(position);

                    foreach (var direction in new[] { Coord.Forward, Coord.Back, Coord.Up, Coord.Down, Coord.Left, Coord.Right }) {
                        if (!node.DirectionIsOpen(direction)) {
                            Gizmos.DrawCube(
                                NodeGrid.GetWorldPosFromCoord(position) + 0.5f * (Vector3)direction,
                                0.8f * Vector3.one - 0.7f * (Vector3)direction.Abs
                            );
                        }
                    }
                }
    }
}