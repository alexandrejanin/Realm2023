using UnityEngine;

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
		int x = GameController.Random.Next(5, 95);
		int z = GameController.Random.Next(5, 95);
		location.characters.Add(new Character(new Coord(x, location.heightMap[x, z], z), true));
	}

	private void CreateGround() {
		location.heightMap = GameController.Map.settings.GenerateLocationHeightMap(location, locationNoiseSettings);
		for (int x = 0; x < location.size; x++) {
			for (int z = 0; z < location.size; z++) {
				int height = location.heightMap[x, z];
				location.AddWall(new Wall(new Coord(x, height, z), Coord.Down, WallType.Grass));
				if (x - 1 > 0 && location.heightMap[x - 1, z] < height) location.AddWall(new Wall(new Coord(x, height - 1, z), Coord.Left, WallType.Grass));
				if (x + 1 < location.size && location.heightMap[x + 1, z] < height) location.AddWall(new Wall(new Coord(x, height - 1, z), Coord.Right, WallType.Grass));
				if (z - 1 > 0 && location.heightMap[x, z - 1] < height) location.AddWall(new Wall(new Coord(x, height - 1, z), Coord.Back, WallType.Grass));
				if (z + 1 < location.size && location.heightMap[x, z + 1] < height) location.AddWall(new Wall(new Coord(x, height - 1, z), Coord.Forward, WallType.Grass));

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
		int yRotation = GameController.Random.Next(0, 4) * 2;
		QuaternionInt rotation = new QuaternionInt(0, yRotation, 0);
		Coord size = blueprint.RandomSize();

		Coord offset = Blueprint.RotationOffset(yRotation);

		bool validPosition = false;
		int tries = 0;

		Coord bottomLeft = new Coord();

		while (!validPosition && tries < 100) {
			int bottomLeftX = GameController.Random.Next(0, location.size - (rotation * size).x.Abs());
			int bottomLeftZ = GameController.Random.Next(0, location.size - (rotation * size).z.Abs());
			int floorHeight = location.heightMap[bottomLeftX, bottomLeftZ];
			bottomLeft = new Coord(bottomLeftX, floorHeight, bottomLeftZ);

			validPosition = true;

			for (int x = -1; x <= size.x; x++) {
				for (int y = 0; y < size.y; y++) {
					for (int z = -1; z <= size.z; z++) {
						Coord tileCoord = new Coord(x, y, z);
						Coord tilePos = bottomLeft + rotation * tileCoord + offset;

						if (!(location.GetTileFree(tilePos) && location.GetHeight(tilePos) == floorHeight)) validPosition = false;
						if (!validPosition) break;
					}
					if (!validPosition) break;
				}
				if (!validPosition) break;
			}

			tries++;
		}

		if (validPosition) {
			blueprint.GenerateBuilding(location, bottomLeft, size, rotation);
		} else {
			Debug.Log($"Could not find valid position for {blueprint}");
		}
	}
}