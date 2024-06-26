﻿using TMPro;
using UnityEditor.Profiling;
using UnityEngine;
using UnityEngine.UI;

public class WorldUI : MonoBehaviour {
    [SerializeField] private TMP_InputField seedField;
    [SerializeField] private Text tileInfoText;

    [SerializeField] private InputSlider stepsPerSecond, steps;
    [SerializeField] private TMP_Text simButton;
    private float stepDuration;
    private bool simActive;

    private WorldCamera worldCamera;
    private Tile currentTile;

    public void SetSeed(string seed) {
        seedField.text = seed;
        GameManager.GenerateWorld(seed);
    }

    public void SetMapDrawMode(int mode) {
        if (mode < 0)
            Debug.LogWarningFormat("Tried setting negative MapDrawMode ({0})", mode);
        else if (mode > System.Enum.GetNames(typeof(MapDrawMode)).Length)
            Debug.LogWarningFormat("Tried setting incorrect MapDrawMode ({0})", mode);
        else
            GameManager.WorldManager.SetMapDrawMode((MapDrawMode)mode);
    }

    public void RandomSeed(int length) {
        const string chars = "abcdefghijkmnopqrstuvwxyz" +
                             "ABCDEFGHJKLMNPQRSTUVWXYZ" +
                             "123456789";

        var seed = "";
        for (var i = 0; i < length; i++)
            seed += chars[Random.Range(0, chars.Length)];

        SetSeed(seed);
    }

    private void Awake() {
        worldCamera = FindAnyObjectByType<WorldCamera>();
        RandomSeed(10);
    }

    private void Update() {
        var newTile = worldCamera.HoverTile();

        if (newTile != null) {
            if (newTile != currentTile) {
                currentTile = newTile;
                var text = $"x: {currentTile.x} y: {currentTile.y}" +
                           $"\nHeight: {currentTile.elevation:F2}" +
                           $"\nTemp: ({currentTile.temperature:F2})" +
                           $"\nRegion: {currentTile.region}";

                if (currentTile.location != null)
                    text += $"\n{currentTile.location}";

                if (currentTile.location is Town town)
                    text += $"\nPopulation: {town.population}";

                tileInfoText.text = text;
            }

            if (Input.GetMouseButtonDown(0) && currentTile.location != null)
                GameManager.LoadLocation(currentTile.location);
        }

        UpdateSim();
    }

    public void ToggleSim() {
        simActive = !simActive;
    }

    private void UpdateSim() {
        if (!simButton || !steps || !stepsPerSecond)
            return;

        if (steps.Value <= 0)
            simActive = false;

        if (simActive) {
            simButton.text = "Stop";
            stepDuration += Time.deltaTime;
            if (stepDuration > 1f / stepsPerSecond.Value) {
                GameManager.WorldManager.SimulateWorld();
                steps.SetValue(steps.Value - 1);
                stepDuration = 0;
            }
        }
        else {
            simButton.text = "Start";
            stepDuration = 0;
        }
    }
}