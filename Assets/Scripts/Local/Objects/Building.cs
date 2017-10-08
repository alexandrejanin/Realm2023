using UnityEngine;

public class Building : MonoBehaviour {
	public Floor[] floors;

	public Light[] lights;
	//public Interactable[] interactables;

	public Bounds bounds;

	public bool playerLeft = true;

	private int maxY;
	private bool autoHeight = true;

	private void Update() {
		if (bounds.size.x < 0) {
			bounds.size = new Vector3(-bounds.size.x, bounds.size.y, bounds.size.z);
		}
		if (bounds.size.z < 0) {
			bounds.size = new Vector3(bounds.size.x, bounds.size.y, -bounds.size.z);
		}

		if (bounds.Contains(ObjectManager.playerCharacter.WorldPosition)) {
			if (playerLeft) {
				playerLeft = false;
				foreach (HideableObject hideableObject in GetComponentsInChildren<HideableObject>()) {
					hideableObject.manualMode = true;
				}
			}
			if (Input.GetKeyDown(KeyCode.KeypadMinus)) {
				autoHeight = false;
				maxY--;
			}

			if (Input.GetKeyDown(KeyCode.KeypadPlus)) {
				autoHeight = false;
				maxY++;
			}

			maxY = Mathf.Clamp(maxY, 0, floors.Length - 1);

			if (maxY == ObjectManager.playerCharacter.position.y) autoHeight = true;
			if (autoHeight) {
				maxY = ObjectManager.playerCharacter.position.y;
			}

			for (int i = 0; i < floors.Length; i++) {
				floors[i].SetActive(i <= maxY);
			}

			foreach (Light light in lights) {
				light.enabled = Mathf.FloorToInt(light.transform.position.y) == maxY;
			}
		} else if (!playerLeft) {
			foreach (Floor floor in floors) {
				floor.SetActive(true);
			}

			foreach (HideableObject hideableObject in GetComponentsInChildren<HideableObject>()) {
				hideableObject.manualMode = false;
			}

			foreach (Light light in lights) {
				light.enabled = false;
			}

			autoHeight = true;
			playerLeft = true;
		}
	}
}