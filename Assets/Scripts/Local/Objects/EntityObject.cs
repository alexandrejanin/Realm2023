using UnityEngine;
using UnityEngine.Rendering;

public abstract class EntityObject : MonoBehaviour {
    public abstract Entity Entity { get; }

    public bool manualMode;
    public bool manualVisible;
    public bool manualSeen;

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

        var seen = manualMode ? manualSeen : Entity.seen;
        var visible = manualMode ? manualVisible : Entity.visible;

        if (renderers != null) {
            foreach (var r in renderers) {
                r.enabled = seen || Entity.inRenderRange;
                if (r.enabled) {
                    r.shadowCastingMode = seen ? ShadowCastingMode.On : ShadowCastingMode.ShadowsOnly;
                    r.receiveShadows = visible;
                    r.material = visible ? materialActive : materialInactive;
                }
            }
        }

        if (colliders != null) {
            foreach (var c in colliders) {
                c.gameObject.layer = seen ? visibleLayer : hiddenLayer;
            }
        }
    }
}