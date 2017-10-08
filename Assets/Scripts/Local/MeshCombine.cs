using System.Collections.Generic;
using UnityEngine;

public static class MeshCombine {

	public static void CombineMeshes(IList<MeshFilter> meshFilters, Transform parent) {
		foreach (MeshFilter filter in meshFilters) {
			filter.transform.parent = parent;
		}

		Quaternion rotation = parent.rotation;
		Vector3 position = parent.localPosition;

		parent.rotation = Quaternion.identity;
		parent.position = Vector3.zero;

		Mesh combinedMesh = new Mesh {subMeshCount = 2};
		CombineInstance[] combineInstances = new CombineInstance[meshFilters.Count];

		MeshFilter meshFilter = parent.gameObject.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = parent.gameObject.AddComponent<MeshRenderer>();
		MeshCollider meshCollider = parent.gameObject.AddComponent<MeshCollider>();

		for (int i = 0; i < meshFilters.Count; i++) {
			if (meshFilters[i] == meshFilter) continue;

			combineInstances[i].subMeshIndex = 0;
			combineInstances[i].mesh = meshFilters[i].sharedMesh;
			combineInstances[i].transform = meshFilters[i].transform.localToWorldMatrix;
			Object.Destroy(meshFilters[i].gameObject);
		}

		combinedMesh.CombineMeshes(combineInstances, true);

		meshFilter.mesh = combinedMesh;
		meshRenderer.material = meshFilters[0].GetComponent<MeshRenderer>().material;
		meshCollider.sharedMesh = combinedMesh;

		parent.rotation = rotation;
		parent.localPosition = position;
	}
}