using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class GetComponentInChildrenAttribute: PropertyAttribute
{
   public GetComponentInChildrenAttribute(){}
}

#if UNITY_EDITOR
[CustomPropertyDrawer( typeof( GetComponentInChildrenAttribute ) )]
public class GetComponentInChildrenDrawer : PropertyDrawer
{
    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
    {
        var fieldPos = position;
        fieldPos.width -= 18;

        label = EditorGUI.BeginProperty( position, label, property );
        EditorGUI.PropertyField( fieldPos, property, label );

        Rect  buttonPos  =  position ;
        buttonPos.xMin = buttonPos.xMax - 13;

        if ( GUI.Button( buttonPos, GUIContent.none, new GUIStyle( "miniButton" )    ) )
        {
            var obj = property.serializedObject;
            var go = obj.targetObject as Component;
            var com = go.gameObject.GetComponentInChildren( GetPropertyType( property ), true );

            property.serializedObject.Update();
            property.objectReferenceValue = com;
            property.serializedObject.ApplyModifiedProperties();
        }

        EditorGUI . EndProperty ();
    }

    private static string GetPropertyType( SerializedProperty property )
    {
        var type = property.type;
        var match = Regex.Match( type, @"PPtr<\$(.*?)>" );
        return match.Success ? match.Groups[ 1 ].Value : type;
    }
}
#endif