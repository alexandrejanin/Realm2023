using UnityEngine;

// ReSharper disable UnassignedField.Global

#pragma warning disable 0649

[System.Serializable]
public class Race {
	public string collectiveName, individualName, adjective;

	[SerializeField] private string[] placeNames, maleFirstNames, femaleFirstNames, lastNames;
	[SerializeField] private int minPlaceNameLength, maxPlaceNameLength, minFirstNameLength, maxFirstNameLength, minLastNameLength, maxLastNameLength;

	[SerializeField] private float expansionism, hostility;

	public string GetPlaceName() {
		string placeName = "";
		int length = GameController.Random.Next(minPlaceNameLength, maxPlaceNameLength + 1);
		for (int i = 0; i < length; i++) {
			placeName += placeNames[GameController.Random.Next(0, placeNames.Length)];
		}

		return placeName.Capitalize();
	}

	public string GetFirstName(bool isFemale) {
		string firstName = "";
		int length = GameController.Random.Next(minFirstNameLength, maxFirstNameLength + 1);
		string[] names = isFemale ? femaleFirstNames : maleFirstNames;
		for (int i = 0; i < length; i++) {
			firstName += names[GameController.Random.Next(0, names.Length)];
		}

		return firstName.Capitalize();
	}

	public string GetLastName() {
		string lastName = "";
		int length = GameController.Random.Next(minLastNameLength, maxLastNameLength + 1);
		for (int i = 0; i < length; i++) {
			lastName += lastNames[GameController.Random.Next(0, lastNames.Length)];
		}

		return lastName.Capitalize();
	}

	public override string ToString() => collectiveName.Capitalize();
}