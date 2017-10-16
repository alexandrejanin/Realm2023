using System.Collections.Generic;

public class BodyPart {
	public Body body;
	public readonly string name;

	public readonly Slot slot;
	public Equipable equipable;

	public BodyPart parent;
	public readonly List<BodyPart> children = new List<BodyPart>();

	public BodyPartX x;
	public BodyPartY y;
	public BodyPartZ z;

	public BodyPartAttribute[] bodyPartAttributes;

	public BodyPart(string name, BodyPart parent, Slot slot, BodyPartX x, BodyPartY y, BodyPartZ z, BodyPartAttribute[] bodyPartAttributes) {
		this.name = name;
		this.slot = slot;
		this.parent = parent;
		parent?.AddChild(this);
		this.x = x;
		this.y = y;
		this.z = z;
		this.bodyPartAttributes = bodyPartAttributes;
	}

	public void AddChild(BodyPart bodyPart) {
		if (!children.Contains(bodyPart)) children.Add(bodyPart);
		bodyPart.parent = this;
	}

	public void OnChosen(Equipment equipment) => equipment.ChooseBodyPart(this);

	public override string ToString() => name;
}

public enum BodyPartX {
	Middle,
	Left,
	Right
}

public enum BodyPartY {
	Middle,
	Top,
	Bottom
}

public enum BodyPartZ {
	Center,
	Front,
	Back
}

public enum BodyPartAttribute {
	Breathing,
	Flying,
	Grasping,
	InternalOrgans,
	Limb,
	Seeing,
	Thinking,
	Vital,
	Walking,
}