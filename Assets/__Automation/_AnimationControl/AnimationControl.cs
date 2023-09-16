using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif
using UnityAutomation;
using System;
//-------------Enum-----------------------
public enum WhatToUse{
    Frame,
    Time,
    Percentage
}
//---------------------------------------------------------------
//					Animation Conditions Variable
//----------------------------------------------------------------
#region Animation Conditions

[System.Serializable]
public class AnimationConditions{
    [AnimationList("animationComponent")]
    public string animationName;
    public float speed;
    public WrapMode animationWrapMode;
    public WhatToUse use;
    [FromToAttribute("animationComponent","animationName","use",isFrom:true)]
    public float from;
    [FromToAttribute("animationComponent","animationName","use",isTo:true)]
    public float to;
    public UnityEvent2[] whenAnimationStarted;
    public UnityEvent2[] whenAnimationStopped;
}
#endregion
//---------------------------------------------------------------
//				Binding Conditions Variable
//----------------------------------------------------------------

#region Binging Variables
[System.Serializable]
public class BindingEventConditions{
    public float whenValueReaches;
    public bool invokeWhenStateChanges;
    public UnityEvent2[] Do;
}
[System.Serializable]

public class BindingConditions{
    [AnimationList("animationComponent")]
    public string animationName;
    public UnityEngine.Object unityObj;
    // [HideInInspector]
    // public Component component; // Selected Component
    [FieldList("unityObj", addField:true, addProperty:true ,onlyNumeric:true)]
    public string property; // Selected Property
    [HideInInspector]
    public bool isFieldProperty;

    public float minValue, maxValue; // Bind property value from minValue to maxValue
    public WhatToUse use;
    [FromToAttribute("animationComponent","animationName","use",isFrom:true)]
    public float animFromValue;
    [FromToAttribute("animationComponent","animationName","use",isTo:true)]
    public float animToValue; 
    // [HideInInspector]
    public int valueChanges, prevValueChanges;// 1 => value at min | 2 => value at max | 0 => neither
    
    public PropertyInfo selectedProperty;
    public FieldInfo selectedField;
    public System.Object prevPropertyValue,currentValue; // property Value
    public BindingEventConditions[] eventConditions;
}
#endregion
public class AnimationControl : MonoBehaviour {
    [HideInInspector]
    public Animation animationComponent;
    [HideInInspector]
    public float currentAnimationPercentage = 0f;
    // bool isGoingToZero = true;
    [HideInInspector]
    public Animation prevAnimationComponent = null;
    int currentlyPlayingIndex;	
    // [HideInInspector]
    public AnimationConditions[] animationConditions;
    // [HideInInspector]
    public BindingConditions[] bindingConditions;
    

    //==========================================================================================
    //==========================================================================================
    //==========================================================================================
    //==========================================================================================
    private void Start() {
        for (int i = 0; i < animationConditions.Length; i++)
        {
            animationConditions[i].whenAnimationStarted.SetVariable();
            animationConditions[i].whenAnimationStopped.SetVariable();
        }

        for (int i = 0; i < bindingConditions.Length; i++)
        {
            for (int y = 0; y < bindingConditions[i].eventConditions.Length; y++)
            {
                bindingConditions[i].eventConditions[y].Do.SetVariable();
            }

            if(bindingConditions[i].minValue > bindingConditions[i].maxValue)
            {
                // Swap Min Max
                float tempValue = bindingConditions[i].minValue;
                bindingConditions[i].minValue = bindingConditions[i].maxValue;
                bindingConditions[i].maxValue = tempValue;
            }

            string[] splitProperty = bindingConditions[i].property.Split('/');

            string bindingProperty = splitProperty[1];
            if(bindingConditions[i].unityObj != null)
            {
                PropertyInfo bindPropertyInfo =  bindingConditions[i].unityObj.GetType().GetProperty(bindingProperty);

                if( typeof(UnityEngine.GameObject).IsAssignableFrom( bindingConditions[i].unityObj.GetType() ) )
                {
                    bindingConditions[i].selectedProperty = bindPropertyInfo;
                    bindingConditions[i].prevPropertyValue = bindPropertyInfo.GetValue(bindingConditions[i].unityObj,null);
                    bindingConditions[i].isFieldProperty = true;
                }else
                {
                    //property value
                    if(bindPropertyInfo != null)
                    {
                        bindingConditions[i].selectedProperty = bindPropertyInfo;
                        bindingConditions[i].prevPropertyValue = bindPropertyInfo.GetValue(bindingConditions[i].unityObj,null);
                        bindingConditions[i].isFieldProperty = true;
                    }else
                    {
                        FieldInfo bindFieldInfo =  bindingConditions[i].unityObj.GetType().GetField(bindingProperty);
                        bindingConditions[i].selectedField = bindFieldInfo;
                        bindingConditions[i].prevPropertyValue = bindFieldInfo.GetValue(bindingConditions[i].unityObj);
                        bindingConditions[i].isFieldProperty = false;
                    }
                }
            }
            

            // current state of value
            float propertyValue = Convert.ToSingle( bindingConditions[i].prevPropertyValue );
            if(propertyValue == bindingConditions[i].minValue)
            {
                bindingConditions[i].valueChanges = 1; // currently at min
            }else
            if(propertyValue == bindingConditions[i].maxValue)
            {
                bindingConditions[i].valueChanges = 2; // currently at max
            }else
            {
                bindingConditions[i].valueChanges = 0; // currently at neither min nor max
            }

            if(propertyValue < bindingConditions[i].minValue) bindingConditions[i].valueChanges = 1; // currently at min
            if(propertyValue > bindingConditions[i].maxValue) bindingConditions[i].valueChanges = 2; // currently at max

            // bindingCondition.prevValueChanges = bindingCondition.valueChanges;
        }
    }
    // -----------------------------------------
    //    Binding Process
    //------------------------------------------
    bool didMinMaxStateChanges(BindingConditions bindingCondition, float currentValue)
    { 

        if(bindingCondition.prevValueChanges != bindingCondition.valueChanges) 
        {
            bindingCondition.prevValueChanges = bindingCondition.valueChanges;
            return true;
        }
        return false;
    }
    public void _StartBinding(int index)
    {
        if(bindingConditions[index].unityObj == null) return;
        float from = bindingConditions[index].animFromValue, to = bindingConditions[index].animToValue;
        if(bindingConditions[index].use == WhatToUse.Frame)
        {
            from = ConvertToSecond(bindingConditions[index].animationName,from,true);
            to = ConvertToSecond(bindingConditions[index].animationName,to,true);
        }
        if(bindingConditions[index].use == WhatToUse.Percentage)
        {
            from = ConvertToSecond(bindingConditions[index].animationName,from,false,true);
            to = ConvertToSecond(bindingConditions[index].animationName,to,false,true);
        }
        StartCoroutine(BindAnimationControl(bindingConditions[index],from, to, index));
    }
    public void _StopBinding()
    {
        StopAllCoroutines();
        animationComponent.Stop();
        #if UNITY_EDITOR
        currentAnimationPercentage = 0f;
        #endif
    }
    IEnumerator BindAnimationControl(BindingConditions bindingCondition, float from, float to, int index)
    {
        float minValue = bindingCondition.minValue;
        float maxValue = bindingCondition.maxValue;
        System.Object currentPropertyValue;
        bool currentValueIncreasing = false;
        bool? triggeredWhenValueIncreasing = null;

        while(true)
        {
            yield return new WaitForEndOfFrame();

            System.Object prevPropertyValue = bindingCondition.prevPropertyValue;
            
            if(bindingCondition.isFieldProperty)
            {
                currentPropertyValue = bindingCondition.selectedProperty.GetValue(bindingCondition.unityObj,null);
            }else
            {
                currentPropertyValue = bindingCondition.selectedField.GetValue(bindingCondition.unityObj);
                
            }
            if(currentPropertyValue.GetHashCode() != prevPropertyValue.GetHashCode())
            {
                float currentPropValue = Convert.ToSingle( currentPropertyValue );
                
                // Calculate property percentage
                float propertyPercentage = (100f / Mathf.Abs(maxValue - minValue) ) * (currentPropValue - minValue);
                // currentAnimationPercentage = propertyPercentage / 100f;
                

                // Check if value is increasing or decreasing
                float prevPropValue = Convert.ToSingle( prevPropertyValue );
                if(currentPropValue >= prevPropValue) currentValueIncreasing = true;
                else currentValueIncreasing = false;

                // Debug.Log("CurrentPropValue => " + currentPropValue + " | prevPropValue => " + prevPropValue + " | valueIncresing => " + currentValueIncreasing);

                // Trigger Event
                bool triggerEvent = false;
                for (int i = 0; i < bindingCondition.eventConditions.Length; i++)
                {
                    float whenValueReaches = bindingCondition.eventConditions[i].whenValueReaches;
                    
                    if(currentValueIncreasing)
                    {
                        if(whenValueReaches <= currentPropValue && bindingCondition.valueChanges == 1) 
                        {
                            if(!triggeredWhenValueIncreasing ?? true)
                            {
                                triggeredWhenValueIncreasing = true;
                                triggerEvent = true;
                            }
                        }
                        if(currentPropValue >= maxValue) bindingCondition.valueChanges = 2;
                    }else
                    {
                        if(whenValueReaches >= currentPropValue && bindingCondition.valueChanges == 2) 
                        {
                            if(triggeredWhenValueIncreasing ?? true)
                            {
                                triggeredWhenValueIncreasing = false;
                                triggerEvent = true;
                            }
                        }
                        if(currentPropValue <= minValue) bindingCondition.valueChanges = 1;

                    }

                    if(triggerEvent)
                    {
                        if(bindingCondition.eventConditions[i].invokeWhenStateChanges)
                        {
                            bool didStateChange = didMinMaxStateChanges(bindingCondition, currentPropValue);
                            if( didStateChange ) 
                            {
                                bindingCondition.eventConditions[i].Do.Invoke();
                                // #if UNITY_EDITOR
                                // Debug.Log( bindingCondition.GetType().Name + "(" + index + ") => Event Conditions (" + i + ") => Do() Called");
                                // #endif
                            }
                        }else
                        {
                            bindingCondition.eventConditions[i].Do.Invoke();
                            // #if UNITY_EDITOR
                            // Debug.Log( bindingCondition.GetType().Name + "(" + index + ") => Event Conditions (" + i + ") => Do() Called");
                            // #endif
                        }
                    }
                    
                }

                // Play animation only when value is between minValue and maxValue
                if( (currentPropValue >= minValue && currentPropValue <= maxValue) )
                {
                    // calculate animation time using property percentage
                    string animationName = bindingCondition.animationName;
                    animationComponent[animationName].speed = 0;
                    animationComponent[animationName].time = Mathf.Lerp(from, to, propertyPercentage / 100f); // calculate animation time using property percentage
                    animationComponent.CrossFade(animationName);

                    #if UNITY_EDITOR
                    currentAnimationPercentage = (100f/animationComponent[animationName].length * animationComponent[animationName].time) / 100f;
                    #endif
                }

                bindingCondition.prevPropertyValue = currentPropertyValue;
            }
        }
    }
    // -----------------------------------------
    //    Animation Process
    //------------------------------------------
    public void _PlayAnimation(int index)
    {
        _PlayAnimation(index,fromCurrentTime:false);
    }
    public void _PlayAnimationReverse(int index)
    {
        _PlayAnimation(index,fromCurrentTime:false,playReverse:true);
    }
    public void _PlayForwardFromCurrentTime(int index)
    {
        _PlayAnimation(index,fromCurrentTime:true,forward:true);
    }
    public void _PlayBackwardFromCurrentTime(int index)
    {
        float currentTime = animationComponent[animationConditions[index].animationName].time;
        if(currentTime == 0) _PlayAnimation(index, fromCurrentTime: false, playReverse: true);
        _PlayAnimation(index,fromCurrentTime:true,backWard:true);
    }

    public void _PlayAnimation(int index, bool fromCurrentTime, bool forward = false, bool backWard = false, bool playReverse = false)
    {
        StopAllCoroutines();
        // AnimationConditions animCondition = animationConditions[index];
        
        // Change [from] and [to] variable into time
        float from = animationConditions[index].from, to = animationConditions[index].to;
        if(animationConditions[index].use == WhatToUse.Frame)
        {
            from = ConvertToSecond(animationConditions[index].animationName,from, isFrame:true);
            to = ConvertToSecond(animationConditions[index].animationName,to, isFrame:true);
        }
        if(animationConditions[index].use == WhatToUse.Percentage)
        {
            from = ConvertToSecond(animationConditions[index].animationName,from,isPercentage:true);
            to = ConvertToSecond(animationConditions[index].animationName,to,isPercentage:true);
        }

        // Start Playing Animation
        if(fromCurrentTime)
        {
            float currentTime = 0;
            float currentAnimTime = animationComponent[animationConditions[index].animationName].time;

            if (forward && currentAnimTime < from) currentTime = from;
            else if (backWard && currentAnimTime > to) currentTime = to;
            //else if (forward && currentAnimTime == from) return;
            //else if (backWard && currentAnimTime == to) return;
            else    currentTime = currentAnimTime;

            if(forward && currentTime < to) StartCoroutine(PlayAnimation(currentTime,to,index));
            else if(backWard && currentTime > from) StartCoroutine(PlayAnimation(currentTime,from,index));
        }
        else if(playReverse)
        {
            StartCoroutine(PlayAnimation(to,from,index));
        }else
        {
            StartCoroutine(PlayAnimation(from,to,index));
        }
    }
    IEnumerator PlayAnimation(float fromSecond, float toSecond, int index)
    {
        #if UNITY_EDITOR
        // Debug.Log("Animation Started [ " + index + " ]");
        bool isGoingToZero = false;
        #endif

        currentlyPlayingIndex = index;

        // AnimationConditions animCondition = animationConditions[index];

        animationConditions[index].whenAnimationStarted.Invoke();
        
        // Set Speed
        float speed = Mathf.Abs( animationConditions[index].speed );
        if(fromSecond > toSecond) 
        {
            speed *= -1f;
            #if UNITY_EDITOR
            isGoingToZero = true;
            #endif
        }
        
        // Play Animation with pre define parameters
        string animationName = animationConditions[index].animationName;
        animationComponent[animationName].speed = speed;
        animationComponent[animationName].wrapMode = animationConditions[index].animationWrapMode;
        animationComponent[animationName].time = fromSecond;
        animationComponent.Blend(animationName,1f);

        if(animationConditions[index].animationWrapMode == WrapMode.Default || animationConditions[index].animationWrapMode == WrapMode.Once)
        {
        
            // Detect Stop Event
            while (true)
            {
                yield return null;
                #if UNITY_EDITOR
                currentAnimationPercentage = (100f/animationComponent[animationName].length * animationComponent[animationName].time) / 100f;
                #endif
                // #if UNITY_EDITOR
                // Debug.Log(animationComponent[animationName].time + " => " + toSecond);
                // #endif
                if(fromSecond > toSecond){
                if(animationComponent[animationName].time <= toSecond) break;
                }else
                {
                    if(animationComponent[animationName].time >= toSecond) break;
                }
                if(!animationComponent.IsPlaying(animationName) )
                {
                    break;
                } 
            }
            animationConditions[index].whenAnimationStopped.Invoke();
            animationComponent.Stop();

            #if UNITY_EDITOR
            if(isGoingToZero)
            {
                currentAnimationPercentage = 0f;
            }else
            {
                currentAnimationPercentage =  1f;
            }
            // Debug.Log("Animation Stopped [ " + index + " ]");
            #endif
        }
    }
    public void _StopAnimation()
    {
        StopAllCoroutines();
        animationComponent.Stop();
        animationConditions[currentlyPlayingIndex].whenAnimationStopped.Invoke();
        #if UNITY_EDITOR
        currentAnimationPercentage = 0f;
        #endif
    }

    // ==============================================================================================
    float ConvertToSecond(string animationName, float num, bool isFrame = false, bool isPercentage = false)
    {
        if(isFrame)
        {
            float frameRate = animationComponent[animationName].clip.frameRate;
            return (1f / frameRate) * num;
        }
        if(isPercentage)
        {
            float totalTime = animationComponent[animationName].length;
            Mathf.Lerp(0f, totalTime, num/100f);
        }
        return 0;
    }
    
}


// ------------------------------------------------------------------------------
//                               Editor Script
// ------------------------------------------------------------------------------

#if UNITY_EDITOR

//-------------------------------------------------------
//-------------------------------------------------------

[CustomEditor(typeof(AnimationControl))]
public class AnimationControlEditor : Editor
{
    public static int selectedToolbar;
    string[] toolbarString = new string[]{"Animation Control", "Binding Control"};
    // public static GUIStyle groupHeader;
    AnimationControl myScript;
    private void OnEnable() {
        myScript = (AnimationControl)target;
    }
	public override void OnInspectorGUI()
    {
        // DrawDefaultInspector();
        serializedObject.Update ();

        ProgressBar ( myScript.currentAnimationPercentage, "Animation");        

        var animComponent = serializedObject.FindProperty("animationComponent");
        EditorGUILayout.PropertyField(animComponent,new GUIContent("Animation"));

        
        EditorGUI.BeginChangeCheck();
        selectedToolbar = GUILayout.Toolbar(selectedToolbar,toolbarString);
        if(EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            GUI.FocusControl(null);
        }

        EditorGUI.BeginChangeCheck();
        if(selectedToolbar == 0)
        {
            if(myScript.animationComponent)
            {
                if(myScript.animationConditions.Length != 0)
                {
                    EditorPrefs.SetInt("animButtonIndex", EditorGUILayout.IntSlider("Index : ", EditorPrefs.GetInt("animButtonIndex"), 0, myScript.animationConditions.Length-1) );
                
                    GUILayout.BeginHorizontal();
                    if(GUILayout.Button("Play Animation"))  
                    {
                        myScript._PlayAnimation(EditorPrefs.GetInt("animButtonIndex"));
                    }
                    if(GUILayout.Button("Stop Animation"))  
                    {
                        myScript._StopAnimation();
                    }
                    GUILayout.EndHorizontal();
                }
                EditorGUILayout.PropertyField(serializedObject.FindProperty("animationConditions"));
            }
        }
        if(selectedToolbar == 1)
        {
            if(myScript.animationComponent)
            {
                if(myScript.bindingConditions.Length != 0)
                {
                    EditorPrefs.SetInt("bindingButtonIndex", EditorGUILayout.IntSlider("Index : ", EditorPrefs.GetInt("bindingButtonIndex"), 0, myScript.bindingConditions.Length-1) );
                    GUILayout.BeginHorizontal();
                    if(GUILayout.Button("Start Binding")) 
                    {
                        myScript._StartBinding(EditorPrefs.GetInt("animButtonIndex"));
                    }
                    if(GUILayout.Button("Stop Binding"))  
                    {
                        myScript._StopBinding();
                    }
                    GUILayout.EndHorizontal();
                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("bindingConditions"));

            }
        }
        if(EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }
    void ProgressBar (float value, string label)
    {
        // Get a rect for the progress bar using the same margins as a textfield:
        Rect rect = GUILayoutUtility.GetRect (18, 18, EditorStyles.textField);
		
        EditorGUI.ProgressBar (rect, value, label);
        EditorGUILayout.Space ();
    }
}
#endif
