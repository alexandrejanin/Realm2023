using UnityEngine;
using UnityEngine.Rendering;

public abstract class EntityObject : MonoBehaviour {
	public abstract Entity Entity { get; }

	public bool manualMode;
	public bool manualVisible;
	public bool manualSeen;

	private bool seen, visible;

	private Renderer[] renderers;
	private Collider[] colliders;

	[SerializeField] private Material materialActive;
	[SerializeField] private Material materialInactive;

	[SerializeField] private int hiddenLayer = 8;
	[SerializeField] private int visibleLayer = 9;

	protected virtual void Awake() {
		renderers = GetComponentsInChildren<Renderer>();
		colliders = GetComponentsInChildren<Collider>();
		ObjectManager.EntityObjects.Add(this);
	}

	protected void Destroy() {
		ObjectManager.DisplayedEntities.Remove(Entity);
		Destroy(gameObject);
	}

	public void UpdateDisplay() {
		if (Entity == null) return;

		seen = manualMode ? manualSeen : Entity.seen;
		visible = manualMode ? manualVisible : Entity.visible;

		if (renderers != null) {
			foreach (Renderer renderer in renderers) {
				renderer.enabled = seen || Entity.inRenderRange;
				if (renderer.enabled) {
					renderer.shadowCastingMode = seen ? ShadowCastingMode.On : ShadowCastingMode.ShadowsOnly;
					renderer.receiveShadows = visible;
					renderer.material = visible ? materialActive : materialInactive;
				}
			}
		}

		if (colliders != null) {
			foreach (Collider collider in colliders) {
				collider.gameObject.layer = seen ? visibleLayer : hiddenLayer;
			}
		}
	}
}