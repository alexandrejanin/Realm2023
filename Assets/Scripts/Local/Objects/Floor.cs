using UnityEngine;
using UnityEngine.Rendering;

public class Floor : MonoBehaviour {
	private GameObject[] gameObjects;

	private bool active;
	private bool playerInside;

	public void SetGameObjects(GameObject[] gameObjects) {
		foreach (GameObject go in gameObjects) {
			go.transform.parent = transform;
		}
		this.gameObjects = gameObjects;
	}

	public void SetActive(bool newState) {
		if (active == newState) return;

		foreach (GameObject o in gameObjects) {
			HideableObject hideableObject = o.GetComponent<HideableObject>();
			hideableObject.manualSeen = newState;
			hideableObject.manualVisible = newState;
			hideableObject.UpdateDisplay();
		}

		active = newState;
	}
}