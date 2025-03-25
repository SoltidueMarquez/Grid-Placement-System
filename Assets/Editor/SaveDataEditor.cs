using GridPlacement;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(SaveData))]
    public class SaveDataEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var data = (SaveData)target;
            
            if (GUILayout.Button("清空", GUILayout.Height(30)))
            {
                data.Clear();
            }
        }
    }
}