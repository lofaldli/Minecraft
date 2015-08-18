using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TooltipUI : MonoBehaviour {

    public static Text text;
    public static Image image;

    static bool active = true;

    void Start () {
        text = GetComponentInChildren<Text> ();
        image = GetComponent<Image> ();
        SetActive(false);
    }

	void Update () {
        if (active) {
            GetComponent<RectTransform>().sizeDelta = new Vector2(text.preferredWidth + 10, text.preferredHeight);
        }
	}

    public static void SetActive(bool active) {
        TooltipUI.active = active;
        text.enabled = active;
        image.enabled = active;
    }
}
