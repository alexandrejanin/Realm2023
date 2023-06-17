using UnityEngine;

public class CharacterObject : InteractableObject {
    private Animator animator;

    [SerializeField] private Character character;

    protected override Interactable Interactable => character;

    protected override void Awake() {
        base.Awake();
        animator = GetComponent<Animator>();
    }

    public void SetCharacter(Character newCharacter) {
        character = newCharacter;
        gameObject.name = character.Name;
    }

    protected void Update() {
        animator.enabled = character.visible;
        animator.SetBool("Moving", !correctPosition || character.IsMoving);

        var horizontalDirection = character.lookDirection;
        horizontalDirection.y = 0;
        if (horizontalDirection != Vector3.zero) transform.forward = horizontalDirection;
        UpdatePosition();
    }
}