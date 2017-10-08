using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : Entity {
	protected Interactable(Coord position) : base(position) { }

	protected virtual bool ValidPosition(Coord pos) => pos == position;

	public abstract List<Interaction> GetInteractions(Character character);

	protected List<Interaction> GetBasicInteractions(Character character) {
		List<Interaction> interactions = new List<Interaction> {new Interaction("Inspect", Inspect)};
		if (!ValidPosition(character.position)) interactions.Add(new Interaction("Move To", () => MoveTo(character)));
		return interactions;
	}

	private void Inspect() => Debug.Log(InspectText());

	protected virtual string InspectText() => Name;

	public virtual void MoveTo(Character character) {
		character.RequestPathToPos(position);
	}

	public override string ToString() => Name;
}

public class Interaction {
	public readonly string name;
	public readonly Action action;

	public Interaction(string name, Action action) {
		this.name = name;
		this.action = action;
	}
}