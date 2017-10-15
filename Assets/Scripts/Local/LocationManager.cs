using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class LocationManager : MonoBehaviour {
	private Location location;

	[SerializeField] private NoiseSettings locationNoiseSettings;

	public void LoadLocation(Location newLocation) {
		location = newLocation;
		CreateGround();
		CreateBuildings();
		if (ObjectManager.playerCharacter == null) SpawnPlayer();
	}

	private void SpawnPlayer() {
		int x = Random.Range(5, 95);
		int z = Random.Range(5, 95);
		location.characters.Add(new Character(new Coord(x, location.heightMap[x, z], z), true));
	}

	private void CreateGround() {
		location.heightMap = location.map.settings.GenerateLocationHeightMap(location, locationNoiseSettings);
		for (int x = 0; x < location.size; x++) {
			for (int z = 0; z < location.size; z++) {
				int height = location.heightMap[x, z];
				location.walls.Add(new Wall(new Coord(x, height, z), Coord.Down, WallType.Grass));
				if (x - 1 > 0 && location.heightMap[x - 1, z] < height) location.walls.Add(new Wall(new Coord(x, height - 1, z), Coord.Left, WallType.Grass));
				if (x + 1 < location.size && location.heightMap[x + 1, z] < height) location.walls.Add(new Wall(new Coord(x, height - 1, z), Coord.Right, WallType.Grass));
				if (z - 1 > 0 && location.heightMap[x, z - 1] < height) location.walls.Add(new Wall(new Coord(x, height - 1, z), Coord.Back, WallType.Grass));
				if (z + 1 < location.size && location.heightMap[x, z + 1] < height) location.walls.Add(new Wall(new Coord(x, height - 1, z), Coord.Forward, WallType.Grass));

				for (int i = 1; i < height; i++) {
					location.SetTileFree(x, i - 1, z, false);
				}
			}
		}
	}

	private void CreateBuildings() {
		for (int i = 0; i < location.buildingsAmount; i++) {
			BuildBlueprint(GameController.Blueprints.RandomItem());
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
			int bottomLeftX = Random.Range(0, location.size - (rotation * size).x.Abs());
			int bottomLeftZ = Random.Range(0, location.size - (rotation * size).z.Abs());
			Coord bottomLeft = new Coord(bottomLeftX, location.heightMap[bottomLeftX, bottomLeftZ], bottomLeftZ);

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