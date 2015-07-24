using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Hotbar : MonoBehaviour {

    public static Hotbar hotbar;

    public GameObject slotPrefab;

    GameObject[] slots;
	public int size = 10;
    public int currentSlot = 0;


	void Start () {
	    hotbar = this;

        slots = new GameObject[size];
        for (int i = 0; i < size; i++) {
            slots[i] = (GameObject)Instantiate(slotPrefab, Vector3.zero, Quaternion.identity);
            slots[i].transform.SetParent(gameObject.transform, false);
        }
        slots[currentSlot].transform.localScale = Vector3.one * 1.1f;
	}

	void Update () {

        int previousSlot = currentSlot;

	    float scroll = Input.GetAxis("Mouse ScrollWheel");


        if (scroll < 0f) {
            currentSlot++;
            if (currentSlot > size - 1) currentSlot = 0;
        } else if (scroll > 0f) {
            currentSlot--;
            if (currentSlot < 0) currentSlot = size-1;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            currentSlot = 0;
        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            currentSlot = 1;
        } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            currentSlot = 2;
        } else if (Input.GetKeyDown(KeyCode.Alpha4)) {
            currentSlot = 3;
        } else if (Input.GetKeyDown(KeyCode.Alpha5)) {
            currentSlot = 4;
        } else if (Input.GetKeyDown(KeyCode.Alpha6)) {
            currentSlot = 5;
        } else if (Input.GetKeyDown(KeyCode.Alpha7)) {
            currentSlot = 6;
        } else if (Input.GetKeyDown(KeyCode.Alpha8)) {
            currentSlot = 7;
        } else if (Input.GetKeyDown(KeyCode.Alpha9)) {
            currentSlot = 8;
        } else if (Input.GetKeyDown(KeyCode.Alpha0)) {
            currentSlot = 9;
        }

        if (currentSlot != previousSlot) {
            slots[previousSlot].transform.localScale = Vector3.one;
            slots[currentSlot].transform.localScale = Vector3.one * 1.2f;
        }
	}

    public void SetImage(int index, Image image) {

    }

    public void SetCount(int index, int count) {

    }
}