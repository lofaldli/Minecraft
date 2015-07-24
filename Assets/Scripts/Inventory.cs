using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : MonoBehaviour {

	public InventoryItem[] hotbar;
    public int size = 10;

    public int itemInHand = 0;

	void Start () {
        if (hotbar.Length == 0) {
            hotbar = new InventoryItem[size];
        }
	}

	void Update () {

	}

    public bool PickUp(int id, int count) {
        foreach (InventoryItem item in hotbar) {
            if (item.id == id) {
                item.count += count;
                return true;
            }
        }

        return false;
    }


}

[System.Serializable]
public class InventoryItem {

    public int id = 0;
    public int count = 0;

    public InventoryItem() {}

    public InventoryItem(int id, int count) {
        this.id = id;
        if (id != 0) {
            this.count = count;
        }
    }


}
