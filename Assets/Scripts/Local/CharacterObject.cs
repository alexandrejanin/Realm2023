using UnityEngine;

public class CharacterObject : InteractableObject {
	private Animator animator;

	public Character Character { get; private set; }

	public override Interactable Interactable => Character;

	protected override void Awake() {
		base.Awake();
		animator = GetComponent<Animator>();
	}

	public void SetCharacter(Character character) {
		Character = character;
		gameObject.name = character.Name;
	}

	protected void Update() {
		animator.SetBool("Moving", !correctPosition || Character.Path != null);

		Coord horizontalDirection = Character.lookDirection;
		horizontalDirection.y = 0;
		if (horizontalDirection != Vector3.zero) transform.forward = horizontalDirection;
		UpdatePosition();
	}
}