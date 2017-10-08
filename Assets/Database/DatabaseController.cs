using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;

public static class DatabaseController {
	private static readonly Regex regex = new Regex(@"\s+");

	public static T[] LoadXML<T>(TextAsset database, string nodeName) where T : Importable {
		IEnumerable<Dictionary<string, string>> dictionaryList = LoadDatabaseAsDictionaries(database, nodeName);
		return dictionaryList.Select(dictionary => (T) Activator.CreateInstance(typeof(T), dictionary)).ToArray();
	}

	private static XmlNodeList LoadDatabase(TextAsset textAsset, string tagName) {
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(textAsset.text);
		XmlNodeList itemList = xmlDocument.GetElementsByTagName(tagName);
		return itemList;
	}

	private static IEnumerable<Dictionary<string, string>> LoadDatabaseAsDictionaries(TextAsset database, string tagName) {
		XmlNodeList itemList = LoadDatabase(database, tagName);

		return (from XmlNode item in itemList
			select item.ChildNodes
			into itemContent
			select itemContent.Cast<XmlNode>().ToDictionary(content => content.Name, content => regex.Replace(content.InnerText, ""))).ToList();
	}
}