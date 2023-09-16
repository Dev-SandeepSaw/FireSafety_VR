using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class AnimationListAttribute : PropertyAttribute {
    public string animComponent;
    public AnimationListAttribute(string animComponent){
         this.animComponent = animComponent;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer( typeof( AnimationListAttribute ) )]
public class AnimationListDrawer : PropertyDrawer {
    AnimationListAttribute animAttribClass { get {return (AnimationListAttribute)attribute;} }
    int selectedIndex = 0;
    float propertyHeight = 0;
	public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
    {
        // property.serializedObject.Update();
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.BeginChangeCheck();
        int indent = EditorGUI.indentLevel;
        // EditorGUI.indentLevel = 0;

        string[] animNameArray = GetAnimationNames(animAttribClass.animComponent, property.serializedObject.targetObject);

        if(animNameArray == null) {
            // GUI.enabled = false;
            // GUI.Label(position, animAttribClass.animComponent + " Not Found");
            // EditorGUI.HelpBox(position,animAttribClass.animComponent + " Not Found",MessageType.Error);
            // EditorGUI.PropertyField(position,property);
            // GUI.enabled = true;
            propertyHeight = 0;
            return;
        }

        // Currently Selected Name
        string currentAnimName = property.stringValue;

        // Index Of Selected Name
        selectedIndex = animNameArray.ToList().IndexOf(currentAnimName);
        if(selectedIndex > animNameArray.Length || selectedIndex < 0) selectedIndex = 0;

         // Create Popup list
        selectedIndex = EditorGUI.Popup(position, property.displayName, selectedIndex, animNameArray);

        property.stringValue = animNameArray[selectedIndex];

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
        EditorGUI.EndChangeCheck();
        // property.serializedObject.ApplyModifiedProperties();

        propertyHeight = base.GetPropertyHeight(property,label);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return propertyHeight;
    }
    public string[] GetAnimationNames(string anim, UnityEngine.Object target)
    {
        System.Object objectValue = target.GetType().GetField(anim).GetValue(target);
        
        Animation animation = (Animation) objectValue;

        // if(animation == null) return null;

        // List<string> animNameList = new List<string>();

        // foreach (AnimationState animState in animation)
        // {
        //     if(animState == null) return null;
        //     animNameList.Add(animState.clip.name);
        // }
        // if(animNameList.Count == 0) return null;
        // return animNameList.ToArray();

        return animation.AnimationList();
    }
}
#endif