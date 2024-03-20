public abstract class Unit {
    public Tile Tile { get; protected set; }

    protected Unit(Tile tile) {
        Tile = tile;
    }

    public abstract void Sim();
}
