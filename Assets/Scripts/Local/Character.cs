using System.Collections.Generic;
using UnityEngine;

public class Character : Interactable {
	public readonly bool isPlayer;

	public Coord lookDirection = Coord.Forward;

	public Vector3 WorldPositionCenter => NodeGrid.GetWorldPosFromCoord(position, NodeGrid.NodeOffsetType.Center);
	public override Vector3 WorldPosition => NodeGrid.GetWorldPosFromCoord(position, NodeGrid.NodeOffsetType.CenterNoY);

	private int pathIndex;
	private Coord[] path;

	private Coord[] Path {
		get {
			if (path?.Length == 0) path = null;
			return path;
		}
		set {
			path = value;
			pathIndex = 0;
		}
	}

	public bool IsMoving => Path != null;
	public void StopPath() => Path = null;

	public bool isFemale;
	public Race race;
	private readonly string firstName;
	private readonly string lastName;
	public override string Name => firstName + " " + lastName;

	public readonly Body body;
	public readonly Inventory inventory;
	public readonly Equipment equipment;
	public bool HasItem(Item item) => inventory.Contains(item) || equipment.Contains(item);
	private readonly List<StatModifier> modifiers = new List<StatModifier>();

	public bool skipTurn;

	public Character(Coord position, bool isPlayer = false) : this(position, GameController.RandomRace(), Utility.RandomBool, isPlayer) { }

	public Character(Coord position, Race race, bool isFemale, bool isPlayer = false) : base(position) {
		this.race = race;
		this.isFemale = isFemale;
		firstName = race.GetFirstName(isFemale);
		lastName = race.GetLastName();

		this.isPlayer = isPlayer;
		if (isPlayer) ObjectManager.playerCharacter = this;

		inventory = new Inventory(this);
		equipment = new Equipment(this);

		body = new Body(BodyType.Humanoid);
	}

	public override bool ValidPosition(Coord pos) => (pos - position).MaxDimension <= 1;

	public int GetStat(Stat stat) {
		int statBase = 0;
		foreach (StatModifier statModifier in modifiers) {
			if (statModifier.stat == stat) statBase += statModifier.value;
		}
		foreach (Equipable equipable in equipment) {
			foreach (StatModifier statModifier in equipable.modifiers) {
				if (statModifier.stat == stat) statBase += statModifier.value;
			}
		}
		return statBase;
	}

	public void AddModifier(StatModifier modifier) {
		modifiers.Add(modifier);
	}

	public void RemoveModifier(StatModifier modifier) {
		modifiers.Remove(modifier);
	}

	public void TakeTurn() {
		if (skipTurn) {
			skipTurn = false;
			return;
		}

		if (Path != null) {
			ProcessPath();
		} else if (!isPlayer && CanSeeTo(ObjectManager.playerCharacter.position)) {
			lookDirection = ObjectManager.playerCharacter.position - position;
		}

		inventory.Update();
		equipment.Update();
	}

	private void ProcessPath() {
		MoveToCoord(Path[pathIndex]);
		pathIndex++;
		bool pathDone = pathIndex >= Path.Length;
		if (pathDone) {
			Path = null;
		}
	}

	private void Talk() { }

	public void Attack(Character character) {
		Log.Add(Name + " attacked " + character.Name + "! It's super effective!");
	}

	public override void MoveTo(Character character) => character.RequestPathToPositions(position.GetAdjacent(true, true));

	private void MoveToCoord(Coord coord) {
		lookDirection = (coord - position).Normalize;
		position = coord;
	}

	public void RequestPathToPosition(Coord goalCoord) => RequestPathToPositions(new[] {goalCoord});

	public void RequestPathToPositions(Coord[] goalCoords) {
		Coord[] waypoints = Pathfinder.FindPath(position, goalCoords);
		if (waypoints != null) {
			Path = waypoints;
		}
	}

	public override List<Interaction> GetInteractions(Character character) {
		List<Interaction> interactions = GetBasicInteractions(character);
		if (character != this) {
			if (CanBeSeenFrom(character.position)) interactions.Add(new Interaction("Talk", Talk, true));
			if ((character.position - position).MaxDimension <= 1) interactions.Add(new Interaction("Attack", () => character.Attack(this), false));
		}
		return interactions;
	}

	protected override string InspectText() => Name;
}