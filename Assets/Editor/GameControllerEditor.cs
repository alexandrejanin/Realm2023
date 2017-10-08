using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameController))]
public class GameControllerEditor : Editor {
	private GameController gameController;

	public override void OnInspectorGUI() {
		gameController = (GameController) target;

		bool refreshed = DrawDefaultInspector();

		if (GUILayout.Button("Refresh Database")) {
			gameController.LoadDatabase();
		}

		if (refreshed && gameController.autoUpdate || GUILayout.Button("Generate World")) {
			gameController.GenerateMap();
		}
	}
}