using System;
using System.Collections;
using System.Collections.Generic;

public class Body : IEnumerable<BodyPart> {
	private readonly List<BodyPart> bodyParts;

	public IEnumerable<Equipable> Equipables {
		get {
			List<Equipable> equipables = new List<Equipable>();
			foreach (BodyPart bodyPart in bodyParts) {
				if (bodyPart.equipable != null && !equipables.Contains(bodyPart.equipable)) {
					equipables.Add(bodyPart.equipable);
				}
			}
			return equipables;
		}
	}

	public Body(BodyType bodyType) {
		bodyParts = GetBody(bodyType);

		foreach (BodyPart bodyPart in bodyParts) {
			bodyPart.body = this;
		}
	}

	private static List<BodyPart> GetBody(BodyType bodyType) {
		List<BodyPart> bodyParts;

		switch (bodyType) {
			case BodyType.Humanoid:
				BodyPart torso = new BodyPart("Torso", null, Slot.Torso, BodyPartX.Middle, BodyPartY.Middle, BodyPartZ.Center, new[] {Attribute.Breathing, Attribute.InternalOrgans, Attribute.Vital});
				BodyPart abdomen = new BodyPart("Abdomen", torso, Slot.None, BodyPartX.Middle, BodyPartY.Bottom, BodyPartZ.Center, new[] {Attribute.InternalOrgans, Attribute.Vital});
				BodyPart neck = new BodyPart("Neck", torso, Slot.Neck, BodyPartX.Middle, BodyPartY.Top, BodyPartZ.Center, new[] {Attribute.Breathing, Attribute.InternalOrgans, Attribute.Vital});
				BodyPart head = new BodyPart("Head", neck, Slot.Head, BodyPartX.Middle, BodyPartY.Top, BodyPartZ.Center, new[] {Attribute.Breathing, Attribute.InternalOrgans, Attribute.Seeing, Attribute.Thinking, Attribute.Vital});
				BodyPart leftArm = new BodyPart("Left Arm", torso, Slot.None, BodyPartX.Left, BodyPartY.Middle, BodyPartZ.Center, new[] {Attribute.Limb});
				BodyPart rightArm = new BodyPart("Left Arm", torso, Slot.None, BodyPartX.Right, BodyPartY.Middle, BodyPartZ.Center, new[] {Attribute.Limb});
				BodyPart leftHand = new BodyPart("Left Hand", leftArm, Slot.Hand, BodyPartX.Left, BodyPartY.Bottom, BodyPartZ.Center, new[] {Attribute.Grasping});
				BodyPart rightHand = new BodyPart("Right Hand", rightArm, Slot.Hand, BodyPartX.Right, BodyPartY.Bottom, BodyPartZ.Center, new[] {Attribute.Grasping});
				BodyPart leftLeg = new BodyPart("Left Leg", abdomen, Slot.Legs, BodyPartX.Left, BodyPartY.Bottom, BodyPartZ.Center, new[] {Attribute.Limb, Attribute.Walking});
				BodyPart rightLeg = new BodyPart("Right Leg", abdomen, Slot.Legs, BodyPartX.Right, BodyPartY.Bottom, BodyPartZ.Center, new[] {Attribute.Limb, Attribute.Walking});
				BodyPart leftFoot = new BodyPart("Left Foot", leftLeg, Slot.Feet, BodyPartX.Left, BodyPartY.Bottom, BodyPartZ.Center, new[] {Attribute.Walking});
				BodyPart rightFoot = new BodyPart("Right Foot", leftLeg, Slot.Feet, BodyPartX.Right, BodyPartY.Bottom, BodyPartZ.Center, new[] {Attribute.Walking});
				bodyParts = new List<BodyPart> {head, neck, torso, abdomen, leftArm, rightArm, leftHand, rightHand, leftLeg, rightLeg, leftFoot, rightFoot};
				break;
			case BodyType.Quadruped:
				bodyParts = new List<BodyPart>();
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(bodyType), bodyType, null);
		}

		return bodyParts;
	}

	public IEnumerator<BodyPart> GetEnumerator() => bodyParts.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public enum BodyType {
	Humanoid,
	Quadruped
}