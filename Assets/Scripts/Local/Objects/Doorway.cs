public class Doorway : Wall {
    public readonly Door door;

    public Doorway(Location location, Coord position, Coord direction, WallType wallType) : base(location, position, direction, wallType) {
        door = new Door(this);
        location.interactables.Add(door);
    }
}