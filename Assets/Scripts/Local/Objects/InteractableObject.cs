using UnityEngine;

public abstract class InteractableObject : EntityObject {

	public abstract Interactable Interactable { get; }
	public override Entity Entity => Interactable;
	protected bool correctPosition;

	protected virtual Vector3 TargetPosition => Interactable.WorldPosition;

	protected virtual void UpdatePosition() {
		if (Interactable == null) Destroy();
		correctPosition = (transform.position - TargetPosition).sqrMagnitude < 0.002f;
		transform.position = correctPosition ? TargetPosition : Vector3.Lerp(transform.position, TargetPosition, 0.1f);
	}
}