using UnityEngine;

[System.Serializable]
public class Blueprint {
	public string name;
	public int weight;

	[SerializeField] private Coord minSize;
	[SerializeField] private Coord maxSize;

	[SerializeField] private WallType wallType;

	public Coord RandomSize() => new Coord(Random.Range(minSize.x, maxSize.x + 1), Random.Range(minSize.y, maxSize.y + 1), Random.Range(minSize.z, maxSize.z + 1));

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
		int maxX = size.x - 1;
		int maxY = size.y - 1;
		int maxZ = size.z - 1;

		Coord rotationOffset = RotationOffset(rotation.y);

		int roofY = Mathf.CeilToInt((float) size.z / 2);
		float middle = (float) maxZ / 2;

		Coord doorPos = new Coord(Random.Range(1, maxX - 1), 0, 0);
		Coord itemPos = Coord.RandomRange(Coord.Zero, new Coord(maxX, maxY, maxZ));
		Coord charPos = Coord.RandomRange(Coord.Zero, new Coord(maxX, maxY, maxZ));

		Coord left = rotation * Coord.Left;
		Coord right = rotation * Coord.Right;
		//Coord up = rotation * Coord.up;
		Coord down = rotation * Coord.Down;
		Coord back = rotation * Coord.Back;
		Coord forward = rotation * Coord.Forward;

		for (int y = 0; y < size.y + roofY; y++) {
			Coord lightCoord = new Coord(Random.Range(0, size.x), y, Random.Range(0, size.z));
			for (int x = 0; x < size.x; x++) {
				for (int z = 0; z < size.z; z++) {
					Coord localCoord = new Coord(x, y, z);

					Coord worldCoord = bottomLeft + rotation * localCoord + rotationOffset;

					location.SetTileFree(worldCoord, false);

					if (localCoord == itemPos) location.items.Add(new Equipable(worldCoord));
					if (localCoord == charPos) location.characters.Add(new Character(worldCoord, GameController.RandomRace(), Utility.RandomBool));

					if (y <= size.y) {
						//Floor
						bool isStair = y > 0 && x == maxX - (maxY - y) - 1 && z == (y == size.y ? maxZ - 1 : maxZ);

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
							location.AddWall(new Doorway(worldCoord, back, wallType));
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
						bool isRoof = z == y - size.y || maxZ - z == y - size.y;
						bool isEdge = x == 0 || x == maxX;

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
		location.AddWall(new Wall(position, direction, wallType));
	}

	public override string ToString() => name;
}