using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Blueprint {
	[SerializeField] private string name;

	[SerializeField] private Coord minSize;
	[SerializeField] private Coord maxSize;

	[SerializeField] private GameObject floorTile;
	[SerializeField] private GameObject roofTile;
	[SerializeField] private GameObject wallTile;
	[SerializeField] private GameObject halfWallTile;
	[SerializeField] private GameObject doorPrefab;
	[SerializeField] private GameObject lightPrefab;

	private readonly List<Light> lights = new List<Light>();
	private readonly List<Interactable> interactables = new List<Interactable>();

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
				return Coord.zero;
		}
	}

	public void GenerateBuilding(Transform buildingParent, Vector3 center, Coord size, Coord rotationMultiplier, string objectName = "") {
		if (size.x * size.y * size.z == 0) {
			Debug.Log("Error: Blueprint Dimension = 0");
		}
		Quaternion rotation = Quaternion.Euler(90 * rotationMultiplier);
		Coord rotatedSize = rotation * size;
		Coord bottomLeft = new Coord(center - (Vector3) rotatedSize / 2);

		Transform parent = new GameObject(objectName == "" ? name : objectName).transform;
		parent.name = name;
		parent.parent = buildingParent;
		parent.position = center;
		parent.rotation = rotation;

		int maxX = size.x - 1;
		int maxY = size.y - 1;
		int maxZ = size.z - 1;

		Coord rotationOffset = RotationOffset(rotationMultiplier.y);

		int roofY = Mathf.CeilToInt((float) size.z / 2);
		float middle = (float) maxZ / 2;

		Coord doorPos = new Coord(Random.Range(1, maxX - 1), 0, 0);
		Coord itemPos = Coord.RandomRange(Coord.zero, new Coord(maxX, maxY, maxZ));

		List<GameObject>[] blocks = new List<GameObject>[size.y + roofY]; //Walls to combine

		for (int y = 0; y < size.y + roofY; y++) {
			Coord lightCoord = new Coord(Random.Range(0, size.x), y, Random.Range(0, size.z));

			blocks[y] = new List<GameObject>();

			for (int x = 0; x < size.x; x++) {
				for (int z = 0; z < size.z; z++) {
					Coord localCoord = new Coord(x, y, z);

					Coord worldCoord = bottomLeft + rotation * localCoord + rotationOffset;
					Vector3 worldPos = NodeGrid.GetWorldPosFromCoord(worldCoord, NodeGrid.NodeOffsetType.CenterNoY);

					TownManager.SetTileTaken(worldCoord, true);

					if (localCoord == itemPos) new Equipable(worldCoord);

					if (y <= size.y) {
						//Floor
						bool isStair = y > 0 && x == maxX - (maxY - y) - 1 && z == (y == size.y ? maxZ - 1 : maxZ);

						GameObject floor;
						if (isStair) {
							floor = y == size.y
								? TownManager.CreateTile(parent, wallTile, worldPos, rotation, new Vector3(-0.5f, -0.5f, 0), new Vector3(0, 0, -90)) //Attic stairs
								: TownManager.CreateTile(parent, wallTile, worldPos, rotation, new Vector3(0.5f, -0.5f, 0), new Vector3(0, 0, 90)); //Normal stairs
							blocks[y - 1].Add(floor);
							NodeGrid.BlockPassage(worldCoord + new Coord(0, -1, 0), rotation * (y == size.y ? new Coord(-1, 0, 0) : new Coord(1, 0, 0)));
						} else {
							floor = TownManager.CreateTile(parent, floorTile, worldPos, rotation);
							blocks[y].Add(floor);
							NodeGrid.BlockPassage(worldCoord, new Coord(0, -1, 0));
						}
					}

					if (y < size.y) {
						//Walls
						if (localCoord == lightCoord) {
							GameObject light = TownManager.CreateTile(parent, lightPrefab, worldPos, rotation, new Vector3(x < size.x / 2 ? 0.5f : -0.5f, 0, z < size.z / 2 ? 0.5f : -0.5f));
							blocks[y].Add(light);
							lights.Add(light.GetComponent<Light>());
							light.GetComponent<Light>().enabled = false;
						}

						if (localCoord == doorPos) {
							GameObject doorObject = TownManager.CreateTile(parent, doorPrefab, worldPos, rotation, new Vector3(0, 0.5f, -0.5f), new Vector3(0, 0, 0));
							blocks[y].Add(doorObject);
							Door door = new Door(worldCoord, rotation * new Coord(0, 0, -1));
							interactables.Add(door);
							doorObject.GetComponentInChildren<DoorObject>().door = door;
						} else {
							GameObject wall;
							if (x == 0) {
								wall = TownManager.CreateTile(parent, wallTile, worldPos, rotation, new Vector3(-0.5f, 0.5f, 0), new Vector3(90, 0, 90));
								blocks[y].Add(wall);
								NodeGrid.BlockPassage(worldCoord, rotation * new Coord(-1, 0, 0));
							}
							if (x == maxX) {
								wall = TownManager.CreateTile(parent, wallTile, worldPos, rotation, new Vector3(0.5f, 0.5f, 0), new Vector3(90, 0, 90));
								blocks[y].Add(wall);
								NodeGrid.BlockPassage(worldCoord, rotation * new Coord(1, 0, 0));
							}
							if (z == 0) {
								wall = TownManager.CreateTile(parent, wallTile, worldPos, rotation, new Vector3(0, 0.5f, -0.5f), new Vector3(-90, 0, 0));
								blocks[y].Add(wall);
								NodeGrid.BlockPassage(worldCoord, rotation * new Coord(0, 0, -1));
							}
							if (z == maxZ) {
								wall = TownManager.CreateTile(parent, wallTile, worldPos, rotation, new Vector3(0, 0.5f, 0.5f), new Vector3(90, 0, 0));
								blocks[y].Add(wall);
								NodeGrid.BlockPassage(worldCoord, rotation * new Coord(0, 0, 1));
							}
						}
					} else {
						//Roof
						bool isRoof = z == y - size.y || maxZ - z == y - size.y;
						bool isEdge = x == 0 || x == maxX;

						if (isEdge) {
							GameObject wall;
							if (isRoof && z != middle) {
								wall = TownManager.CreateTile(parent, halfWallTile, worldPos, rotation, new Vector3(x == 0 ? -0.5f : 0.5f, 0.5f, 0), new Vector3(-90, 0, z > middle ? -90 : 90));
								blocks[y].Add(wall);
								NodeGrid.BlockPassage(worldCoord, rotation * new Coord(x == 0 ? -1 : 1, 0, 0));
							} else if (y - size.y < z && y - size.y < maxZ - z) {
								wall = TownManager.CreateTile(parent, wallTile, worldPos, rotation, new Vector3(x == 0 ? -0.5f : 0.5f, 0.5f, 0), new Vector3(90, 0, 90));
								blocks[y].Add(wall);
								NodeGrid.BlockPassage(worldCoord, rotation * new Coord(x == 0 ? -1 : 1, 0, 0));
							}
						}

						if (isRoof) {
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
						}
					}
				}
			}
		}

		Floor[] floors = new Floor[blocks.Length];

		for (int i = 0; i < blocks.Length; i++) {
			List<GameObject> gameObjects = blocks[i];

			Transform floorParent = new GameObject("Floor " + i).transform;
			floorParent.position = new Vector3(parent.position.x, i, parent.position.z);
			floorParent.parent = parent;
			floorParent.gameObject.layer = ObjectManager.HiddenTerrainLayer;
			floors[i] = floorParent.gameObject.AddComponent<Floor>();

			floors[i].SetGameObjects(gameObjects.ToArray());
		}

		//Add building component
		Building building = parent.gameObject.AddComponent<Building>();

		building.bounds.center = parent.position;
		building.bounds.size = rotatedSize;
		building.floors = floors;
		building.lights = lights.ToArray();
		//building.interactables = interactables.ToArray();
	}

}