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

	[SerializeField] private ShadowCastingMode shadowCastingModeActive = ShadowCastingMode.On;
	[SerializeField] private ShadowCastingMode shadowCastingModeInactive = ShadowCastingMode.ShadowsOnly;
	[SerializeField] private Material materialActive;
	[SerializeField] private Material materialInactive;

	protected virtual void Awake() {
		renderers = GetComponentsInChildren<Renderer>();
		//lights = GetComponentsInChildren<Light>();
		colliders = GetComponentsInChildren<Collider>();
		ObjectManager.Hideables.Add(this);
	}

	public void UpdateDisplay() {
		seen = manualMode ? manualSeen : Entity.seen;
		visible = manualMode ? manualVisible : Entity.visible;

		if (renderers != null)
			foreach (Renderer renderer in renderers) {
				renderer.enabled = seen || Entity.isInRange && shadowCastingModeInactive == ShadowCastingMode.ShadowsOnly;
				if (renderer.enabled) {
					renderer.shadowCastingMode = seen ? shadowCastingModeActive : shadowCastingModeInactive;
					renderer.receiveShadows = visible;
					renderer.material = visible ? materialActive : materialInactive;
				}
			}

		/*if (lights != null)
			foreach (Light light in lights) {
				light.enabled = visible;
			}*/

		if (colliders != null)
			foreach (Collider collider in colliders) {
				collider.gameObject.layer = seen ? ObjectManager.VisibleTerrainLayer : ObjectManager.HiddenTerrainLayer;
			}

		if (colliders != null)
			foreach (Collider collider in colliders) {
				collider.gameObject.layer = seen ? ObjectManager.VisibleTerrainLayer : ObjectManager.HiddenTerrainLayer;
			}
	}
}