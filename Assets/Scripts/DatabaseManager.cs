using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DatabaseManager {
    [SerializeField] private Race[] races;
    [SerializeField] private Climate[] climates;

    public Race[] Races {
        get {
            if (races == null || races.Length == 0)
                LoadDatabase();

            return races;
        }
    }

    public Climate[] Climates {
        get {
            if (climates == null || climates.Length == 0)
                LoadDatabase();

            return climates;
        }
    }

    private static string RacesPath =>
        Application.streamingAssetsPath + "/Database/Races/";

    private static string ClimatesPath =>
        Application.streamingAssetsPath + "/Database/Climates/";


    public void LoadDatabase() {
        races = LoadFromDirectory<Race>(RacesPath);
        climates = LoadFromDirectory<Climate>(ClimatesPath);
    }

    public void SaveDatabase() {
        SaveToDirectory(races, RacesPath);
        SaveToDirectory(climates, ClimatesPath);
    }

    private static T[] LoadFromDirectory<T>(string path) => Directory.GetFiles(path, "*.json").Select(file => JsonUtility.FromJson<T>(File.ReadAllText(file))).ToArray();

    private static void SaveToDirectory<T>(IEnumerable<T> database, string path) {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        foreach (var o in database) {
            File.WriteAllText(path + o + ".json", JsonUtility.ToJson(o, true));
        }
    }

    public Race RandomRace() => Races.RandomItem();
}
