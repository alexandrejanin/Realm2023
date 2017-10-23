using UnityEngine;
using UnityEngine.UI;

public class WorldCamera : MonoBehaviour {
	[SerializeField] private float zoomSensitivity = 10;
	private float height;

	private const int mouseButtonPan = 2;

	[SerializeField] private float panSensitivity = 0.1f;

	private Vector3 initialMousePosition;
	private Vector3 initialPosition;

	public Vector3 targetPos;

	[SerializeField] private Toggle perspectiveToggle;
	private new Camera camera;

	private void Awake() {
		targetPos = transform.position;
		height = targetPos.y;
		camera = GetComponent<Camera>();
	}

	private void Update() {
		float mouseWheel = -Input.GetAxis("Mouse ScrollWheel");

		height = Mathf.Clamp(height + mouseWheel * zoomSensitivity, GameController.Map.size / 10, GameController.Map.size / 2);
		camera.orthographic = !perspectiveToggle.isOn;
		if (camera.orthographic) camera.orthographicSize = height;

		if (Input.GetMouseButtonDown(mouseButtonPan)) {
			initialMousePosition = Input.mousePosition;
			initialPosition = transform.position;
		}

		if (Input.GetMouseButton(mouseButtonPan)) {
			Vector3 mousePosDiff = initialMousePosition - Input.mousePosition;
			Vector3 cameraPosDiff = new Vector3(mousePosDiff.x, 0, mousePosDiff.y) * panSensitivity * height / 100;
			targetPos.x = initialPosition.x + cameraPosDiff.x;
			targetPos.z = initialPosition.z + cameraPosDiff.z;
		}

		targetPos.x = Mathf.Clamp(targetPos.x, 0, GameController.Map.size);
		targetPos.z = Mathf.Clamp(targetPos.z, 0, GameController.Map.size);
		targetPos.y = height;

		transform.position = (transform.position - targetPos).magnitude > 0.01f
			? Vector3.Lerp(transform.position, targetPos, 0.1f)
			: targetPos;
	}

}