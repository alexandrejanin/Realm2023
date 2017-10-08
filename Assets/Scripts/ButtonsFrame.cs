using UnityEngine;
using UnityEngine.UI;

public class ButtonsFrame : MonoBehaviour {
	[SerializeField] private float baseHeight;
	[SerializeField] private float heightPerElement;
	[SerializeField] private LayoutGroup layoutGroup;

	public void Size() {
		GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, baseHeight + heightPerElement * layoutGroup.transform.childCount);
	}
}