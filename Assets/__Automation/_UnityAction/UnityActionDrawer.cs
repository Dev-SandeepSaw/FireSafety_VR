#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using UnityAutomation;
using Automation;
// ======================================================================================
//                                  Function Parameters
// ======================================================================================

[CustomPropertyDrawer( typeof( Parameters ) )]
public class ParametersDrawer : PropertyDrawer
{
    float height = 0;
    SerializedProperty giveParameterAs, selectFunctionParameters, unityObj, selectField, stringArg;
    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
    {
        giveParameterAs = property.FindPropertyRelative("giveParameterAs");
        unityObj = property.FindPropertyRelative("unityObj");
        selectField = property.FindPropertyRelative("selectField");
        selectFunctionParameters = property.FindPropertyRelative("selectFunctionParameters");
        stringArg = property.FindPropertyRelative("stringArg");        

        float singleLineHeight = EditorGUIUtility.singleLineHeight;

        EditorGUI.BeginChangeCheck();
        EditorGUI.BeginProperty(position, label, property);

        Rect newPos = position;
        newPos.x = position.x + 5;
        newPos.height = singleLineHeight;
        // Debug.Log(Mathf.InverseLerp(0f, position.width, 30f));
        newPos.width = Mathf.Lerp(0f, position.width, 0.2f); //position.width / 4;
        // EditorGUI.PropertyField(newPos, giveParameterAs, new GUIContent("Give Parameters As"));
        EditorGUI.PropertyField(newPos, giveParameterAs, GUIContent.none);
        // newPos.y += singleLineHeight;
        
        if(giveParameterAs.enumValueIndex == 0)
        {
            MethodInfo methodInfo = null;
            // UnityEngine.Object selectedUnityObj = (UnityEngine.Object) property.GetTargetObjectOfProperty(propertyName:"unityObj");
            // string selectedField = (string) property.GetTargetObjectOfProperty(propertyName:"selectField");
            // Debug.Log(selectedField);
            // ReflectionExtension.putSelectedValue(ref methodInfo,selectedUnityObj,selectedField);
            
            methodInfo = (MethodInfo) property.GetTargetObjectOfProperty(propertyName:"selectedMethodInfo");
            if(methodInfo != null)
            {
                ParameterInfo[] allMethodParameters = methodInfo.GetParameters();
                IEnumerable parametersFields = property.GetTargetObjectOfProperty(propertyName:"parameters") as IEnumerable;

                // for (int i = 0; i < allMethodParameters.Length; i++)
                // {
                    if (parametersFields != null)
                    {
                        int count = 0;
                        foreach (Parameters element in parametersFields)
                        {
                            if(count > allMethodParameters.Length - 1) break;
                            element.stringArg.valueType = allMethodParameters[count].ParameterType;
                            count++;
                        }
                    }
                // }
            }

            newPos.x += newPos.width + 5;
            newPos.width = position.width - newPos.width - 5;
            EditorGUI.PropertyField(newPos,stringArg,new GUIContent("Enter Value"));
            newPos.y += singleLineHeight;
        }else
        {
            newPos.x += newPos.width + 5;
            // newPos.width = position.width - newPos.width - 5;
            // EditorGUI.PropertyField(newPos, gm, new GUIContent("GameObject"));
            EditorGUI.PropertyField(newPos, unityObj, GUIContent.none);
            if(unityObj.objectReferenceValue != null)
            {
                // newPos.y += singleLineHeight;
                newPos.x += newPos.width + 5;
                newPos.width = Mathf.Lerp(0f, position.width, 0.6f) - 10;
                EditorGUI.PropertyField(newPos,selectField);

                bool openBracket = selectField.stringValue.Contains("(");
                bool closeBracket = selectField.stringValue.Contains(")");
                bool noParameter = selectField.stringValue.Contains("()");
                
                if(openBracket && closeBracket && !noParameter){
                    newPos.y += singleLineHeight;
                    // Parameters[] allParameter = (Parameters) parameters.GetTargetObjectOfProperty(returnObject:true);

                    MethodInfo methodInfo = null;
                    UnityEngine.Object selectedUnityObj = (UnityEngine.Object)unityObj.objectReferenceValue;
                    ReflectionExtension.putSelectedValue(ref methodInfo,selectedUnityObj,selectField.stringValue);
                    if(methodInfo != null)
                    {
                        ParameterInfo[] allMethodParameters = methodInfo.GetParameters();
                        selectFunctionParameters.arraySize = allMethodParameters.Length;

                        for (int i = 0; i < allMethodParameters.Length; i++)
                        {
                            selectFunctionParameters.GetArrayElementAtIndex(i).
                                SetTargetObjectOfProperty(
                                                            propertyName: "valueType",
                                                            value:allMethodParameters[i].ParameterType,
                                                            insideObject:true
                                                        );
                            // Debug.Log(selectFunctionParameters.GetArrayElementAtIndex(i) + ".valueType => " + allMethodParameters[i].ParameterType);
                        }
                    }

                    newPos.x = position.x + 5;
                    newPos.width = position.width ;
                    EditorGUI.PropertyField(newPos,selectFunctionParameters); 
                }           
            }else
            {
                return;
            }     
        }
        EditorGUI.EndProperty();
        EditorGUI.EndChangeCheck();
        property.serializedObject.ApplyModifiedProperties();   
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // height = base.GetPropertyHeight(property,label);
        height = 0;
        
        height += getHeight(property, "giveParameterAs");
        if(property.FindPropertyRelative("giveParameterAs").enumValueIndex == 0)
        {
            // height += getHeight(property, "stringArg");
        }else
        {
            // height += getHeight(property, "gm");
            if(property.FindPropertyRelative("unityObj").objectReferenceValue != null)
            {
                // height += getHeight(property, "selectField");
                bool openBracket = property.FindPropertyRelative("selectField").stringValue.Contains("(");
                bool closeBracket = property.FindPropertyRelative("selectField").stringValue.Contains(")");
                bool noParameter2 = property.FindPropertyRelative("selectField").stringValue.Contains("()");

                if(openBracket && closeBracket && !noParameter2){
                    height += getHeight(property, "selectFunctionParameters");
                }

            }
        }        
        return height + 5;

        // return EditorGUI.GetPropertyHeight(property,true);
    }
    public float getHeight(SerializedProperty property, string name)
    {
        return EditorGUI.GetPropertyHeight(property.FindPropertyRelative(name));
    }
}
// ======================================================================================
//                                All-in-One  Event Field Property
// ======================================================================================

[CustomPropertyDrawer( typeof( UnityEvent2 ) )]
public class UnityEvent2Drawer : PropertyDrawer
{
    float height = 0;
    SerializedProperty unityObj, unityObj2, selectField, selectField2, parameters2, valueOrGM, setValue;
    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
    {
        unityObj = property.FindPropertyRelative("unityObj");
        unityObj2 = property.FindPropertyRelative("unityObj2");
        selectField = property.FindPropertyRelative("selectField");
        selectField2 = property.FindPropertyRelative("selectField2"); 
        valueOrGM = property.FindPropertyRelative("valueOrGM"); 
        setValue = property.FindPropertyRelative("setValue");    
        parameters2 = property.FindPropertyRelative("parameters");    

        float singleLineHeight = EditorGUIUtility.singleLineHeight;
        
        // property.serializedObject.Update();
        EditorGUI.BeginChangeCheck();
        EditorGUI.BeginProperty(position, label, property);

        Rect newPos = position;
        newPos.x += 10;
        newPos.height = singleLineHeight;
        newPos.width = position.width / 4;
        //EditorGUI.PropertyField(newPos, gm, new GUIContent("Game Object"));
        EditorGUI.PropertyField(newPos, unityObj, GUIContent.none);

        //Restore
        //newPos.y += singleLineHeight;

        bool openBracket = false ;
        bool closeBracket = false ;
        bool noParameter = false;

        if (unityObj.objectReferenceValue != null)
        {
            //-------Start Selected Field--------------
            openBracket = selectField.stringValue.Contains("(");
            if (openBracket)
            {
                newPos.x += newPos.width;
                newPos.width = position.width - newPos.width - 10;
            }
            else
            {
                newPos.x += newPos.width;
                newPos.width = newPos.width * 2;
            }
            //-------End Selected Field--------------

            //EditorGUI.PropertyField(newPos,selectField,new GUIContent("Select Field"));
            // if(selectField == null) Debug.Log(property);
            EditorGUI.PropertyField(newPos,selectField, GUIContent.none); 

            //newPos.y += singleLineHeight;
            //newPos.width *= 4;
            //newPos.x -= newPos.width - 10;


            openBracket = selectField.stringValue.Contains("(");
            closeBracket = selectField.stringValue.Contains(")");
            noParameter = selectField.stringValue.Contains("()");

            if (openBracket && closeBracket && !noParameter){
                // newPos.y += singleLineHeight;
                // Parameters[] allParameter = (Parameters) parameters.GetTargetObjectOfProperty(returnObject:true);

                MethodInfo methodInfo = null;
                UnityEngine.Object selectedUnityObj = (UnityEngine.Object)unityObj.objectReferenceValue;
                ReflectionExtension.putSelectedValue(ref methodInfo,selectedUnityObj,selectField.stringValue);
                property.SetTargetObjectOfProperty(propertyName:"selectedMethodInfo",insideObject:true,value:methodInfo);
                if(methodInfo != null)
                    parameters2.arraySize = methodInfo.GetParameters().Length;

                newPos.y += singleLineHeight;
                newPos.x = position.x + 10;
                newPos.width = position.width ;
                EditorGUI.PropertyField(newPos,parameters2); 
            }else if(noParameter){
                return;
            }
            else
            {
                EventInfo eventInfo = null;
                UnityEngine.Events.UnityEvent unityEvent = null;
                ReflectionExtension.putSelectedValue(ref eventInfo, (UnityEngine.Object)unityObj.objectReferenceValue, selectField.stringValue); 
                ReflectionExtension.putSelectedValue(ref unityEvent, (UnityEngine.Object)unityObj.objectReferenceValue, selectField.stringValue); 

                if(eventInfo != null || unityEvent != null) return;

                //EditorGUI.PropertyField(newPos,valueOrGM,new GUIContent("Set With"));
                newPos.x += newPos.width;
                newPos.width = position.width / 4 - 5;
                EditorGUI.PropertyField(newPos,valueOrGM, GUIContent.none);

                // Restore
                newPos.x = position.x + 10;
                newPos.y += singleLineHeight;
                newPos.width = position.width - 5;

                if (valueOrGM.enumValueIndex == 0)
                {
                    //EditorGUI.PropertyField(newPos,setValue,new GUIContent("Enter Value"));
                    newPos.x += position.width / 4;
                    newPos.width = position.width / 2 + position.width / 4 - 5;
                    EditorGUI.PropertyField(newPos,setValue, GUIContent.none);
                }else
                {
                    //EditorGUI.PropertyField(newPos, gm2, new GUIContent("GameObject"));
                    // newPos.x += 10;
                    newPos.height = singleLineHeight;
                    newPos.width = position.width / 4;
                    EditorGUI.PropertyField(newPos, unityObj2, GUIContent.none);
                    if(unityObj2.objectReferenceValue != null)
                    {
                        //newPos.y += singleLineHeight;
                        newPos.x += newPos.width;
                        newPos.width = position.width / 2 + position.width / 4 - 5;
                        EditorGUI.PropertyField(newPos,selectField2);

                        openBracket = selectField2.stringValue.Contains("(");
                        closeBracket = selectField2.stringValue.Contains(")");
                        noParameter = selectField2.stringValue.Contains("()");
                        
                        if(openBracket && closeBracket && !noParameter){
                            newPos.y += singleLineHeight;
                            // Parameters[] allParameter = (Parameters) parameters.GetTargetObjectOfProperty(returnObject:true);

                            MethodInfo methodInfo = null;
                            UnityEngine.Object selectedUnityObj2 = (UnityEngine.Object)unityObj2.objectReferenceValue;
                            ReflectionExtension.putSelectedValue(ref methodInfo,selectedUnityObj2,selectField2.stringValue);
                            property.SetTargetObjectOfProperty(propertyName:"selectedMethodInfo",insideObject:true,value:methodInfo);
                            if(methodInfo != null)
                                parameters2.arraySize = methodInfo.GetParameters().Length;

                            newPos.x = position.x + 10;
                            newPos.width = position.width - 5;
                            EditorGUI.PropertyField(newPos,parameters2); 
                        }           
                    }else
                    {
                        return;
                    }     
                }
            }           
            
        }
        EditorGUI.EndProperty();
        EditorGUI.EndChangeCheck();
        property.serializedObject.ApplyModifiedProperties();   
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // height = base.GetPropertyHeight(property,label);
        height = 0;
        
        height += getHeight(property, "unityObj");
        SerializedProperty serGM = property.FindPropertyRelative("unityObj");
        SerializedProperty serSelectField ;
        if(serGM.objectReferenceValue != null)
        {
            //height += getHeight(property, "selectField");

            serSelectField = property.FindPropertyRelative("selectField");
            bool openBracket = serSelectField.stringValue.Contains("(");
            bool closeBracket = serSelectField.stringValue.Contains(")");
            bool noParameter = serSelectField.stringValue.Contains("()");

            if(openBracket && closeBracket && !noParameter)
            {
                height += getHeight(property, "parameters");
            }
            else if(!noParameter)
            {
                EventInfo eventInfo = null;
                UnityEngine.Events.UnityEvent unityEvent = null;
                ReflectionExtension.putSelectedValue(ref eventInfo, (UnityEngine.Object)serGM.objectReferenceValue, serSelectField.stringValue); 
                ReflectionExtension.putSelectedValue(ref unityEvent, (UnityEngine.Object)serGM.objectReferenceValue, serSelectField.stringValue); 

                if(eventInfo == null && unityEvent == null) {
                    //height += getHeight(property, "valueOrGM");
                    if(property.FindPropertyRelative("valueOrGM").enumValueIndex == 0)
                    {
                        height += getHeight(property, "setValue");
                    }else
                    {
                        height += getHeight(property, "unityObj2");
                        if(property.FindPropertyRelative("unityObj2").objectReferenceValue != null)
                        {
                            //height += getHeight(property, "selectField2");
                            openBracket = property.FindPropertyRelative("selectField2").stringValue.Contains("(");
                            closeBracket = property.FindPropertyRelative("selectField2").stringValue.Contains(")");
                            noParameter = property.FindPropertyRelative("selectField2").stringValue.Contains("()");

                            if(openBracket && closeBracket && !noParameter)
                                height += getHeight(property, "parameters");
                        }
                    }    
                }                 
            }
        }
        
        return height + 3;

        // return EditorGUI.GetPropertyHeight(property,true);
    }
    public float getHeight(SerializedProperty property, string name)
    {
        if(property.FindPropertyRelative(name) == null) return EditorGUIUtility.singleLineHeight;
        return EditorGUI.GetPropertyHeight(property.FindPropertyRelative(name));
    }
}
#endif