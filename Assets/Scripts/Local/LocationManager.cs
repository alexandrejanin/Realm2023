using UnityEngine;
using UnityEngine.SceneManagement;

public class LocationManager {
    private Location location;
    public Location CurrentLocation => location;

    private readonly NoiseParameters locationNoiseSettings = new() {
        octaves = 2,
        persistance = 0.5f,
        lacunarity = 2,
        scale = 50
    };

    public void LoadLocation(Location newLocation) {
        location = newLocation;
        NodeGrid.CreateGrid(location);
        CreateGround();
        CreateBuildings();

        if (ObjectManager.playerCharacter == null)
            SpawnPlayer();

        ObjectManager.RefreshObjects();
    }

    private void SpawnPlayer() {
        var x = GameManager.Random.Next(5, 95);
        var z = GameManager.Random.Next(5, 95);
        location.characters.Add(new Character(location, new Coord(x, location.heightMap[x, z], z), true));
    }

    private void CreateGround() {
        location.heightMap = GenerateLocationHeightMap(locationNoiseSettings);

        var wallType = location.Climate.wallType;

        for (var x = 0; x < location.size; x++) {
            for (var z = 0; z < location.size; z++) {
                var height = location.heightMap[x, z];

                location.AddWall(new Wall(location, new Coord(x, height, z), Coord.Down, wallType));

                if (x - 1 > 0 && location.heightMap[x - 1, z] < height)
                    location.AddWall(new Wall(location, new Coord(x, height - 1, z), Coord.Left, wallType));
                if (x + 1 < location.size && location.heightMap[x + 1, z] < height)
                    location.AddWall(new Wall(location, new Coord(x, height - 1, z), Coord.Right, wallType));
                if (z - 1 > 0 && location.heightMap[x, z - 1] < height)
                    location.AddWall(new Wall(location, new Coord(x, height - 1, z), Coord.Back, wallType));
                if (z + 1 < location.size && location.heightMap[x, z + 1] < height)
                    location.AddWall(new Wall(location, new Coord(x, height - 1, z), Coord.Forward, wallType));

                for (var i = 1; i < height; i++) {
                    location.SetTileFree(x, i - 1, z, false);
                }
            }
        }
    }

    public int[,] GenerateLocationHeightMap(NoiseParameters settings) {
        var floatMap = settings.Generate(location.size, location.size);

        var heightMap = new int[location.size, location.size];
        for (var x = 0;
             x < location.size;
             x++) {
            for (var z = 0; z < location.size; z++) {
                heightMap[x, z] = (int)(floatMap[x, z] * location.steepness);
            }
        }

        return heightMap;
    }

    private void CreateBuildings() {
        for (var i = 0; i < location.buildingsAmount; i++) {
            BuildBlueprint(
                GameManager.LocalManager.Blueprints
                    [GameManager.Random.Next(GameManager.LocalManager.Blueprints.Length)]);
        }
    }

    private void BuildBlueprint(Blueprint blueprint) {
        var yRotation = GameManager.Random.Next(0, 4) * 2;
        var rotation = new QuaternionInt(0, yRotation, 0);
        var size = blueprint.RandomSize();

        var offset = Blueprint.RotationOffset(yRotation);

        var validPosition = false;
        var tries = 0;

        var bottomLeft = new Coord();

        while (!validPosition && tries < 1000) {
            var bottomLeftX = GameManager.Random.Next(0, location.size - (rotation * size).x.Abs());
            var bottomLeftZ = GameManager.Random.Next(0, location.size - (rotation * size).z.Abs());
            var floorHeight = location.heightMap[bottomLeftX, bottomLeftZ];
            bottomLeft = new Coord(bottomLeftX, floorHeight, bottomLeftZ);

            validPosition = true;

            for (var x = -1; x <= size.x; x++) {
                for (var y = 0; y < size.y; y++) {
                    for (var z = -1; z <= size.z; z++) {
                        var tileCoord = new Coord(x, y, z);
                        var tilePos = bottomLeft + rotation * tileCoord + offset;

                        if (!(location.GetTileFree(tilePos) && location.GetHeight(tilePos) == floorHeight))
                            validPosition = false;
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