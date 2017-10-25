using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour {
	private static Character character;

	private const float turnsPerSecond = 4;
	public const float turnDuration = 1 / turnsPerSecond; //Duration of a turn in seconds
	private static float turnTimer;
	private static bool paused;

	private static GameObject interactionsMenu;

	[SerializeField] private Canvas canvas;
	[SerializeField] private static Transform canvasTransform;

	private static Camera Camera => camera ?? (camera = Camera.main);
	private new static Camera camera;

	private void Awake() {
		canvasTransform = canvas.transform;
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			paused = !paused;
		}

		if (character == null || !character.isPlayer) {
			character = ObjectManager.playerCharacter;
		} else {
			if (character.IsMoving) {
				turnTimer -= Time.deltaTime;
				if (turnTimer <= 0 && !paused) {
					ObjectManager.TakeTurn();
					turnTimer = turnDuration;
				}
			} else {
				turnTimer = 0;

				bool leftClick = Input.GetMouseButtonDown(0);
				bool rightClick = Input.GetMouseButtonDown(1);

				if (leftClick || rightClick) {
					if (interactionsMenu != null && !RectTransformUtility.RectangleContainsScreenPoint(interactionsMenu.GetComponent<RectTransform>(), Input.mousePosition)) {
						ClearInteractionMenu();
					} else if (!EventSystem.current.IsPointerOverGameObject()) {
						RaycastHit[] hits = Physics.RaycastAll(Camera.ScreenPointToRay(Input.mousePosition));
						InteractableObject[] interactableObjects = hits.Select(h => h.transform.GetComponent<InteractableObject>()).ToArray();
						InteractableObject hitInteractableObject = interactableObjects.FirstOrDefault(entityObject => entityObject.Entity.seen);
						if (hitInteractableObject != null) {
							Interactable entity = hitInteractableObject.Interactable;
							if (rightClick) {
								DisplayInteractable(entity);
							} else if (!entity.ValidPosition(character.position)) {
								entity.MoveTo(character);
							}
						}
					}
				}

				if (Input.GetKeyDown(KeyCode.Keypad5)) {
					character.StopPath();
					ObjectManager.TakeTurn();
				}

				if (Input.GetKeyDown(KeyCode.Keypad1)) {
					MoveTowards(Coord.Back + Coord.Left);
				}
				if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.DownArrow)) {
					MoveTowards(Coord.Back);
				}
				if (Input.GetKeyDown(KeyCode.Keypad3)) {
					MoveTowards(Coord.Back + Coord.Right);
				}
				if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.LeftArrow)) {
					MoveTowards(Coord.Left);
				}
				if (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.RightArrow)) {
					MoveTowards(Coord.Right);
				}
				if (Input.GetKeyDown(KeyCode.Keypad7)) {
					MoveTowards(Coord.Forward + Coord.Left);
				}
				if (Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.UpArrow)) {
					MoveTowards(Coord.Forward);
				}
				if (Input.GetKeyDown(KeyCode.Keypad9)) {
					MoveTowards(Coord.Forward + Coord.Right);
				}
			}
		}
	}

	public void MoveTowards(Coord direction) {
		QuaternionInt rotation = new QuaternionInt(0, Mathf.RoundToInt(Camera.transform.eulerAngles.y / 45f), 0);
		direction = rotation * direction;
		Node node = GetValidNode(direction);
		if (node != null) {
			character.RequestPathToPosition(node.position);
			ObjectManager.TakeTurn();
		}
	}

	private static Node GetValidNode(Coord direction) {
		for (int i = 0; i < 3; i++) {
			direction.y = i == 0 ? 0 : (i == 1 ? 1 : -1);
			Node targetNode = NodeGrid.GetNeighbor(character.position, direction);
			if (targetNode != null) return targetNode;
		}
		return null;
	}

	public static void DisplayInteractable(Interactable entity) {
		DisplayInteractions(entity.Name, entity.GetInteractions(character));
	}

	public static void DisplayInteractions(string title, ICollection<Interaction> interactions) {
		if (interactions.Count == 0) return;

		PrefabManager prefabManager = GameController.PrefabManager;

		ButtonsFrame frame = Instantiate(prefabManager.buttonsFramePrefab, Input.mousePosition, Quaternion.identity, canvasTransform);
		frame.GetComponentInChildren<Text>().text = title;
		Transform parent = frame.buttonsParent.transform;

		foreach (Interaction interaction in interactions) {
			Button button = Instantiate(prefabManager.buttonPrefab, parent);
			button.GetComponentInChildren<Text>().text = interaction.name;
			button.onClick.AddListener(() => interaction.action.Invoke());
			button.onClick.AddListener(() => Destroy(frame.gameObject));
			if (interaction.skipTurn) button.onClick.AddListener(ObjectManager.TakeTurn);
		}

		frame.SetSize(prefabManager.buttonPrefab.GetComponent<RectTransform>().sizeDelta.y + 1);
		interactionsMenu = frame.gameObject;
	}

	private static void ClearInteractionMenu() {
		if (interactionsMenu != null) {
			Destroy(interactionsMenu);
			interactionsMenu = null;
		}
	}
}