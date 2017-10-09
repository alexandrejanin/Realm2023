using UnityEngine;
using UnityEngine.Rendering;

// ReSharper disable LocalVariableHidesMember

public abstract class HideableObject : MonoBehaviour {
	protected abstract Entity Entity { get; }

	[HideInInspector] public bool manualMode;
	[HideInInspector] public bool manualVisible;
	[HideInInspector] public bool manualSeen;

	private bool seen = true, visible;

	private Renderer[] renderers;

	//private Light[] lights;
	private Collider[] colliders;

	private const ShadowCastingMode shadowCastingModeActive = ShadowCastingMode.On;
	private const ShadowCastingMode shadowCastingModeInactive = ShadowCastingMode.ShadowsOnly;
	[SerializeField] private Material materialActive;
	[SerializeField] private Material materialInactive;

	protected virtual void Awake() {
		renderers = GetComponentsInChildren<Renderer>();
		//lights = GetComponentsInChildren<Light>();
		colliders = GetComponentsInChildren<Collider>();
		ObjectManager.Hideables.Add(this);
	}

	protected void Destroy() {
		Entity.displayed = false;
		Destroy(gameObject);
	}

	public void UpdateDisplay() {
		if (Entity == null) return;

		seen = manualMode ? manualSeen : Entity.seen && Entity.isInSeenRange;
		visible = manualMode ? manualVisible : Entity.visible;

		if (renderers != null) {
			foreach (Renderer renderer in renderers) {
				renderer.enabled = seen || Entity.isInViewRange;
				if (renderer.enabled) {
					renderer.shadowCastingMode = seen ? shadowCastingModeActive : shadowCastingModeInactive;
					renderer.receiveShadows = visible;
					renderer.material = visible ? materialActive : materialInactive;
				}
			}
		}

		/*if (lights != null)
			foreach (Light light in lights) {
				light.enabled = visible;
			}*/

		if (colliders != null) {
			foreach (Collider collider in colliders) {
				collider.gameObject.layer = seen ? ObjectManager.VisibleTerrainLayer : ObjectManager.HiddenTerrainLayer;
			}
		}
	}
}