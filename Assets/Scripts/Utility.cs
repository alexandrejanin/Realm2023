using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public static class Utility {

	private static readonly System.Random random = new System.Random();

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

	public static T RandomItem<T>(this IList<T> list, int startRank, int endRank) => list[random.Next(startRank, endRank)];
	public static T RandomItem<T>(this IList<T> list) => list[random.Next(0, list.Count)];
	public static T RandomValue<T>(int startRank = 0, int endRank = int.MaxValue) where T : IConvertible, IFormattable, IComparable => ((T[]) Enum.GetValues(typeof(T))).ToList().RandomItem(startRank, endRank);

	public static int Sign(this int i, SignType signType = SignType.ZeroIsZero) {
		if (i > 0) return 1;
		if (i < 0) return -1;
		return signType == SignType.ZeroIsPositive ? 1 : signType == SignType.ZeroIsNegative ? -1 : 0;
	}

	public enum SignType {
		ZeroIsNegative,
		ZeroIsPositive,
		ZeroIsZero
	}

	public static bool RandomBool => Random.value > 0.5f;
}