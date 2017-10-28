using System.Collections.Generic;

public class Door : Interactable {
	public bool open;
	public bool locked;

	public override string Name => "Door";

	public readonly Doorway doorway;

	public Coord Direction => doorway.direction;

	public Door(Doorway doorway) : base(doorway.location, doorway.position) {
		this.doorway = doorway;
		NodeGrid.BlockPassage(position, Direction);
	}

	public override bool ValidPosition(Coord pos) => pos == position || pos == position + Direction;

	public override List<Interaction> GetInteractions(Character character) {
		List<Interaction> interactions = GetBasicInteractions(character);

		if (ValidPosition(character.position)) {
			if (locked) {
				interactions.Add(new Interaction("Unlock", Unlock, true));
			} else {
				if (open) {
					interactions.Add(new Interaction("Close", Close, true));
				} else {
					interactions.Add(new Interaction("Open", Open, true));
					interactions.Add(new Interaction("Lock", Lock, true));
				}
			}
		}
		return interactions;
	}

	public override bool CanBeSeenFrom(Coord from) => doorway.CanBeSeenFrom(from);

	public override bool CanSeeTo(Coord to) => doorway.CanSeeTo(to);

	public override void MoveTo(Character character) => character.RequestPathToPositions(new[] {position, position + Direction});

	public void Open() {
		open = true;
		NodeGrid.OpenPassage(position, Direction);
	}

	public void Close() {
		open = false;
		NodeGrid.BlockPassage(position, Direction);
	}

	public void Lock() {
		locked = true;
	}

	public void Unlock() {
		locked = false;
	}
}