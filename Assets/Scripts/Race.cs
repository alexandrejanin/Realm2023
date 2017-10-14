using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Race : Importable {
	public readonly string collectiveName;
	public readonly string individualName;
	public readonly string adjective;

	private readonly string[] placeNames;
	private readonly int[] placeNameLength;

	private readonly string[] maleFirstNames;
	private readonly string[] femaleFirstNames;
	private readonly int[] firstNameLength;

	private readonly string[] lastNames;
	private readonly int[] lastNameLength;

	public Race(IDictionary<string, string> d) : base(d) {
		collectiveName = d["CollectiveName"];
		individualName = d["IndividualName"];
		adjective = d["Adjective"];

		placeNames = d["PlaceNamesList"].Split(',');
		placeNameLength = Regex.Matches(d["PlaceNameLength"], @"\d").Cast<Match>().Select(m => int.Parse(m.Value)).ToArray();

		maleFirstNames = d["MaleFirstNamesList"].Split(',');
		femaleFirstNames = d["FemaleFirstNamesList"].Split(',');
		firstNameLength = Regex.Matches(d["FirstNameLength"], @"\d").Cast<Match>().Select(m => int.Parse(m.Value)).ToArray();

		lastNames = d["LastNamesList"].Split(',');
		lastNameLength = Regex.Matches(d["LastNameLength"], @"\d").Cast<Match>().Select(m => int.Parse(m.Value)).ToArray();
	}

	public string GetPlaceName() {
		string placeName = "";
		int length = Random.Range(placeNameLength[0], placeNameLength[1] + 1);
		for (int i = 0; i < length; i++) {
			placeName += placeNames[Random.Range(0, placeNames.Length)];
		}

		return placeName.Capitalize();
	}

	public string GetFirstName(bool isFemale) {
		string firstName = "";
		int length = Random.Range(firstNameLength[0], firstNameLength[1] + 1);
		string[] names = isFemale ? femaleFirstNames : maleFirstNames;
		for (int i = 0; i < length; i++) {
			firstName += names[Random.Range(0, names.Length)];
		}

		return firstName.Capitalize();
	}

	public string GetLastName() {
		string lastName = "";
		int length = Random.Range(lastNameLength[0], lastNameLength[1] + 1);
		for (int i = 0; i < length; i++) {
			lastName += lastNames[Random.Range(0, lastNames.Length)];
		}

		return lastName.Capitalize();
	}

	public float GetTileCompatibility(Tile tile) {
		float heightDiff = Mathf.Abs(0.5f - tile.height);
		float tempDiff = Mathf.Abs(0.5f - tile.temp);

		return heightDiff + tempDiff;
	}

	public override string ToString() => collectiveName.Capitalize();
}