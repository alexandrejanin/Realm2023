using UnityEngine;

public class UnitObject : MonoBehaviour {
    private Unit unit;

    public void SetUnit(Unit unit) {
        this.unit = unit;
    }

    public void Vis() {
        if (unit != null)
            transform.position = GameManager.WorldManager.TileToVector3(unit.Tile);
    }
}