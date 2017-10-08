using UnityEngine;

public class DoorObject : InteractableObject {
	[SerializeField] private Transform target;
	[SerializeField] private Vector3 closedPosition;
	[SerializeField] private Vector3 closedRotation;
	[SerializeField] private Vector3 openPosition;
	[SerializeField] private Vector3 openRotation;

	public Door door;
	public override Interactable Interactable => door;

	protected void Update() {
		if (!door.visible) return;

		Vector3 targetPosition = door.open ? openPosition : closedPosition;

		target.localPosition = (target.localPosition - targetPosition).sqrMagnitude > 0.01f ? Vector3.Lerp(target.localPosition, targetPosition, 0.1f) : targetPosition;

		Vector3 targetRotation = door.open ? openRotation : closedRotation;

		target.localEulerAngles = (target.localEulerAngles - targetRotation).sqrMagnitude > 0.01f
			? Vector3.Lerp(target.localEulerAngles, targetRotation, 0.1f)
			: targetRotation;
	}
}