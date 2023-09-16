#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Linq;
using UnityAutomation;
using Automation;

[CustomPropertyDrawer( typeof( AllCondition ) )]
public class AllConditionDrawer : PropertyDrawer
{
    float height = 0;
    int selectedAndORIndex = 0;
    int selectedOperatorIndex = 0;
    SerializedProperty AndOR , unityObj, selectField, operatorField, compareWith, unityObj2, selectField2, valueCompare, parameters2, parameters1;
    string[] andOrArray = new string[] { "None", "&&", "||", "(", ")", "&&(", "||(" , ")&&", ")||", ")&&(", ")||(" };
    string[] operatorArray = new string[] { "==", "!=", ">", ">=", "<", "<=" };
    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
    {
        // property.serializedObject.Update();
        EditorGUI.BeginChangeCheck();
        EditorGUI.BeginProperty(position, label, property);

        AndOR = property.FindPropertyRelative("AndOR");
        unityObj = property.FindPropertyRelative("unityObj");
        selectField = property.FindPropertyRelative("selectField");
        operatorField = property.FindPropertyRelative("operator");
        compareWith = property.FindPropertyRelative("compareWith");
        unityObj2 = property.FindPropertyRelative("unityObj2");
        selectField2 = property.FindPropertyRelative("selectField2");
        valueCompare = property.FindPropertyRelative("valueCompare");
        parameters2 = property.FindPropertyRelative("parameters2");
        parameters1 = property.FindPropertyRelative("parameters1");
        

        float singleLineHeight = EditorGUIUtility.singleLineHeight;

        Rect newPos = position;
        newPos.x = position.x + 10;
        newPos.height = singleLineHeight;
        newPos.width = Mathf.Lerp(0f, position.width, 0.2f);

        // Index Of Selected AndOR
        selectedAndORIndex = AndOR.enumValueIndex;
        if(selectedAndORIndex > andOrArray.Length || selectedAndORIndex < 0) selectedAndORIndex = 0;

         // Create Popup list
        selectedAndORIndex = EditorGUI.Popup(newPos, "", selectedAndORIndex, andOrArray);
        AndOR.enumValueIndex = selectedAndORIndex;

        // EditorGUI.PropertyField(newPos,AndOR,GUIContent.none);

        // newPos.x -= 10;
        // newPos.y += singleLineHeight;
        newPos.x += newPos.width + 5;
        bool openBracket = false;
        bool closeBracket = false;
        bool noParameter = false;
        // EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            // EditorGUI.PropertyField(newPos,gm, new GUIContent("GameObject"));
            EditorGUI.PropertyField(newPos,unityObj, GUIContent.none);
            if(unityObj.objectReferenceValue != null)
            {
                // newPos.y += singleLineHeight;
                newPos.x += newPos.width + 5;
                newPos.width = Mathf.Lerp(0f, position.width, 0.6f) - 15;
                EditorGUI.PropertyField(newPos,selectField);

                openBracket = selectField.stringValue.Contains("(");
                closeBracket = selectField.stringValue.Contains(")");
                noParameter = selectField.stringValue.Contains("()");
                if(openBracket && closeBracket && !noParameter ){
                    newPos.y += singleLineHeight;
                    newPos.x = position.x + 10;
                    newPos.width = position.width - 5;
                    EditorGUI.PropertyField(newPos,parameters1);
                    newPos.y += 5;
                    
                    MethodInfo methodInfo = null;
                    UnityEngine.Object selectedGM = (UnityEngine.Object)unityObj.objectReferenceValue;
                    ReflectionExtension.putSelectedValue(ref methodInfo,selectedGM,selectField.stringValue);
                    ParameterInfo[] allMethodParameters = methodInfo.GetParameters();
                    parameters1.arraySize = allMethodParameters.Length;
                    for (int i = 0; i < allMethodParameters.Length; i++)
                    {
                        parameters1.GetArrayElementAtIndex(i).
                            SetTargetObjectOfProperty(
                                                        propertyName: "valueType",
                                                        value:allMethodParameters[i].ParameterType,
                                                        insideObject:true
                                                    );
                    }

                    //---------------------------
                    // MethodInfo methodInfo = null;
                    // UnityEngine.Object selectedUnityObj = (UnityEngine.Object)unityObj.objectReferenceValue;
                    // ReflectionExtension.putSelectedValue(ref methodInfo,selectedUnityObj,selectField.stringValue);
                    // if(methodInfo != null)
                    // {
                    //     ParameterInfo[] allMethodParameters = methodInfo.GetParameters();
                    //     parameters1.arraySize = allMethodParameters.Length;

                    //     for (int i = 0; i < allMethodParameters.Length; i++)
                    //     {
                    //         parameters1.GetArrayElementAtIndex(i).
                    //             SetTargetObjectOfProperty(
                    //                                         propertyName: "valueType",
                    //                                         value:allMethodParameters[i].ParameterType,
                    //                                         insideObject:true
                    //                                     );
                    //         // Debug.Log(selectFunctionParameters.GetArrayElementAtIndex(i) + ".valueType => " + allMethodParameters[i].ParameterType);
                    //     }
                    // }
                    // MethodInfo methodInfo = null;
                    // UnityEngine.Object selectedUnityObj2 = (UnityEngine.Object)unityObj2.objectReferenceValue;
                    // ReflectionExtension.putSelectedValue(ref methodInfo,selectedUnityObj2,selectField2.stringValue);
                    // property.SetTargetObjectOfProperty(propertyName:"selectedMethodInfo",insideObject:true,value:methodInfo);
                    // if(methodInfo != null)
                    //     parameters2.arraySize = methodInfo.GetParameters().Length;
                    //--------------------
                }
            }else
            {
                return;
            }            
        // EditorGUILayout.EndVertical();
        if( !(openBracket && closeBracket) || noParameter)
            newPos.y += singleLineHeight;
        else
        {
            newPos.y += EditorGUI.GetPropertyHeight(parameters1);
        }

        // Index Of Selected AndOR
        selectedOperatorIndex = operatorField.enumValueIndex;
        if(selectedOperatorIndex > operatorArray.Length || selectedOperatorIndex < 0) selectedOperatorIndex = 0;

        newPos.x = position.x + 10;
        newPos.height = singleLineHeight;
        newPos.width = Mathf.Lerp(0f, position.width, 0.2f);

        // Create Popup list
        selectedOperatorIndex = EditorGUI.Popup(newPos, "", selectedOperatorIndex, operatorArray);
        operatorField.enumValueIndex = selectedOperatorIndex;

        // EditorGUI.PropertyField(newPos,operatorField,GUIContent.none);

        // newPos.y += singleLineHeight;
        newPos.x += newPos.width;
        EditorGUI.PropertyField(newPos,compareWith,GUIContent.none);

        if(compareWith.enumValueIndex == 1)
        {
            // newPos.y += singleLineHeight;
            newPos.x += newPos.width;
            // EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                // EditorGUI.PropertyField(newPos,gm2, new GUIContent("GameObject"));
                EditorGUI.PropertyField(newPos,unityObj2, GUIContent.none);
                if(unityObj2.objectReferenceValue != null)
                {
                    // newPos.y += singleLineHeight;
                    newPos.x += newPos.width;
                    newPos.width = Mathf.Lerp(0f, position.width, 0.4f) - 5;
                    EditorGUI.PropertyField(newPos,selectField2);

                    bool openBracket2 = selectField2.stringValue.Contains("(");
                    bool closeBracket2 = selectField2.stringValue.Contains(")");
                    bool noParameter2 = selectField2.stringValue.Contains("()");
                    if(openBracket2 && closeBracket2 && !noParameter2){
                        newPos.y += singleLineHeight;
                        newPos.x = position.x + 10;
                        newPos.width = position.width - 5;
                        EditorGUI.PropertyField(newPos,parameters2);

                        MethodInfo methodInfo = null;
                        UnityEngine.Object selectedGM = (UnityEngine.Object)unityObj2.objectReferenceValue;
                        ReflectionExtension.putSelectedValue(ref methodInfo,selectedGM,selectField2.stringValue);
                        ParameterInfo[] allMethodParameters2 = methodInfo.GetParameters();
                        parameters2.arraySize = allMethodParameters2.Length;
                        for (int i = 0; i < allMethodParameters2.Length; i++)
                        {
                            parameters2.GetArrayElementAtIndex(i).
                                SetTargetObjectOfProperty(
                                                            propertyName: "valueType",
                                                            value:allMethodParameters2[i].ParameterType,
                                                            insideObject:true
                                                        );
                        }
                    }
                }
            // EditorGUILayout.EndVertical();
        }else
        {
            // newPos.y += singleLineHeight;
            newPos.x += newPos.width + 5;
            newPos.width = Mathf.Lerp(0f, position.width, 0.6f) - 15;
            EditorGUI.PropertyField(newPos,valueCompare,new GUIContent("Enter Value"));
        }
        EditorGUI.EndProperty();
        EditorGUI.EndChangeCheck();
        property.serializedObject.ApplyModifiedProperties();
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // height = base.GetPropertyHeight(property,label);
        height = 0;
        height += getHeight(property, "AndOR");
        // height += getHeight(property, "gm");
        if(property.FindPropertyRelative("unityObj").objectReferenceValue != null)
        {
            // height += getHeight(property, "selectField");

            bool openBracket = property.FindPropertyRelative("selectField").stringValue.Contains("(");
            bool closeBracket = property.FindPropertyRelative("selectField").stringValue.Contains(")");
            bool noParameter = property.FindPropertyRelative("selectField").stringValue.Contains("()");
            if( openBracket && closeBracket && !noParameter){
                height += getHeight(property, "parameters1");
            }

            height += getHeight(property, "operator");
            // height += getHeight(property, "compareWith");
            if(property.FindPropertyRelative("compareWith").enumValueIndex == 1)
            {
                // height += getHeight(property, "gm2");
                if(property.FindPropertyRelative("unityObj2").objectReferenceValue != null)
                {
                    // height += getHeight(property, "selectField2");
                    bool openBracket2 = property.FindPropertyRelative("selectField2").stringValue.Contains("(");
                    bool closeBracket2 = property.FindPropertyRelative("selectField2").stringValue.Contains(")");
                    bool noParameter2 = property.FindPropertyRelative("selectField2").stringValue.Contains("()");
                    if(openBracket2 && closeBracket2 && !noParameter2){
                        // Debug.Log("Both Are True");
                        height += getHeight(property, "parameters2");
                    }
                }
            }else
            {
                // height += getHeight(property, "valueCompare");
            }
        }
        return height + 5;
    }

    public float getHeight(SerializedProperty property, string name)
    {
        return EditorGUI.GetPropertyHeight(property.FindPropertyRelative(name));
    }
}
#endif
