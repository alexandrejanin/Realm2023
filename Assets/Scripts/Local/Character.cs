using System.Collections.Generic;
using UnityEngine;

public class Character : Interactable {
	public readonly bool isPlayer;

	public Coord lookDirection = Coord.forward;

	public Vector3 WorldPositionCenter => NodeGrid.GetWorldPosFromCoord(position, NodeGrid.NodeOffsetType.Center);
	public override Vector3 WorldPosition => NodeGrid.GetWorldPosFromCoord(position, NodeGrid.NodeOffsetType.CenterNoY);

	private int pathIndex;
	private Coord[] path;

	public Coord[] Path {
		get {
			if (path?.Length == 0) path = null;
			return path;
		}
		set {
			path = value;
			pathIndex = 0;
		}
	}

	public bool isFemale;
	public Race race;
	private readonly string firstName;
	private readonly string lastName;
	public override string Name => firstName + " " + lastName;

	public readonly Inventory inventory;
	public readonly Equipment equipment;
	public bool HasItem(Item item) => inventory.Contains(item) || equipment.Contains(item);
	private readonly List<StatModifier> modifiers = new List<StatModifier>();

	public Body body;

	public Character(Coord position, bool isPlayer = false) : this(position, GameController.RandomRace(), Utility.RandomBool, isPlayer) { }

	public Character(Coord position, Race race, bool isFemale, bool isPlayer = false) : base(position) {
		this.race = race;
		this.isFemale = isFemale;
		firstName = race.GetFirstName(isFemale);
		lastName = race.GetLastName();

		this.isPlayer = isPlayer;
		if (isPlayer) ObjectManager.SetPlayer(this);

		inventory = new Inventory(this);
		equipment = new Equipment(this);

		body = new Body(BodyType.Humanoid);
	}

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

	public override void StartTurn() {
		if (Path != null) {
			ProcessPath();
		}
		inventory.Update();
		equipment.Update();
		base.StartTurn();
	}

	public override void EndTurn() {
		if (!isPlayer) {
			if (Path == null) {
				RequestPathToPos(ObjectManager.playerCharacter.position);
			}
		}
		base.EndTurn();
	}

	private void ProcessPath() {
		MoveToCoord(Path[pathIndex]);
		pathIndex++;
		bool pathDone = pathIndex >= Path.Length;
		if (pathDone) {
			Path = null;
		}
	}

	private void MoveToCoord(Coord coord) {
		lookDirection = (coord - position).Normalize;
		position = coord;
	}

	public void RequestPathToPos(Coord goalCoord) {
		Coord[] waypoints = Pathfinder.FindPath(position, goalCoord, !isPlayer);
		if (waypoints != null) {
			Path = waypoints;
		}
	}

	public override List<Interaction> GetInteractions(Character character) => GetBasicInteractions(character);

	protected override string InspectText() => Name;
}