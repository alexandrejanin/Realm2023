using UnityEngine;

[System.Serializable]
public class LocalManager {
    private LocationManager locationManager = new();
    public Location CurrentLocation => locationManager.CurrentLocation;

    [SerializeField] private Blueprint[] blueprints;
    public Blueprint[] Blueprints => blueprints;

    private DialogueManager dialogueManager = new();
    public DialogueManager DialogueManager => dialogueManager;

    public void LoadLocation(Location newLocation) {
        locationManager.LoadLocation(newLocation);
    }
}