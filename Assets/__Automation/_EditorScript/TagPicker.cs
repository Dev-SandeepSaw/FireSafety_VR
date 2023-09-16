using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif


[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
public class TagPicker : PropertyAttribute
{
    public bool showPath = false;
    public TagPicker() { }
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(TagPicker))]
public class TagPickerPropertyDrawer : PropertyDrawer
{
    private int selectedIndex;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property.serializedObject.Update();
        
        TagPicker picker = (TagPicker)attribute;
        
        string[] validTags = GetValidTags();
        
        // set index of popup list to current position
        string currentTag = property.stringValue;
        selectedIndex = (currentTag == "") ? 0 : validTags.ToList().IndexOf(currentTag);
        
        // Create Popup list
        selectedIndex = EditorGUI.Popup(position, property.displayName, selectedIndex, validTags);
        property.stringValue = validTags[selectedIndex];
        
        property.serializedObject.ApplyModifiedProperties();
    }

    private string[] GetValidTags()
    {
        List<string> Tags = new List<string>();

        UnityEngine.Object[] asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if ((asset != null) && (asset.Length > 0))
        {
            SerializedObject serializedObject = new SerializedObject(asset[0]);
            SerializedProperty tags = serializedObject.FindProperty("tags");

            for (int i = 0; i < tags.arraySize; ++i)
            {
                Tags.Add(tags.GetArrayElementAtIndex(i).stringValue);
            }
        }
        return Tags.ToArray();
    }
}
#endif
