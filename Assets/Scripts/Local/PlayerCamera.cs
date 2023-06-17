using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerCamera : MonoBehaviour {
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private float heightOffset = 0.5f;

    [SerializeField] private float zoomSpeed = 4f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 15f;
    [SerializeField] private float zoom = 10f;
    private Transform target;

    [SerializeField] public float rotateSpeed = 5f;
    private Vector3 startingMousePosition;
    private Vector2 rotation;

    private void Update() {
        if (!EventSystem.current.IsPointerOverGameObject()) {
            if (target == null) target = ObjectManager.playerCharacterObject?.transform;
            zoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
            zoom = Mathf.Clamp(zoom, minZoom, maxZoom);

            if (Input.GetMouseButtonDown(2)) {
                startingMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButton(2)) {
                Vector2 mouseOffset = Input.mousePosition - startingMousePosition;
                startingMousePosition = Input.mousePosition;

                rotation += mouseOffset * rotateSpeed * Time.deltaTime;
                rotation.y = Mathf.Clamp(rotation.y, -35, 45);
            }
        }
    }

    private void LateUpdate() {
        if (target == null) return;
        var targetPosition = target.position + Vector3.up * heightOffset;
        transform.position = targetPosition - positionOffset * zoom;
        transform.LookAt(targetPosition);

        transform.RotateAround(target.position, Vector3.up, rotation.x);
        transform.RotateAround(target.position, transform.right, -rotation.y);
    }
}