[System.Serializable]
public class WorldSim {
    public void Sim(World world) {
        foreach (var town in world.Towns) {
            town.Sim();
        }
        foreach (var unit in world.Units) {
            unit.Sim();
        }
    }
}