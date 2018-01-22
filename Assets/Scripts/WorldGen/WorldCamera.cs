using UnityEngine;
using UnityEngine.UI;

public class WorldCamera : MonoBehaviour {
	[SerializeField] private float zoomSensitivity = 10;
	private float height;

	private const int MouseButtonPan = 2;

	[SerializeField] private float panSensitivity = 1f;

	private Vector3 initialMousePosition;
	private Vector3 initialPosition;

	[HideInInspector] public Vector3 targetPos;

	[HideInInspector] public bool dragged;

	private void Awake() {
		targetPos = transform.position;
		height = targetPos.y;
	}

	private void Update() {
		float mouseWheel = -Input.GetAxis("Mouse ScrollWheel");

		height = Mathf.Clamp(height + mouseWheel * zoomSensitivity, (float) GameController.Map.size / 10, (float) GameController.Map.size / 2);

		if (Input.GetMouseButtonDown(MouseButtonPan)) {
			initialMousePosition = Input.mousePosition;
			initialPosition = transform.position;
		}

		if (dragged = Input.GetMouseButton(MouseButtonPan)) {
			Vector3 mousePosDiff = initialMousePosition - Input.mousePosition;
			Vector3 cameraPosDiff = panSensitivity * (height / 100) * new Vector3(mousePosDiff.x, 0, mousePosDiff.y);
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