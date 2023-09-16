using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
using UnityEditor;
#endif
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
public class ShowIfTrueAttribute: PropertyAttribute
{
    public string variableName;
   public ShowIfTrueAttribute(string variableName){
       this.variableName = variableName;
   }
}
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
public class ShowIfNullAttribute: PropertyAttribute
{
    public string variableName;
   public ShowIfNullAttribute(string variableName){
       this.variableName = variableName;
   }
}
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
public class ShowIfFalseAttribute: PropertyAttribute
{
    public string variableName;
   public ShowIfFalseAttribute(string variableName){
       this.variableName = variableName;
   }
}

#if UNITY_EDITOR
[CustomPropertyDrawer( typeof( ShowIfTrueAttribute ) )]
public class ShowIfTrueAttributeDrawer : PropertyDrawer
{
    bool condition;
    ShowIfTrueAttribute showIfTrueAttribClass { get {return (ShowIfTrueAttribute)attribute;} }
    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
    {
        var fieldPos = position;
        fieldPos.width -= 18;

        label = EditorGUI.BeginProperty( position, label, property );

        System.Object propertyObj = property.GetTargetObjectOfProperty(showIfTrueAttribClass.variableName);
        if(propertyObj != null){
            condition = (bool) propertyObj;
            if(condition)
                EditorGUI.PropertyField( fieldPos, property, label );
        }else condition = false;
        EditorGUI . EndProperty ();
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if(condition)
            return base.GetPropertyHeight(property,label);
        else return 0;
    }
}
[CustomPropertyDrawer( typeof( ShowIfFalseAttribute ) )]
public class ShowIfFalseAttributeDrawer : PropertyDrawer
{
    bool condition;
    ShowIfFalseAttribute showIfFalseAttribClass { get {return (ShowIfFalseAttribute)attribute;} }
    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
    {
        var fieldPos = position;
        fieldPos.width -= 18;

        label = EditorGUI.BeginProperty( position, label, property );

        System.Object propertyObj = property.GetTargetObjectOfProperty(showIfFalseAttribClass.variableName);
        if(propertyObj != null){
            condition = (bool) propertyObj;
            if(!condition)
                EditorGUI.PropertyField( fieldPos, property, label );
        }else condition = true;

        EditorGUI . EndProperty ();
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if(!condition)
            return base.GetPropertyHeight(property,label);
        else return 0;
    }
}
[CustomPropertyDrawer( typeof( ShowIfNullAttribute ) )]
public class ShowIfNullAttributeDrawer : PropertyDrawer
{
    bool condition;
    ShowIfNullAttribute showIfNullAttribClass { get {return (ShowIfNullAttribute)attribute;} }
    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
    {
        var fieldPos = position;
        fieldPos.width -= 18;

        label = EditorGUI.BeginProperty( position, label, property );

        System.Object propertyObj = property.GetTargetObjectOfProperty(showIfNullAttribClass.variableName);
        if(propertyObj == null){
            // condition = (bool) propertyObj;
            // if(!condition)
                EditorGUI.PropertyField( fieldPos, property, label );
                condition = true;
        }
        else condition = false;

        EditorGUI . EndProperty ();
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if(condition)
            return base.GetPropertyHeight(property,label);
        else return 0;
    }
}
#endif