using UnityEngine;

public class BuildingFloor : MonoBehaviour {
    private GameObject[] gameObjects;

    private bool active;
    private bool playerInside;

    public void SetGameObjects(GameObject[] gameObjects) {
        foreach (var go in gameObjects) {
            go.transform.parent = transform;
        }

        this.gameObjects = gameObjects;
    }

    public void SetActive(bool newState) {
        if (active == newState) return;

        foreach (var o in gameObjects) {
            var entityObject = o.GetComponent<EntityObject>();
            entityObject.manualSeen = newState;
            entityObject.manualVisible = newState;
            entityObject.UpdateDisplay();
        }

        active = newState;
    }
}