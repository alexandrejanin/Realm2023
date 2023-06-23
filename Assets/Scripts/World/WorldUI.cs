using TMPro;
using UnityEngine;

public class WorldUI : MonoBehaviour {
    public bool simulate;
    public float simulationSpeed;

    [SerializeField] private TMP_InputField seedField;

    private GameManager gameManager;

    private void Awake() {
        gameManager = FindAnyObjectByType<GameManager>();
        RandomSeed();
    }

    public void SetSeed(string seed) {
        gameManager.GenerateWorld(seed);
    }

    public void RandomSeed() {
        const string chars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ123456789";

        var seed = "";

        for (var i = 0; i < 10; i++)
            seed += chars[Random.Range(0, chars.Length)];

        seedField.text = seed;
        SetSeed(seed);
    }
}