using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Minecraft {
    public class ItemDatabase : MonoBehaviour {
        public static ItemDatabase itemDatabase;
        public List<Item> items;

        void Start() {
            itemDatabase = this;
            if (items.Count == 0) {
                items = new List<Item> ();
                items.Add(new Item(1, "Grass", 64, ItemTexture.Grass));
                items.Add(new Item(2, "Dirt", 64, ItemTexture.Dirt));
                items.Add(new Item(3, "Stone", 64, ItemTexture.Stone));
                items.Add(new Item(4, "Cobblestone", 64, ItemTexture.Cobblestone));
                items.Add(new Item(5, "Bedrock", 64, ItemTexture.Bedrock));
                items.Add(new Item(6, "Tree trunk", 64, ItemTexture.TreeTrunk));
                items.Add(new Item(7, "Tree leaves", 64, ItemTexture.TreeLeaves));
            }
        }

        public Item GetItemById(int id) {
            foreach (Item item in items) {
                if (id == item.id) {
                    return item;
                }
            }
            return null;
        }
    }

    [System.Serializable]
    public class Item {
        public int id;
        public string name;
        public int maxStackSize;
        public ItemTexture texture;

        public Item(int id, string name, int maxStackSize, ItemTexture texture) {
            this.id = id;
            this.name = name;
            this.maxStackSize = maxStackSize;
            this.texture = texture;
        }
    }

    [System.Serializable]
    public class ItemTexture {
        public static float resoution = 0.0625f;
        public Vector2 front = Vector2.one * resoution,
                       back = Vector2.one * resoution,
                       left = Vector2.one * resoution,
                       right = Vector2.one * resoution,
                       top = Vector2.one * resoution,
                       bottom = Vector2.one * resoution;

        public ItemTexture(Vector2 front, Vector2 back, Vector2 left, Vector2 right, Vector2 top, Vector2 bottom) {
            this.front = front * resoution;
            this.back = back * resoution;
            this.left = left * resoution;
            this.right = right * resoution;
            this.top = top * resoution;
            this.bottom = bottom * resoution;
        }

        public ItemTexture(Vector2 all) {
            this.front = all * resoution;
            this.back = all * resoution;
            this.left = all * resoution;
            this.right = all * resoution;
            this.top = all * resoution;
            this.bottom = all * resoution;
        }

        public static ItemTexture Grass = new ItemTexture(new Vector2(3f, 15f), new Vector2(3f, 15f), new Vector2(3f, 15f), new Vector2(3f, 15f), new Vector2(0f, 15f), new Vector2(2f, 15f));
        public static ItemTexture Dirt = new ItemTexture(new Vector2(2f, 15f));
        public static ItemTexture Stone = new ItemTexture(new Vector2(1f, 15f));
        public static ItemTexture Cobblestone = new ItemTexture(new Vector2(0f, 14f));
        public static ItemTexture Bedrock = new ItemTexture(new Vector2(1f, 14f));
        public static ItemTexture TreeTrunk = new ItemTexture(new Vector2(4f, 14f), new Vector2(4f, 14f), new Vector2(4f, 14f), new Vector2(4f, 14f), new Vector2(5f, 14f), new Vector2(5f, 14f));
        public static ItemTexture TreeLeaves = new ItemTexture(new Vector2(4f, 12f));
    }
}
