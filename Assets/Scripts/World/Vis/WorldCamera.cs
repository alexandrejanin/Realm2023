using UnityEngine;

public class WorldCamera : MonoBehaviour {
    [SerializeField] private int mouseButtonPan = 0;
    [SerializeField] private float panSensitivity = 1f;

    [SerializeField] private float zoomSensitivity = 10;

    private Vector3 targetPosition;

    private Vector3 panInitialMousePosition;
    private Vector3 panInitialPosition;

    private new Camera camera;

    public Tile HoverTile() =>
        Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out var hit)
            ? GameManager.World.GetTile(
                Mathf.FloorToInt(hit.point.x),
                GameManager.World.height - Mathf.CeilToInt(hit.point.z) - 1
            )
            : null;

    private void Awake() {
        camera = GetComponent<Camera>();
    }

    private void Update() {
        var world = GameManager.World;

        if (world == null)
            return;

        var mouseWheel = -Input.GetAxis("Mouse ScrollWheel");

        targetPosition.y = Mathf.Clamp(targetPosition.y + mouseWheel * zoomSensitivity, (float)world.MaxDimension / 10,
            (float)world.MaxDimension / 2);

        if (Input.GetMouseButtonDown(mouseButtonPan)) {
            panInitialMousePosition = Input.mousePosition;
            panInitialPosition = transform.position;
        }

        if (Input.GetMouseButton(mouseButtonPan)) {
            var mousePosDiff = panInitialMousePosition - Input.mousePosition;
            var cameraPosDiff = panSensitivity * (targetPosition.y / 100) *
                                new Vector3(mousePosDiff.x, 0, mousePosDiff.y);
            targetPosition.x = panInitialPosition.x + cameraPosDiff.x;
            targetPosition.z = panInitialPosition.z + cameraPosDiff.z;
        }

        targetPosition.x = Mathf.Clamp(targetPosition.x, 0, world.width);
        targetPosition.z = Mathf.Clamp(targetPosition.z, 0, world.height);

        transform.position = (transform.position - targetPosition).magnitude > 0.01f
            ? Vector3.Lerp(transform.position, targetPosition, 0.1f)
            : targetPosition;
    }

    public void MoveTo(Vector3 position) {
        targetPosition = position;
    }
}