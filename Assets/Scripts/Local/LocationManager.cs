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
        var x = GameController.Random.Next(5, 95);
        var z = GameController.Random.Next(5, 95);
        location.characters.Add(new Character(location, new Coord(x, location.heightMap[x, z], z), true));
    }

    private void CreateGround() {
        location.heightMap = GameController.Map.settings.GenerateLocationHeightMap(location, locationNoiseSettings);
        var wallType = location.Climate.wallType;
        for (var x = 0; x < location.size; x++) {
            for (var z = 0; z < location.size; z++) {
                var height = location.heightMap[x, z];
                location.AddWall(new Wall(location, new Coord(x, height, z), Coord.Down, wallType));
                if (x - 1 > 0 && location.heightMap[x - 1, z] < height) location.AddWall(new Wall(location, new Coord(x, height - 1, z), Coord.Left, wallType));
                if (x + 1 < location.size && location.heightMap[x + 1, z] < height) location.AddWall(new Wall(location, new Coord(x, height - 1, z), Coord.Right, wallType));
                if (z - 1 > 0 && location.heightMap[x, z - 1] < height) location.AddWall(new Wall(location, new Coord(x, height - 1, z), Coord.Back, wallType));
                if (z + 1 < location.size && location.heightMap[x, z + 1] < height) location.AddWall(new Wall(location, new Coord(x, height - 1, z), Coord.Forward, wallType));

                for (var i = 1; i < height; i++) {
                    location.SetTileFree(x, i - 1, z, false);
                }
            }
        }
    }

    private void CreateBuildings() {
        for (var i = 0; i < location.buildingsAmount; i++) {
            BuildBlueprint(GameController.Blueprints.RandomItem());
        }
    }

    private void BuildBlueprint(Blueprint blueprint) {
        var yRotation = GameController.Random.Next(0, 4) * 2;
        var rotation = new QuaternionInt(0, yRotation, 0);
        var size = blueprint.RandomSize();

        var offset = Blueprint.RotationOffset(yRotation);

        var validPosition = false;
        var tries = 0;

        var bottomLeft = new Coord();

        while (!validPosition && tries < 1000) {
            var bottomLeftX = GameController.Random.Next(0, location.size - (rotation * size).x.Abs());
            var bottomLeftZ = GameController.Random.Next(0, location.size - (rotation * size).z.Abs());
            var floorHeight = location.heightMap[bottomLeftX, bottomLeftZ];
            bottomLeft = new Coord(bottomLeftX, floorHeight, bottomLeftZ);

            validPosition = true;

            for (var x = -1; x <= size.x; x++) {
                for (var y = 0; y < size.y; y++) {
                    for (var z = -1; z <= size.z; z++) {
                        var tileCoord = new Coord(x, y, z);
                        var tilePos = bottomLeft + rotation * tileCoord + offset;

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
        }
        else {
            Debug.Log($"Could not find valid position for {blueprint}");
        }
    }
}