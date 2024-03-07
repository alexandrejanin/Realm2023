using UnityEngine;

public class DoorObject : InteractableObject {
    [SerializeField] private Vector3 closedPosition;
    [SerializeField] private Vector3 closedRotation;
    [SerializeField] private Vector3 openPosition;
    [SerializeField] private Vector3 openRotation;

    public Door door;
    protected override Interactable Interactable => door;

    protected void Update() {
        if (door.visible) {
            var targetPosition = door.open ? openPosition : closedPosition;

            transform.localPosition = (transform.localPosition - targetPosition).sqrMagnitude > 0.01f
                ? Vector3.Lerp(transform.localPosition, targetPosition, 0.1f)
                : targetPosition;

            var targetRotation = door.open ? openRotation : closedRotation;

            transform.localEulerAngles = (transform.localEulerAngles - targetRotation).sqrMagnitude > 0.01f
                ? Vector3.Lerp(transform.localEulerAngles, targetRotation, 0.1f)
                : targetRotation;
        }
    }
}