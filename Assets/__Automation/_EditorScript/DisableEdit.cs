using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
using UnityEditor;
#endif
[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
public class DisableEditAttribute: PropertyAttribute
{
   public string header;
   public DisableEditAttribute(string header = null){
       this.header = header;
   }
}
#if UNITY_EDITOR
[CustomPropertyDrawer( typeof( DisableEditAttribute ) )]
public class DisableEditAttributeDrawer : PropertyDrawer
{
    DisableEditAttribute disableEditAttribClass { get {return (DisableEditAttribute)attribute;} }
    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
    {
        // property.serializedObject.Update();
        var fieldPos = position;
        fieldPos.width -= 18;

        label = EditorGUI.BeginProperty( position, label, property );
        // using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        // {
            if(disableEditAttribClass.header != null)
            {
                EditorGUI.LabelField(fieldPos, disableEditAttribClass.header, EditorStyles.boldLabel);
                fieldPos.y += EditorGUIUtility.singleLineHeight;
            }
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUI.PropertyField( fieldPos, property, label );
            }
        // }
        EditorGUI . EndProperty ();
        // property.serializedObject.ApplyModifiedProperties();

    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float addHeight = 0;
        if(disableEditAttribClass.header != null)
        {
            addHeight += EditorGUIUtility.singleLineHeight;
        }
        return base.GetPropertyHeight(property,label) + addHeight;
    }
}
#endif