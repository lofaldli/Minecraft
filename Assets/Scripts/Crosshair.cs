using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour {

    Image image;

	void Start () {
	    image = GetComponent<Image> ();
        image.enabled = true;
	}

	public void Hide() {
        image.enabled = false;
    }

    public void Show() {
        image.enabled = true;
    }
}
