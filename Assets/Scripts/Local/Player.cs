using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour {
	private static Player instance;
	[SerializeField] private ButtonsFrame framePrefab;
	[SerializeField] private Button buttonPrefab;
	private static Character character;

	private const float turnsPerSecond = 4;
	public const float turnDuration = 1 / turnsPerSecond; //Duration of a turn in seconds
	private static float turnTimer;
	private static bool paused;

	private static GameObject interactionsMenu;

	[SerializeField] private LayerMask raycastLayers;

	private void Awake() {
		instance = this;
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			paused = !paused;
		}

		if (character == null || !character.isPlayer) {
			character = ObjectManager.playerCharacter;
		} else {
			if (character.Path != null) {
				turnTimer -= Time.deltaTime;
				if (turnTimer <= 0 && !paused) {
					TakeTurn();
					turnTimer = turnDuration;
				}
			} else {
				turnTimer = 0;
				if (Input.GetMouseButtonDown(0)) {
					if (interactionsMenu != null && !RectTransformUtility.RectangleContainsScreenPoint(interactionsMenu.GetComponent<RectTransform>(), Input.mousePosition)) {
						ClearInteractionMenu();
					} else if (!EventSystem.current.IsPointerOverGameObject()) {
						RaycastHit hit;
						if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, raycastLayers)) {
							Interactable interactable = hit.transform?.GetComponent<InteractableObject>()?.Interactable;
							if (interactable != null) {
								interactable.MoveTo(character);
							} else {
								character.RequestPathToPos(NodeGrid.GetCoordFromWorldPos(hit.point + hit.normal * 0.2f));
							}
						}
					}
				}

				if (Input.GetMouseButtonDown(1)) {
					ClearInteractionMenu();
					RaycastHit hit;
					if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100, raycastLayers)) {
						Interactable interactable = hit.transform.GetComponent<InteractableObject>()?.Interactable;
						if (interactable != null) {
							DisplayInteractable(interactable);
						}
					}
				}

				if (Input.GetKeyDown(KeyCode.Keypad5)) {
					character.Path = null;
					TakeTurn();
				}

				if (Input.GetKeyDown(KeyCode.Keypad1)) {
					MoveTowards(new Coord(-1, 0, -1));
				}
				if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.DownArrow)) {
					MoveTowards(new Coord(0, 0, -1));
				}
				if (Input.GetKeyDown(KeyCode.Keypad3)) {
					MoveTowards(new Coord(1, 0, -1));
				}
				if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.LeftArrow)) {
					MoveTowards(new Coord(-1, 0, 0));
				}
				if (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.RightArrow)) {
					MoveTowards(new Coord(1, 0, 0));
				}
				if (Input.GetKeyDown(KeyCode.Keypad7)) {
					MoveTowards(new Coord(-1, 0, 1));
				}
				if (Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKeyDown(KeyCode.UpArrow)) {
					MoveTowards(new Coord(0, 0, 1));
				}
				if (Input.GetKeyDown(KeyCode.Keypad9)) {
					MoveTowards(new Coord(1, 0, 1));
				}
			}
		}
	}

	private static void TakeTurn() {
		ObjectManager.TakeTurn();
	}

	private static void MoveTowards(Coord direction) {
		Quaternion rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
		direction = rotation * direction;

		Node validNode = GetValidNode(direction);
		if (validNode != null) {
			character.Path = new[] {validNode.position};
			TakeTurn();
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

	public static void DisplayInteractable(Interactable interactable) {
		DisplayInteractions(interactable.Name, interactable.GetInteractions(character));
	}

	public static void DisplayInteractions(string title, ICollection<Interaction> interactions) {
		if (interactions.Count == 0) return;

		Vector3 offset = Vector3.zero;
		ButtonsFrame frame = Instantiate(instance.framePrefab, Input.mousePosition, Quaternion.identity, GameObject.Find("Canvas").transform);
		frame.GetComponentInChildren<Text>().text = title;
		float offsetPerButton = instance.buttonPrefab.GetComponent<RectTransform>().sizeDelta.y + 1;

		foreach (Interaction interaction in interactions) {
			Button button = Instantiate(instance.buttonPrefab, Input.mousePosition + offset + instance.buttonPrefab.transform.position, instance.buttonPrefab.transform.rotation,
				frame.GetComponentInChildren<LayoutGroup>().transform);
			button.GetComponentInChildren<Text>().text = interaction.name;
			button.onClick.AddListener(() => interaction.action.Invoke());
			button.onClick.AddListener(() => Destroy(frame.gameObject));
			offset += new Vector3(0, -offsetPerButton, 0);
		}

		frame.Size();
		interactionsMenu = frame.gameObject;
	}

	private static void ClearInteractionMenu() {
		if (interactionsMenu != null) {
			Destroy(interactionsMenu);
			interactionsMenu = null;
		}
	}
}