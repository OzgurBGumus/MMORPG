#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemDropSettings))]
public class ItemDropSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ItemDropSettings script = (ItemDropSettings)target;

        GUILayout.Space(16);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Restore Defaults"))
        {
            script.DefaultSettings();

            EditorUtility.SetDirty(script);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
}
#endif