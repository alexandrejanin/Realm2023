using System;
using System.Collections.Generic;
using System.Linq;

public static class Utility {

	private static readonly Random DefaultRandom = new Random();

	public static string Capitalize(this string s) => s.First().ToString().ToUpper() + s.Substring(1);

	public static string[] SliceString(string str, int parts) {
		if (str.Length % parts == 0) {
			int chunkSize = str.Length / parts;
			string[] strings = new string[parts];
			for (int i = 0; i < str.Length; i += chunkSize) {
				strings[i / chunkSize] = str.Substring(i, chunkSize);
			}
			return strings;
		}
		UnityEngine.Debug.LogError("Error: string can't be divided into even parts (" + str.Length + " chars, " + parts + " parts");
		return null;
	}

	public static T RandomItem<T>(this IList<T> list, int startRank = 0) => RandomItem(list, DefaultRandom, startRank);
	public static T RandomItem<T>(this IList<T> list, Random random, int startRank = 0) => list[random.Next(startRank, list.Count)];
	public static T RandomValue<T>(int startRank = 0) where T : IConvertible, IFormattable, IComparable => ((T[]) Enum.GetValues(typeof(T))).RandomItem(startRank);

	public static int Abs(this int i) => i < 0 ? -i : i;

	public static int Sign(this int i) {
		if (i > 0) return 1;
		if (i < 0) return -1;
		return 0;
	}

	public static bool RandomBool => UnityEngine.Random.value > 0.5f;
}