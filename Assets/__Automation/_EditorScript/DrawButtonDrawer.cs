using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class DrawButtonAttribute: PropertyAttribute
{
    public string buttonName;
    public bool playModeOnly;
    public bool editorModeOnly;
   public DrawButtonAttribute(string buttonName = null){
       this.buttonName = buttonName;
   }
}

#if UNITY_EDITOR
[CustomPropertyDrawer( typeof( DrawButtonAttribute ) )]
public class DrawButtonDrawer : PropertyDrawer
{
    DrawButtonAttribute buttonAttribClass { get {return (DrawButtonAttribute)attribute;} }
    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
    {
        var fieldPos = position;
        fieldPos.width -= 18;

        label = EditorGUI.BeginProperty( position, label, property );

        // Debug.Log(buttonAttribClass.GetType());
        // Debug.Log("Hello");

        if(GUILayout.Button(buttonAttribClass.buttonName))
        {
        }

        // condition = (bool) property.GetTargetObjectOfProperty(buttonAttribClass.variableName);
        // if(condition)
        //     EditorGUI.PropertyField( fieldPos, property, label );

        EditorGUI . EndProperty ();
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // if(condition)
        //     return base.GetPropertyHeight(property,label);
        // else 
            return 0;
    }
}
#endif
#if UNITY_EDITOR
[CanEditMultipleObjects] // Don't ruin everyone's day
[CustomEditor(typeof(MonoBehaviour), true)] // Target all MonoBehaviours and descendants
public class MonoBehaviourCustomEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
         // Draw the normal inspector

        // Currently this will only work in the Play mode. You'll see why
        // if (!Application.isPlaying)
        // {
            // Get the type descriptor for the MonoBehaviour we are drawing
            var type = target.GetType();

            // Iterate over each private or public instance method (no static methods atm)
            foreach (var method in type.GetMethods(BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Instance))
            {
                // make sure it is decorated by our custom attribute
                // var attributes = method.GetCustomAttributes(typeof(ButtonAttribute), true);
                // if (attributes.Length > 0)
                // {

                //     if (GUILayout.Button("Run: " + method.Name))
                //     {
                //         // If the user clicks the button, invoke the method immediately.
                //         // There are many ways to do this but I chose to use Invoke which only works in Play Mode.
                //         ((MonoBehaviour)target).Invoke(method.Name, 0f);
                //     }
                // }
                // Debug.Log(method.GetCustomAttributes(typeof(ButtonAttribute), true).Length);
                if (method.GetParameters().Length == 0)
                {
                    if(method.GetCustomAttributes(typeof(DrawButtonAttribute), true).Length > 0)
                    {
                        DrawButtonAttribute buttonAttribute = (DrawButtonAttribute)method.GetCustomAttributes(typeof(DrawButtonAttribute), true)[0];
                        string buttonText = string.IsNullOrEmpty(buttonAttribute.buttonName) ? method.Name : buttonAttribute.buttonName;

                        using (new EditorGUI.DisabledScope( (buttonAttribute.playModeOnly) ? !UnityEditor.EditorApplication.isPlaying : (buttonAttribute.editorModeOnly) ?  UnityEditor.EditorApplication.isPlaying : false))
                        {
                            if (GUILayout.Button(buttonText))
                            {
                                method.Invoke(target, null);
                            }
                        }
                    }
                    
                }
                else
                {
                    if(method.GetCustomAttributes(typeof(DrawButtonAttribute), true).Length > 0)
                    {
                        string warning = typeof(DrawButtonAttribute).Name + " works only on methods with no parameters";
                        EditorGUILayout.HelpBox(warning, MessageType.Warning);
                    }
                }
                
            }
           

            
        // }
        DrawDefaultInspector();
    }

    // public void DrawMethod(UnityEngine.Object target, MethodInfo methodInfo)
    // {
    //     if (methodInfo.GetParameters().Length == 0)
    //     {
    //         ButtonAttribute buttonAttribute = (ButtonAttribute)methodInfo.GetCustomAttributes(typeof(ButtonAttribute), true)[0];
    //         string buttonText = string.IsNullOrEmpty(buttonAttribute.buttonName) ? methodInfo.Name : buttonAttribute.buttonName;

    //         if (GUILayout.Button(buttonText))
    //         {
    //             methodInfo.Invoke(target, null);
    //         }
    //     }
    //     else
    //     {
    //         string warning = typeof(ButtonAttribute).Name + " works only on methods with no parameters";
    //         EditorDrawUtility.DrawHelpBox(warning, MessageType.Warning, context: target);
    //     }
    // }
}
#endif