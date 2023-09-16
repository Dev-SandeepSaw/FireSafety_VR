using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
using System.Linq;
using System.Reflection;

public class FromToAttribute : PropertyAttribute {
    public bool isFrom = false;
    public bool isTo = false;
    public string animationComponent;
    public string animationName;
    public string enumProperty;
    public FromToAttribute(string animationComponent, string animationName,string enumProperty,bool isFrom = false, bool isTo = false){
        this.isFrom = isFrom;
        this.isTo = isTo;
        this.enumProperty = enumProperty;
        this.animationComponent = animationComponent;
        this.animationName = animationName;
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer( typeof( FromToAttribute ) )]
public class FromToDrawer : PropertyDrawer
{
    FromToAttribute fromToAttribClass { get {return (FromToAttribute)attribute;} }

    public override void OnGUI( Rect rect, SerializedProperty serializedProperty, GUIContent label )
    {
        // serializedProperty.serializedObject.Update();
        EditorGUI.BeginProperty(rect, label, serializedProperty);
        EditorGUI.BeginChangeCheck();
        int indent = EditorGUI.indentLevel;
        // EditorGUI.indentLevel = 0;

        // Get Value from Attribute
		bool isFrom = fromToAttribClass.isFrom;
		bool isTo = fromToAttribClass.isTo;
        string enumProperty = fromToAttribClass.enumProperty;
        string animationComponent = fromToAttribClass.animationComponent;
        string animationName = fromToAttribClass.animationName;

        System.Object target = serializedProperty.serializedObject.targetObject;

        System.Object enumVar = GetTargetObjectOfProperty(serializedProperty,enumProperty);
        string animName = (string) GetTargetObjectOfProperty(serializedProperty,animationName);
        if(animName == "") return;
        Animation anim = (Animation) target.GetType().GetField(animationComponent).GetValue(target);
        string enumName = enumVar.GetType().GetEnumName(enumVar);

        // --------------------------- Start From - To ------------------------------------------
        // SerializedProperty fromProperty = element.FindPropertyRelative("from");
        // SerializedProperty toProperty = element.FindPropertyRelative("to");
        
        string propertyLabel = "";

        if(enumName == "Frame") // Frame
        {
            float totalTime = anim[ animName ].length;
            float frameRate = anim[ animName ].clip.frameRate;
            float totalFrame = Mathf.RoundToInt(frameRate * totalTime);

            if(isFrom)
                propertyLabel = "From Frame : ";
            else
                propertyLabel = "To Frame : ";
            
            serializedProperty.floatValue = Mathf.RoundToInt( EditorGUI.Slider(rect,propertyLabel,serializedProperty.floatValue , 0f, totalFrame) );
        }
        if(enumName == "Time") // Time
        {
            float totalTime = anim[animName].length;
            if(isFrom)
                propertyLabel = "From Time : ";
            else
                propertyLabel = "To Time : ";
            serializedProperty.floatValue = EditorGUI.Slider(rect,propertyLabel,serializedProperty.floatValue , 0f, totalTime);
            // serializedProperty.floatValue = EditorGUI.Slider(rect,propertyLabel,serializedProperty.floatValue , 0f, totalTime);
        }
        if(enumName == "Percentage") // Percentage
        {
            if(isFrom)
                propertyLabel = "From % : ";
            else
                propertyLabel = "To % : ";
            serializedProperty.floatValue = EditorGUI.Slider(rect,propertyLabel,serializedProperty.floatValue , 0f, 100f);
            // serializedProperty.floatValue = EditorGUI.Slider(rect,"From % : ",serializedProperty.floatValue , 0f, 100f);
        }
        // --------------------------- End From - To ------------------------------------------

        // Debug.Log(enumVar.GetType().GetEnumName(enumVar));

        // EditorGUI.PropertyField(rect,serializedProperty, new GUIContent("From"));

        // SerializedProperty animFromValue = serializedProperty.FindPropertyRelative( "animFromValue" );
        // EditorGUI.PropertyField(rect,animFromValue,new GUIContent("From"));
        

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
        EditorGUI.EndChangeCheck();
        // serializedProperty.serializedObject.ApplyModifiedProperties();

        // Caching.ClearCache();
    }

    public override float GetPropertyHeight( SerializedProperty serializedProperty, GUIContent label )
    {
        return base.GetPropertyHeight(serializedProperty,label);;
    }
    public static object GetTargetObjectOfProperty(SerializedProperty prop, string propertyName = null , bool returnObject = false)
	{
		string path = prop.propertyPath.Replace(".Array.data[", "[");
		// Debug.Log("Path => " + path);
		object obj = prop.serializedObject.targetObject;
		string[] elements = path.Split('.');

        if(propertyName != null) elements[elements.Length - 1 ] = propertyName;

        if(returnObject && propertyName == null)
        {
            elements[elements.Length - 1] = null;
        }

		foreach (string element in elements)
		{
            if(element == null) continue;
			if (element.Contains("["))
			{
				string elementName = element.Substring(0, element.IndexOf("["));
				int index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
				obj = GetValue_Imp(obj, elementName, index);

				// Debug.Log("Array Element => " + elementName + " | index => " + index);
			}
			else
			{
				// Debug.Log("Element => " + element);
				obj = GetValue_Imp(obj, element);
			}
		}
		return obj;
	}
	private static object GetValue_Imp(object source, string name)
	{
		if (source == null)
			return null;
		var type = source.GetType();

		while (type != null)
		{
			var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			if (f != null)
				return f.GetValue(source);

			var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
			if (p != null)
				return p.GetValue(source, null);

			type = type.BaseType;
		}
		return null;
	}

	private static object GetValue_Imp(object source, string name, int index)
	{
		var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
		if (enumerable == null) return null;
		var enm = enumerable.GetEnumerator();
		//while (index-- >= 0)
		//    enm.MoveNext();
		//return enm.Current;

		for (int i = 0; i <= index; i++)
		{
            
			if (!enm.MoveNext()) return null;
		}
		return enm.Current;
	}
    

}

#endif