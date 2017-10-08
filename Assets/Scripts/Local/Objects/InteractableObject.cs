using UnityEngine;

public abstract class InteractableObject : HideableObject {

	public abstract Interactable Interactable { get; }
	protected override Entity Entity => Interactable;
	protected bool correctPosition;

	protected override void Awake() {
		ObjectManager.InteractableObjects.Add(this);
		base.Awake();
	}

	protected void UpdateStatus() {
		if (Interactable == null) Destroy();

		Vector3 targetPosition = Interactable.WorldPosition;
		correctPosition = (transform.position - targetPosition).sqrMagnitude < 0.001f;
		transform.position = correctPosition ? targetPosition : Vector3.Lerp(transform.position, targetPosition, 0.1f);
	}

	protected void Destroy() {
		ObjectManager.InteractableObjects.Remove(this);
		Destroy(gameObject);
	}
}