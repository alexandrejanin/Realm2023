using UnityEngine;

public class Town : Location {
    public string Name { get; }
    public Civilization civilization;
    public Race Race => civilization.race;
    public int population;

    private float settlerSpawn;

    public Town(Tile tile, Civilization civilization, int size, int population) : base(tile, size, 20) {
        this.civilization = civilization;
        this.population = population;

        Name = Race.GetPlaceName();

        tile.customColor = Color.black;
    }

    public override void Sim() {
        settlerSpawn += Mathf.Log(population) / 100f;

        if (settlerSpawn >= 1f) {
            SpawnSettler();
            settlerSpawn = 0;
        }
    }

    private void SpawnSettler() {
        var settler = new Settler(tile, this);
        GameManager.World.SpawnUnit(settler);
    }

    private string GetSize() =>
        population > 5000
            ? "city"
            : population > 1000
                ? "town"
                : population > 100
                    ? "village"
                    : "settlement";

    public override string ToString() => $"{Name}, {Race.adjective} {GetSize()}";
}