using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Character : Interactable {
    public readonly bool isPlayer;

    public Coord lookDirection = Coord.Forward;

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

    public Character(Location location, Coord position, bool isPlayer = false) : this(location, position, GameManager.Database.RandomRace(), Utility.RandomBool, isPlayer) { }

    public Character(Location location, Coord position, Race race, bool isFemale, bool isPlayer = false) : base(location, position) {
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
        var statBase = 0;
        foreach (var statModifier in modifiers) {
            if (statModifier.stat == stat) statBase += statModifier.value;
        }

        foreach (var equipable in equipment) {
            statBase += equipable.modifiers.Where(statModifier => statModifier.stat == stat).Sum(statModifier => statModifier.value);
        }

        return statBase;
    }

    public void TakeTurn() {
        if (Path != null) {
            ProcessPath();
        }
        else if (!isPlayer && CanSeeTo(ObjectManager.playerCharacter.position)) {
            lookDirection = ObjectManager.playerCharacter.position - position;
        }

        inventory.Update();
        equipment.Update();
    }

    private void ProcessPath() {
        MoveToCoord(Path[pathIndex]);
        pathIndex++;
        var pathDone = pathIndex >= Path.Length;
        if (pathDone) {
            Path = null;
        }
    }

    private void Talk(Character character) {
        GameManager.LocalManager.DialogueManager.EnqueueSentence(new Sentence(Name, Utility.RandomBool ? "Hi!" : $"Hello, {character.Name}!"));
    }

    public void Attack(Character target) {
        if (isPlayer) {
            Player.DisplayInteractions("Attack " + Name, target.body.Select(bodyPart => new Interaction(bodyPart.name, () => bodyPart.Attack(this), true)).ToList());
        }
    }

    public override void MoveTo(Character character) => character.RequestPathToPositions(position.GetAdjacent(true, true));

    private void MoveToCoord(Coord coord) {
        lookDirection = (coord - position).Normalize;
        position = coord;
    }

    public void RequestPathToPosition(Coord goalCoord) => RequestPathToPositions(new[] {goalCoord});

    public void RequestPathToPositions(Coord[] goalCoords) {
        var waypoints = Pathfinder.FindPath(position, goalCoords);
        if (waypoints != null) {
            Path = waypoints;
        }
    }

    public override List<Interaction> GetInteractions(Character character) {
        var interactions = GetBasicInteractions(character);
        if (character != this) {
            if (CanBeSeenFrom(character.position)) interactions.Add(new Interaction("Talk", () => Talk(character), true));
            if ((character.position - position).MaxDimension <= 1) interactions.Add(new Interaction("Attack", () => character.Attack(this), false));
        }

        return interactions;
    }

    protected override string InspectText() => Name;
}