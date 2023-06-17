using UnityEngine;

public class ButtonsFrame : MonoBehaviour {
    public Transform buttonsParent;

    public void SetSize(float heightPerButton) {
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, (buttonsParent.childCount + 1) * heightPerButton);
    }
}