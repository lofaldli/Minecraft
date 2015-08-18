using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Minecraft.Inspector {
    [CustomEditor(typeof(ItemDatabase))]
    public class ItemDatabaseEditor : Editor {

        public override void OnInspectorGUI() {
            serializedObject.Update();
            ItemDatabase itemDatabase = (ItemDatabase)target;
            foreach (Item item in itemDatabase.items) {
                EditorGUILayout.IntField("Id: ", item.id);
                EditorGUILayout.TextField("Name: ", item.name);
                EditorGUILayout.IntField("Stack: ", item.maxStackSize);
                EditorGUILayout.Space();
            }
        }
    }
}
