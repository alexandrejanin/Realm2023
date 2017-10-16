using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameController))]
public class GameControllerEditor : Editor {
	private GameController gameController;

	public override void OnInspectorGUI() {
		gameController = (GameController) target;

		bool refreshed = DrawDefaultInspector();

		if (GUILayout.Button("Load Database")) {
			gameController.LoadDatabase();
		}
		if (GUILayout.Button("Save Database")) {
			gameController.SaveDatabase();
		}

		if (refreshed && gameController.autoUpdate || GUILayout.Button("Generate World")) {
			gameController.GenerateMap();
		}
	}
}