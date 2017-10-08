using UnityEngine;

public class CharacterObject : InteractableObject {
	private Animator animator;

	public Character Character { get; set; }

	public override Interactable Interactable => Character;

	protected override void Awake() {
		base.Awake();
		animator = GetComponent<Animator>();
	}

	public void SetCharacter(Character character) {
		Character = character;
		gameObject.name = character.Name;
	}

	private void Update() {
		animator.SetBool("Moving", !correctPosition || Character.Path != null);

		Vector3 horizontalDirection = Character.lookDirection;
		horizontalDirection.y = 0;
		transform.forward = horizontalDirection;
		UpdateStatus();
	}
}