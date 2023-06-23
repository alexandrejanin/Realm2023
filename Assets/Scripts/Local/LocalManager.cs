using UnityEngine;

[System.Serializable]
public class LocalManager {
    private Location location;
    public Location Location => location;

    [SerializeField] private Blueprint[] blueprints;
    public Blueprint[] Blueprints => blueprints;

    private DialogueManager dialogueManager;
    public DialogueManager DialogueManager => dialogueManager;
}