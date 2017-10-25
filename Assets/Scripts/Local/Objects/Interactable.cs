using System;
using System.Collections.Generic;

public abstract class Interactable : Entity {
	protected Interactable(Coord position) : base(position) { }

	public virtual bool ValidPosition(Coord pos) => pos == position;

	public abstract List<Interaction> GetInteractions(Character character);

	protected List<Interaction> GetBasicInteractions(Character character) {
		List<Interaction> interactions = new List<Interaction> {new Interaction("Inspect", Inspect, false)};
		if (!ValidPosition(character.position)) interactions.Add(new Interaction("Move To", () => MoveTo(character), false));
		return interactions;
	}

	private void Inspect() => Log.Add(InspectText());

	protected virtual string InspectText() => Name;

	public virtual void MoveTo(Character character) {
		character.RequestPathToPosition(position);
	}

	public override string ToString() => Name;
}

public class Interaction {
	public readonly string name;
	public readonly Action action;
	public readonly bool skipTurn;

	public Interaction(string name, Action action, bool skipTurn) {
		this.name = name;
		this.action = action;
		this.skipTurn = skipTurn;
	}
}