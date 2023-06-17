﻿using System;
using UnityEngine;

// ReSharper disable UnassignedField.Global

#pragma warning disable 0649

[Serializable]
public class Race {
    public string collectiveName, individualName, adjective;

    [Range(0, 1)] public float expansionism, hostility;

    [SerializeField] private FloatRange height, temp, humidity;

    [SerializeField] private string[] placeNames, maleFirstNames, femaleFirstNames, lastNames;
    [SerializeField] private IntRange placeNameLength, firstNameLength, lastNameLength;

    public Color skinColor;

    public bool IsValidTile(Tile tile) => !tile.IsWater && height.Contains(tile.height) && temp.Contains(tile.temp) && humidity.Contains(tile.humidity);

    public float GetTileCompatibility(Tile tile) => 1 - ((height.Average - tile.height).Abs() + (temp.Average - tile.temp).Abs() + (humidity.Average - tile.humidity).Abs()) / 3;

    public string GetPlaceName() {
        var placeName = "";
        var length = placeNameLength.Random;
        for (var i = 0; i < length; i++) {
            placeName += placeNames[GameController.Random.Next(0, placeNames.Length)];
        }

        return placeName.Capitalize();
    }

    public string GetFirstName(bool isFemale) {
        var firstName = "";
        var length = firstNameLength.Random;
        var names = isFemale ? femaleFirstNames : maleFirstNames;
        for (var i = 0; i < length; i++) {
            firstName += names[GameController.Random.Next(0, names.Length)];
        }

        return firstName.Capitalize();
    }

    public string GetLastName() {
        var lastName = "";
        var length = lastNameLength.Random;
        for (var i = 0; i < length; i++) {
            lastName += lastNames[GameController.Random.Next(0, lastNames.Length)];
        }

        return lastName.Capitalize();
    }

    public override string ToString() => collectiveName.Capitalize();
}