using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocationManager : MonoBehaviour {
	[SerializeField] private Blueprint[] blueprints;

	[SerializeField] private int buildingsAmount = 50;

	private Location location;

	public void LoadLocation(Location newLocation) {
		location = newLocation;
		CreateGround();
		CreateBuildings();
		if (ObjectManager.playerCharacter == null) SpawnPlayer();
		ObjectManager.RefreshObjects();
	}

	private void SpawnPlayer() {
		location.characters.Add(new Character(new Coord(Random.Range(5, 95), 0, Random.Range(5, 95)), true));
	}

	private void CreateGround() {
		for (int x = 0; x < location.Size.x; x++) {
			for (int z = 0; z < location.Size.z; z++) {
				location.walls.Add(new Wall(new Coord(x, 0, z), Coord.Down, WallType.Grass));
			}
		}
	}

	private void CreateBuildings() {
		blueprints = blueprints.OrderBy(blueprint => blueprint.weight).ToArray();
		float totalWeight = blueprints.Sum(blueprint => blueprint.weight);
		for (int i = 0; i < buildingsAmount; i++) {
			float rnd = Random.Range(0, totalWeight);
			foreach (Blueprint blueprint in blueprints) {
				if (rnd < blueprint.weight) {
					BuildBlueprint(blueprint);
					break;
				}
				rnd -= blueprint.weight;
			}
		}
	}

	private void BuildBlueprint(Blueprint blueprint) {
		Coord rotationCoord = new Coord(0, Random.Range(0, 4), 0);
		Quaternion rotation = Quaternion.Euler(rotationCoord * 90);
		Coord size = blueprint.RandomSize();

		Coord offset = Blueprint.RotationOffset(rotationCoord.y);

		bool validPosition = false;
		Vector3 center = new Vector3();
		int tries = 0;

		while (!validPosition && tries < 100) {
			Coord bottomLeft = new Coord(Random.Range(0, location.Width - (rotation * size).x), 0, Random.Range(0, location.Length - (rotation * size).z));
			center = bottomLeft + rotation * (Vector3) size / 2;
			validPosition = true;

			for (int x = -1; x <= size.x; x++) {
				for (int y = 0; y < size.y; y++) {
					for (int z = -1; z <= size.z; z++) {
						Coord tileCoord = new Coord(x, y, z);
						Coord tilePos = bottomLeft + rotation * tileCoord + offset;

						if (!location.GetTileFree(tilePos)) validPosition = false;
						if (!validPosition) break;
					}
					if (!validPosition) break;
				}
				if (!validPosition) break;
			}

			tries++;
		}

		if (validPosition) {
			blueprint.GenerateBuilding(location, center, size, rotation);
		} else {
			Debug.Log($"Could not find valid position for {blueprint}");
		}
	}
}