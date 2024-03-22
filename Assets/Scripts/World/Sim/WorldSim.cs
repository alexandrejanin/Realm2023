using System.Collections.Generic;

[System.Serializable]
public class WorldSim {
    public void Sim(World world) {
        foreach (var town in world.Towns) {
            town.Sim();
        }

        var unitsToDestroy = new HashSet<Unit>();
        foreach (var unit in world.Units) {
            if (unit.Sim())
                unitsToDestroy.Add(unit);
        }

        foreach (var unit in unitsToDestroy) {
            world.DestroyUnit(unit);
        }
    }
}