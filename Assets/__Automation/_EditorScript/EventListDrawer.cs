#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAutomation;
using UnityEditor;

[CustomPropertyDrawer( typeof( EventList ) )]
public class EventListDrawer : PropertyDrawer
{
    float height = 0;
    SerializedProperty unityObj, selectedEvent,doAllOfThese;
    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
    {
        unityObj = property.FindPropertyRelative("unityObj");
        selectedEvent = property.FindPropertyRelative("selectedEvent");      
        doAllOfThese = property.FindPropertyRelative("doAllOfThese");      

        float singleLineHeight = EditorGUIUtility.singleLineHeight;

        EditorGUI.BeginChangeCheck();
        EditorGUI.BeginProperty(position, label, property);

        Rect newPos = position;
        newPos.x = position.x + 10;
        newPos.height = singleLineHeight;
        
        newPos.width = Mathf.Lerp(0f, position.width, 0.3f); //position.width / 4;
        EditorGUI.PropertyField(newPos, unityObj, GUIContent.none);

        if (unityObj.objectReferenceValue != null)
        {
            newPos.x += newPos.width + 5;
            newPos.width = Mathf.Lerp(0f, position.width, 0.7f) - 10;
            EditorGUI.PropertyField(newPos,selectedEvent, GUIContent.none);

            newPos.x = position.x + 5;
            newPos.y += singleLineHeight;
            newPos.width = position.width;
            EditorGUI.PropertyField(newPos,doAllOfThese, GUIContent.none); 
        }

        EditorGUI.EndProperty();
        EditorGUI.EndChangeCheck();
        property.serializedObject.ApplyModifiedProperties();   
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        height = 0;
        height += getHeight(property, "unityObj");
        if (property.FindPropertyRelative("unityObj").objectReferenceValue != null)
            height += getHeight(property, "doAllOfThese");
        return height;
    }
    public float getHeight(SerializedProperty property, string name)
    {
        if(property.FindPropertyRelative(name) == null) return EditorGUIUtility.singleLineHeight;
        return EditorGUI.GetPropertyHeight(property.FindPropertyRelative(name));
    }
}


[CustomPropertyDrawer( typeof( AllEventList ) )]
public class AllEventListDrawer : PropertyDrawer
{
    float height = 0;
    SerializedProperty unityObj, selectedEvent;
    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
    {
        unityObj = property.FindPropertyRelative("unityObj");
        selectedEvent = property.FindPropertyRelative("selectedEvent");      

        float singleLineHeight = EditorGUIUtility.singleLineHeight;

        EditorGUI.BeginChangeCheck();
        EditorGUI.BeginProperty(position, label, property);

        Rect newPos = position;
        newPos.x = position.x + 10;
        newPos.height = singleLineHeight;
        
        newPos.width = Mathf.Lerp(0f, position.width, 0.3f); //position.width / 4;
        EditorGUI.PropertyField(newPos, unityObj, GUIContent.none);

        if (unityObj.objectReferenceValue != null)
        {
            newPos.x += newPos.width + 5;
            newPos.width = Mathf.Lerp(0f, position.width, 0.7f) - 10; //position.width / 4;
            EditorGUI.PropertyField(newPos,selectedEvent, GUIContent.none);
        }

        EditorGUI.EndProperty();
        EditorGUI.EndChangeCheck();
        property.serializedObject.ApplyModifiedProperties();   
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        height = 0;
        height += getHeight(property, "unityObj");
        return height;
    }
    public float getHeight(SerializedProperty property, string name)
    {
        if(property.FindPropertyRelative(name) == null) return EditorGUIUtility.singleLineHeight;
        return EditorGUI.GetPropertyHeight(property.FindPropertyRelative(name));
    }
}

#endif