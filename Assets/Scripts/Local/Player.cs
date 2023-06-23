using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour {
    private static Character character;

    private const float TurnDuration = 1f / 4f; //Duration of a turn in seconds
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
        }
        else {
            if (character.IsMoving) {
                turnTimer -= Time.deltaTime;
                if (turnTimer <= 0 && !paused) {
                    ObjectManager.TakeTurn();
                    turnTimer = TurnDuration;
                }
            }
            else {
                turnTimer = 0;

                var leftClick = Input.GetMouseButtonDown(0);
                var rightClick = Input.GetMouseButtonDown(1);

                if (leftClick || rightClick) {
                    if (interactionsMenu != null && !RectTransformUtility.RectangleContainsScreenPoint(interactionsMenu.GetComponent<RectTransform>(), Input.mousePosition)) {
                        ClearInteractionMenu();
                    }
                    else if (!EventSystem.current.IsPointerOverGameObject()) {
                        var hits = Physics.RaycastAll(Camera.ScreenPointToRay(Input.mousePosition))
                            .Where(h => {
                                var entityObject = h.transform.GetComponent<EntityObject>();
                                return entityObject != null && entityObject.Entity.seen;
                            }).ToArray();
                        if (hits.Length > 0) {
                            var hit = hits[0];
                            var entity = hit.transform.GetComponent<EntityObject>().Entity;
                            var interactable = entity as Interactable;
                            if (interactable == null) {
                                if (leftClick) character.RequestPathToPosition(NodeGrid.GetCoordFromWorldPos(hit.point + 0.05f * hit.normal));
                            }
                            else {
                                if (rightClick) {
                                    DisplayInteractable(interactable);
                                }
                                else {
                                    interactable.MoveTo(character);
                                }
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

    private static void MoveTowards(Coord direction) {
        var rotation = new QuaternionInt(0, Mathf.RoundToInt(Camera.transform.eulerAngles.y / 45f), 0);
        direction = rotation * direction;
        var node = GetValidNode(direction);
        if (node != null) {
            character.RequestPathToPosition(node.position);
            ObjectManager.TakeTurn();
        }
    }

    private static Node GetValidNode(Coord direction) {
        for (var i = 0; i < 3; i++) {
            direction.y = i == 0 ? 0 : (i == 1 ? 1 : -1);
            var targetNode = NodeGrid.GetNeighbor(character.position, direction);
            if (targetNode != null) return targetNode;
        }

        return null;
    }

    public static void DisplayInteractable(Interactable entity) {
        DisplayInteractions(entity.Name, entity.GetInteractions(character));
    }

    public static void DisplayInteractions(string title, ICollection<Interaction> interactions) {
        if (interactions == null || interactions.Count == 0) return;

        var frame = Instantiate(GameManager.PrefabManager.buttonsFramePrefab, canvasTransform);
        frame.transform.position = Input.mousePosition;
        frame.GetComponentInChildren<Text>().text = title;
        var parent = frame.buttonsParent.transform;

        foreach (var interaction in interactions) {
            var button = Instantiate(GameManager.PrefabManager.buttonPrefab, parent);
            button.GetComponentInChildren<Text>().text = interaction.name;
            button.onClick.AddListener(() => interaction.action.Invoke());
            button.onClick.AddListener(() => Destroy(frame.gameObject));
            if (interaction.skipTurn) button.onClick.AddListener(ObjectManager.TakeTurn);
        }

        frame.SetSize(GameManager.PrefabManager.buttonPrefab.GetComponent<RectTransform>().sizeDelta.y + 1);
        interactionsMenu = frame.gameObject;
    }

    private static void ClearInteractionMenu() {
        if (interactionsMenu != null) {
            Destroy(interactionsMenu);
            interactionsMenu = null;
        }
    }
}