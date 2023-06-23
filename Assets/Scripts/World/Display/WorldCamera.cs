using UnityEngine;

public class WorldCamera : MonoBehaviour {
    [SerializeField] private float zoomSensitivity = 10;

    private const int MouseButtonPan = 2;

    [SerializeField] private float panSensitivity = 1f;

    private Vector3 initialMousePosition;
    private Vector3 initialPosition;

    public Vector3 targetPos;

    private void Update() {
        var world = GameManager.WorldManager.World;

        if (world == null)
            return;

        var mouseWheel = -Input.GetAxis("Mouse ScrollWheel");

        targetPos.y = Mathf.Clamp(targetPos.y + mouseWheel * zoomSensitivity, (float) world.MaxDimension / 10, (float) world.MaxDimension / 2);

        if (Input.GetMouseButtonDown(MouseButtonPan)) {
            initialMousePosition = Input.mousePosition;
            initialPosition = transform.position;
        }

        if (Input.GetMouseButton(MouseButtonPan)) {
            var mousePosDiff = initialMousePosition - Input.mousePosition;
            var cameraPosDiff = panSensitivity * (targetPos.y / 100) * new Vector3(mousePosDiff.x, 0, mousePosDiff.y);
            targetPos.x = initialPosition.x + cameraPosDiff.x;
            targetPos.z = initialPosition.z + cameraPosDiff.z;
        }

        targetPos.x = Mathf.Clamp(targetPos.x, -world.width / 2f, world.width / 2f);
        targetPos.z = Mathf.Clamp(targetPos.z, -world.height / 2f, world.height / 2f);

        transform.position = (transform.position - targetPos).magnitude > 0.01f
            ? Vector3.Lerp(transform.position, targetPos, 0.1f)
            : targetPos;
    }
}