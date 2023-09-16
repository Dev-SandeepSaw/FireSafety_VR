#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class HierarchyWindowGroupHeader
{
    static Dictionary<string, Color> colorDict = new Dictionary<string, Color>();
    static HierarchyWindowGroupHeader ()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        colorDict.Clear();
    }
    static void HierarchyWindowItemOnGUI (int instanceID, Rect selectionRect)
    {
        var gameObject = EditorUtility.InstanceIDToObject (instanceID) as GameObject;
        if (gameObject != null && gameObject.name.StartsWith ("---", System.StringComparison.Ordinal))
        {
            if(!gameObject.activeInHierarchy)
            {
                EditorGUI.DrawRect (selectionRect, Color.gray);

                EditorGUI.DropShadowLabel (selectionRect, gameObject.name.Replace ("-", "").ToUpperInvariant () );
            }else
            {
                if(!colorDict.ContainsKey(gameObject.name)) colorDict.Add(gameObject.name, Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));

                // Pick a random, saturated and not-too-dark color
                EditorGUI.DrawRect (selectionRect, colorDict[gameObject.name]);

                EditorGUI.DropShadowLabel (selectionRect, gameObject.name.Replace ("-", "").ToUpperInvariant () );
            }
            
        }
    }
}
#endif