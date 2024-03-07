public abstract class Unit {
    public Tile Tile { get; protected set; }

    public Unit(Tile tile) {
        Tile = tile;
    }

    public abstract void Sim();
}
