public class Doorway : Wall {

	public readonly Door door;

	public Doorway(Coord position, Coord direction, WallType wallType) : base(position, direction, wallType) {
		door = new Door(this);
	}
}