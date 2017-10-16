using System.Collections.Generic;

public class Door : Wall {
	public bool open;
	public bool locked;

	public override string Name => "Door";

	public Door(Coord position, Coord direction) : base(position, direction, WallType.Wood) {
		NodeGrid.BlockPassage(position, direction);
	}

	public override bool ValidPosition(Coord pos) => pos == position || pos == position + direction;

	public override List<Interaction> GetInteractions(Character character) {
		List<Interaction> interactions = GetBasicInteractions(character);

		if (ValidPosition(character.position)) {
			if (locked) {
				interactions.Add(new Interaction("Unlock", Unlock));
			} else {
				if (open) {
					interactions.Add(new Interaction("Close", Close));
				} else {
					interactions.Add(new Interaction("Open", Open));
					interactions.Add(new Interaction("Lock", Lock));
				}
			}
		}
		return interactions;
	}

	public override void MoveTo(Character character) {
		character.RequestPathToPositions(new[] {position, position + direction});
		/*Coord[] path1 = Pathfinder.FindPath(character.position, position);
		Coord[] path2 = Pathfinder.FindPath(character.position, position + direction);
		if (path1 != null && path2 != null) {
			character.Path = path1.Length > path2.Length ? path2 : path1;
		} else if (path1 != null) {
			character.Path = path1;
		} else if (path2 != null) {
			character.Path = path2;
		}*/
	}

	public void Open() {
		open = true;
		NodeGrid.OpenPassage(position, direction);
	}

	public void Close() {
		open = false;
		NodeGrid.BlockPassage(position, direction);
	}

	public void Lock() {
		locked = true;
	}

	public void Unlock() {
		locked = false;
	}
}