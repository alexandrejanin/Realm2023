using UnityEngine;

[System.Serializable]
public class Blueprint {
    [SerializeField] private string name;

    [SerializeField] private Coord minSize, maxSize;

    [SerializeField] private WallType wallType;

    public Coord RandomSize() => new(
        Random.Range(minSize.x, maxSize.x + 1),
        Random.Range(minSize.y, maxSize.y + 1),
        Random.Range(minSize.z, maxSize.z + 1)
    );

    public static Coord RotationOffset(int yRotation) {
        yRotation %= 4;
        switch (yRotation) {
            case 1:
                return new Coord(0, 0, -1);
            case 2:
                return new Coord(-1, 0, -1);
            case 3:
                return new Coord(-1, 0, 0);
            default:
                return Coord.Zero;
        }
    }

    public void GenerateBuilding(Location location, Coord bottomLeft, Coord size, QuaternionInt rotation) {
        var maxX = size.x - 1;
        var maxY = size.y - 1;
        var maxZ = size.z - 1;

        var rotationOffset = RotationOffset(rotation.y);

        var roofY = Mathf.CeilToInt((float)size.z / 2);
        var middle = (float)maxZ / 2;

        var doorPos = new Coord(Random.Range(1, maxX - 1), 0, 0);
        var itemPos = Coord.RandomRange(Coord.Zero, new Coord(maxX, maxY, maxZ));
        var charPos = Coord.RandomRange(Coord.Zero, new Coord(maxX, maxY, maxZ));

        var left = rotation * Coord.Left;
        var right = rotation * Coord.Right;
        //Coord up = rotation * Coord.up;
        var down = rotation * Coord.Down;
        var back = rotation * Coord.Back;
        var forward = rotation * Coord.Forward;

        for (var y = 0; y < size.y + roofY; y++) {
            var lightCoord = new Coord(Random.Range(0, size.x), y, Random.Range(0, size.z));
            for (var x = 0; x < size.x; x++) {
                for (var z = 0; z < size.z; z++) {
                    var localCoord = new Coord(x, y, z);

                    var worldCoord = bottomLeft + rotation * localCoord + rotationOffset;

                    location.SetTileFree(worldCoord, false);

                    if (localCoord == itemPos) location.items.Add(new Equipable(location, worldCoord));
                    if (localCoord == charPos) location.characters.Add(new Character(location, worldCoord, GameManager.Database.RandomRace(), Utility.RandomBool));

                    if (y <= size.y) {
                        //Floor
                        var isStair = y > 0 && x == maxX - (maxY - y) - 1 && z == (y == size.y ? maxZ - 1 : maxZ);

                        if (isStair) {
                            AddWall(location, worldCoord + new Coord(0, -1, 0), y == size.y ? left : right);
                        } else {
                            AddWall(location, worldCoord, down);
                        }
                    }

                    if (y < size.y) {
                        //Walls
                        if (localCoord == lightCoord) {
                            //Object.Instantiate(lightPrefab, worldPos, rotation, buildingParent);
                        }

                        if (localCoord == doorPos) {
                            location.AddWall(new Doorway(location, worldCoord, back, wallType));
                        } else {
                            if (x == 0) {
                                AddWall(location, worldCoord, left);
                            }

                            if (x == maxX) {
                                AddWall(location, worldCoord, right);
                            }

                            if (z == 0) {
                                AddWall(location, worldCoord, back);
                            }

                            if (z == maxZ) {
                                AddWall(location, worldCoord, forward);
                            }
                        }
                    } else {
                        //Roof
                        var isRoof = z == y - size.y || maxZ - z == y - size.y;
                        var isEdge = x == 0 || x == maxX;

                        if (isEdge) {
                            if (isRoof && z != middle) {
                                AddWall(location, worldCoord, x == 0 ? left : right);
                            } else if (y - size.y < z && y - size.y < maxZ - z) {
                                AddWall(location, worldCoord, x == 0 ? left : right);
                            }
                        }

                        /*if (isRoof) {
                            float angle;
                            float height;
                            float scale;

                            if (z < middle) {
                                angle = -45;
                                height = 0.5f;
                                scale = 1.42f;
                                NodeGrid.BlockPassage(worldCoord, rotation * new Coord(0, 0, 1));
                            } else if (z > middle) {
                                angle = 45;
                                height = 0.5f;
                                scale = 1.42f;
                                NodeGrid.BlockPassage(worldCoord, rotation * new Coord(0, 0, -1));
                            } else {
                                angle = 0;
                                height = 0;
                                scale = 1;
                                NodeGrid.BlockPassage(worldCoord + new Coord(0, 1, 0), new Coord(0, 1, 0));
                            }
                            GameObject roof = TownManager.CreateTile(parent, roofTile, worldPos, rotation, new Vector3(0, height, 0), new Vector3(angle, 0, 0), new Vector3(1, 1, scale));
                            blocks[y].Add(roof);
                        }*/
                    }
                }
            }
        }
    }

    private void AddWall(Location location, Coord position, Coord direction) {
        location.AddWall(new Wall(location, position, direction, wallType));
    }

    public override string ToString() => name;
}
