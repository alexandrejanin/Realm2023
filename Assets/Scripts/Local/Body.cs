using System;
using System.Collections;
using System.Collections.Generic;

public class Body : IEnumerable<BodyPart> {
    private readonly List<BodyPart> bodyParts;

    public IEnumerable<Equipable> Equipables {
        get {
            var equipables = new List<Equipable>();
            foreach (var bodyPart in bodyParts) {
                if (bodyPart.equipable != null && !equipables.Contains(bodyPart.equipable)) {
                    equipables.Add(bodyPart.equipable);
                }
            }

            return equipables;
        }
    }

    public Body(BodyType bodyType) {
        bodyParts = GetBody(bodyType);

        foreach (var bodyPart in bodyParts) {
            bodyPart.body = this;
        }
    }

    public void RemovePart(BodyPart bodyPart) {
        bodyParts.Remove(bodyPart);
        foreach (var child in bodyPart.children) {
            RemovePart(child);
        }
    }

    private static List<BodyPart> GetBody(BodyType bodyType) {
        List<BodyPart> bodyParts;

        switch (bodyType) {
            case BodyType.Humanoid:
                var torso = new BodyPart("Torso", null, Slot.Torso, BodyPartX.Middle, BodyPartY.Middle,
                    BodyPartZ.Center,
                    new[] { BodyPartAttribute.Breathing, BodyPartAttribute.InternalOrgans, BodyPartAttribute.Vital });
                var abdomen = new BodyPart("Abdomen", torso, Slot.None, BodyPartX.Middle, BodyPartY.Bottom,
                    BodyPartZ.Center, new[] { BodyPartAttribute.InternalOrgans, BodyPartAttribute.Vital });
                var neck = new BodyPart("Neck", torso, Slot.Neck, BodyPartX.Middle, BodyPartY.Top, BodyPartZ.Center,
                    new[] { BodyPartAttribute.Breathing, BodyPartAttribute.InternalOrgans, BodyPartAttribute.Vital });
                var head = new BodyPart("Head", neck, Slot.Head, BodyPartX.Middle, BodyPartY.Top, BodyPartZ.Center,
                    new[] {
                        BodyPartAttribute.Breathing, BodyPartAttribute.InternalOrgans, BodyPartAttribute.Seeing,
                        BodyPartAttribute.Thinking, BodyPartAttribute.Vital
                    });
                var leftArm = new BodyPart("Left Arm", torso, Slot.None, BodyPartX.Left, BodyPartY.Middle,
                    BodyPartZ.Center, new[] { BodyPartAttribute.Limb });
                var leftHand = new BodyPart("Left Hand", leftArm, Slot.Hand, BodyPartX.Left, BodyPartY.Bottom,
                    BodyPartZ.Center, new[] { BodyPartAttribute.Grasping });
                var rightArm = new BodyPart("Right Arm", torso, Slot.None, BodyPartX.Right, BodyPartY.Middle,
                    BodyPartZ.Center, new[] { BodyPartAttribute.Limb });
                var rightHand = new BodyPart("Right Hand", rightArm, Slot.Hand, BodyPartX.Right, BodyPartY.Bottom,
                    BodyPartZ.Center, new[] { BodyPartAttribute.Grasping });
                var leftLeg = new BodyPart("Left Leg", abdomen, Slot.Legs, BodyPartX.Left, BodyPartY.Bottom,
                    BodyPartZ.Center, new[] { BodyPartAttribute.Limb, BodyPartAttribute.Walking });
                var leftFoot = new BodyPart("Left Foot", leftLeg, Slot.Feet, BodyPartX.Left, BodyPartY.Bottom,
                    BodyPartZ.Center, new[] { BodyPartAttribute.Walking });
                var rightLeg = new BodyPart("Right Leg", abdomen, Slot.Legs, BodyPartX.Right, BodyPartY.Bottom,
                    BodyPartZ.Center, new[] { BodyPartAttribute.Limb, BodyPartAttribute.Walking });
                var rightFoot = new BodyPart("Right Foot", leftLeg, Slot.Feet, BodyPartX.Right, BodyPartY.Bottom,
                    BodyPartZ.Center, new[] { BodyPartAttribute.Walking });
                bodyParts = new List<BodyPart> {
                    head, neck, torso, abdomen, leftArm, leftHand, rightArm, rightHand, leftLeg, leftFoot, rightLeg,
                    rightFoot
                };
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