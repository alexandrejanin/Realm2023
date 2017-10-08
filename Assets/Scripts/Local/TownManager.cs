using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TownManager : MonoBehaviour {
	private static TownManager instance;

	[SerializeField] private GameObject roadTile;
	[SerializeField] private GameObject grassTile;
	[SerializeField] private Blueprint[] blueprints;

	[SerializeField] private Transform buildingParent;
	[SerializeField] private Transform grassParent;
	[SerializeField] private Transform roadParent;

	[SerializeField] private int buildingsAmount = 50;

	private readonly Coord worldSize = new Coord(100, 5, 100);

	private readonly Dictionary<Coord, bool> tiles = new Dictionary<Coord, bool>();

	public void GenerateTown() {
		instance = this;
		InitializeDictionary();
		NodeGrid.CreateGrid();
		CreateGround();
		CreateRoads();
		CreateBuildings();
		SpawnPlayer();
	}

	private static void SpawnPlayer() {
		new Character(new Coord(Random.Range(5, 95), 0, Random.Range(5, 95)), true);
	}

	private void InitializeDictionary() {
		for (int x = 0; x < worldSize.x; x++) {
			for (int y = 0; y < worldSize.y; y++) {
				for (int z = 0; z < worldSize.z; z++) {
					tiles.Add(new Coord(x, y, z), false);
				}
			}
		}
	}

	public static void SetTileTaken(Coord coord, bool value) => instance.tiles[coord] = value;

	private void CreateGround() {
		for (int x = 0; x < worldSize.x; x++) {
			for (int z = 0; z < worldSize.z; z++) {
				new Wall(new Coord(x, 0, z), Coord.down); //Instantiate(grassTile, NodeGrid.GetWorldPosFromCoord(x, 0, z, NodeGrid.NodeOffsetType.CenterNoY) + grassTile.transform.position, Quaternion.identity, grassParent);
			}
		}
	}

	private void CreateRoads() {
		Queue<Road> roadQueue = new Queue<Road>();
		roadQueue.Enqueue(CreateRoad(new Coord(0, 0, Random.Range(10, 90)), new Coord(99, 0, Random.Range(10, 90)), 3));
	}

	private Road CreateRoad(Coord start, Coord end, int width = 1) {
		List<Coord> positions = NodeGrid.GetLine(start, end);

		Coord direction = (end - start);

		bool includeDiagonals = width % 2 > 0;

		bool horizontal = direction.x >= direction.z;

		int maxI = positions.Count;

		Transform parent = new GameObject().transform;
		parent.parent = roadParent;
		parent.gameObject.layer = ObjectManager.VisibleTerrainLayer;

		List<MeshFilter> meshFilters = new List<MeshFilter>();

		for (int i = 0; i < positions.Count; i++) {
			Coord position = positions[i];
			if (!NodeGrid.IsInGrid(position)) {
				maxI = i;
				break;
			}
			for (int x = 0; x < (width == 1 && horizontal ? 2 : width); x++) {
				for (int z = 0; z < (width == 1 && !horizontal ? 2 : width); z++) {
					if (x == 0 || z == 0 || includeDiagonals) {
						Coord tilePos = position + new Coord(x, 0, z);
						bool tileIsTaken;
						if (tiles.TryGetValue(tilePos, out tileIsTaken) && !tileIsTaken) {
							tiles[tilePos] = true;
							meshFilters.Add(CreateTile(parent, roadTile, NodeGrid.GetWorldPosFromCoord(tilePos, NodeGrid.NodeOffsetType.CenterNoY), Quaternion.identity).GetComponent<MeshFilter>());
						}
					}
				}
			}
		}

		MeshCombine.CombineMeshes(meshFilters, parent);

		return new Road(positions.GetRange(0, maxI).ToArray(), (end - start).Normalize, width);
	}

	private static GameObject CreateTile(Transform parent,
		GameObject prefab,
		Vector3 worldPosition,
		Quaternion rootRotation,
		Vector3 positionOffset = new Vector3(),
		Vector3 eulerAngles = new Vector3(),
		Vector3 scale = new Vector3()) {
		if (scale == new Vector3()) scale = new Vector3(1, 1, 1);

		Quaternion rotation = Quaternion.Euler(rootRotation.eulerAngles + eulerAngles + prefab.transform.eulerAngles);

		GameObject tile = Instantiate(prefab, worldPosition + rootRotation * positionOffset + rotation * prefab.transform.position, rotation, parent);
		tile.transform.localScale = scale;
		return tile;
	}

	private void CreateBuildings() {
		for (int i = 0; i < buildingsAmount; i++) {
			AddRandomBuilding();
		}
	}

	private void AddRandomBuilding() {
		Blueprint blueprint = blueprints.RandomItem();
		Coord rotationCoord = new Coord(0, Random.Range(0, 4), 0);
		Quaternion rotation = Quaternion.Euler(rotationCoord * 90);
		Coord size = blueprint.RandomSize();

		Coord offset = Blueprint.RotationOffset(rotationCoord.y);

		bool validPosition = false;
		Vector3 center = new Vector3();
		int tries = 0;

		while (!validPosition && tries < 100) {
			Coord bottomLeft = new Coord(Random.Range(0, worldSize.x - (rotation * size).x), 0, Random.Range(0, worldSize.z - (rotation * size).z));
			center = bottomLeft + rotation * (Vector3) size / 2;
			validPosition = true;

			for (int x = -1; x <= size.x; x++) {
				for (int y = 0; y < size.y; y++) {
					for (int z = -1; z <= size.z; z++) {
						Coord tileCoord = new Coord(x, y, z);
						Coord tilePos = bottomLeft + rotation * tileCoord + offset;

						bool tileTaken;
						if (!tiles.TryGetValue(tilePos, out tileTaken) || tileTaken) validPosition = false;
					}
				}
			}

			tries++;
		}

		if (validPosition) {
			blueprint.GenerateBuilding(buildingParent, center, size, rotationCoord);
		} else {
			Debug.Log("Could not find valid position");
		}
	}


	private struct Road {
		public readonly Coord[] tiles;
		public readonly Vector3 direction;
		public readonly int width;

		public Road(Coord[] tiles, Vector3 direction, int width) {
			this.tiles = tiles;
			this.direction = direction;
			this.width = width;
		}
	}
}