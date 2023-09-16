using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAutomation;
using System.Reflection;
using System;
using System.Linq;
using UnityEngine.Events;
public static class EventListExtension
{
    public class UnityEvent1Type<T> : UnityEvent<T>
    {
    }
    public static void SetVariable(this EventList[] ChooseEvents)
    {
        for (int i = 0; i < ChooseEvents.Length; i++)
        {
            EventInfo eventInfo = null;

            ReflectionExtension.putSelectedValue(ref eventInfo, ChooseEvents[i].unityObj, ChooseEvents[i].selectedEvent);
            if(eventInfo != null)
            {
                IEnumerable<MethodInfo> extmethod = ReflectionExtension.GetExtensionMethods(typeof(UnityEvent2[]).Assembly,typeof(UnityEvent2[]),"Invoke");
                Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, 
                                     ChooseEvents[i].doAllOfThese, 
                                     extmethod.First());
                eventInfo.AddEventHandler(ChooseEvents[i].unityObj, handler);
            }else
            {
                UnityEvent selectedUnityEvent = null;
                System.Object obj = null;
                List<Type> types = new List<Type>();
                ReflectionExtension.putSelectedValue(ref selectedUnityEvent, ref obj, ref types, ChooseEvents[i].unityObj, ChooseEvents[i].selectedEvent);
                
                if(selectedUnityEvent != null)
                {
                    int index = i;
                    selectedUnityEvent.AddListener(delegate{ChooseEvents[index].doAllOfThese.Invoke();});
                }else{
                    Type type = types[0];


                    UnityEvent1Type<float> firstType = new UnityEvent1Type<float>();
                    // firstType = (UnityEvent1Type<float>) obj;
                    Debug.Log(firstType);
                    // methodInfo.Invoke(delegate{ChooseEvents[index].doAllOfThese.Invoke();} , null)
                    // Debug.Log("SelectedUnityEvent Is NULL => " + methodInfo.ToString());
                }
                
                // PropertyInfo propertyInfo = null;
                // ReflectionExtension.putSelectedValue(ref propertyInfo, ChooseEvents[i].unityObj, ChooseEvents[i].selectedEvent);
                // if(propertyInfo != null)
                // {
                //     System.Object value = propertyInfo.GetValue(ChooseEvents[i].unityObj, null);
                //     if( typeof(UnityEngine.Events.UnityEvent).IsAssignableFrom( value.GetType() ) )
                //     {
                //         UnityEvent unityEvent = (UnityEvent) value;
                //         int index = i;
                //         unityEvent.AddListener(delegate{ChooseEvents[index].doAllOfThese.Invoke();});
                //     }
                // }else
                // {
                //     FieldInfo fieldInfo = null;
                //     ReflectionExtension.putSelectedValue(ref fieldInfo, ChooseEvents[i].unityObj, ChooseEvents[i].selectedEvent);
                //     if(fieldInfo != null)
                //     {
                //         System.Object value = fieldInfo.GetValue(ChooseEvents[i].unityObj);
                //         if( typeof(UnityEngine.Events.UnityEvent).IsAssignableFrom( value.GetType() ) )
                //         {
                //             UnityEvent unityEvent = (UnityEvent) value;
                //             int index = i;
                //             unityEvent.AddListener(delegate{ChooseEvents[index].doAllOfThese.Invoke();});
                //         }
                //     }
                // }
            }
            ChooseEvents[i].doAllOfThese.SetVariable();
        }
    }
    public static void SetVariable(this AllEventList[] EventList, UnityEvent2[] unityEvent2 = null, Automation.UnityIfElse unityIfElse = null)
    {
        for (int i = 0; i < EventList.Length; i++)
        {
            EventInfo eventInfo = null;

            ReflectionExtension.putSelectedValue(ref eventInfo, EventList[i].unityObj, EventList[i].selectedEvent);
            if(eventInfo != null)
            {
                System.Object firstArg;
                MethodInfo extmethod = null;

                if (unityEvent2 != null){
                    extmethod = ReflectionExtension.GetExtensionMethods(typeof(UnityEvent2[]).Assembly,typeof(UnityEvent2[]),"Invoke").First();
                    firstArg = unityEvent2;
                } 
                else {
                    extmethod = unityIfElse.GetType().GetMethod("_RunUnityIfElse");
                    firstArg = unityIfElse;
                }
                
                Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, 
                                     firstArg, 
                                     extmethod);
                eventInfo.AddEventHandler(EventList[i].unityObj, handler);
            }else
            {
                UnityEvent selectedUnityEvent = null;
                ReflectionExtension.putSelectedValue(ref selectedUnityEvent, EventList[i].unityObj, EventList[i].selectedEvent);
                
                if(selectedUnityEvent != null)
                {
                    if (unityEvent2 != null) selectedUnityEvent.AddListener(unityEvent2.Invoke);
                    else selectedUnityEvent.AddListener(unityIfElse._RunUnityIfElse);
                    
                }
                // PropertyInfo propertyInfo = null;
                // ReflectionExtension.putSelectedValue(ref propertyInfo, EventList[i].unityObj, EventList[i].selectedEvent);
                // if(propertyInfo != null)
                // {
                //     System.Object value = propertyInfo.GetValue(EventList[i].unityObj, null);
                //     if( typeof(UnityEngine.Events.UnityEvent).IsAssignableFrom( value.GetType() ) )
                //     {
                //         UnityEvent unityEvent = (UnityEvent) value;
                //         unityEvent.AddListener(unityEvent2.Invoke);
                //     }
                // }else
                // {
                //     FieldInfo fieldInfo = null;
                //     ReflectionExtension.putSelectedValue(ref fieldInfo, EventList[i].unityObj, EventList[i].selectedEvent);
                //     if(fieldInfo != null)
                //     {
                //         System.Object value = fieldInfo.GetValue(EventList[i].unityObj);
                //         if( typeof(UnityEngine.Events.UnityEvent).IsAssignableFrom( value.GetType() ) )
                //         {
                //             UnityEvent unityEvent = (UnityEvent) value;
                //             unityEvent.AddListener(unityEvent2.Invoke);
                //         }
                //     }
                // }
            }
        }
    }

    public static void SetVariable(this AllEventList[] EventList, MonoBehaviour mono, string functionName)
    {
        for (int i = 0; i < EventList.Length; i++)
        {
            EventInfo eventInfo = null;
            ReflectionExtension.putSelectedValue(ref eventInfo, EventList[i].unityObj, EventList[i].selectedEvent);
            if (eventInfo != null)
            {
                try
                {
                    MethodInfo extmethod = mono.GetType().GetMethod(functionName);
                    System.Object firstArg = mono;
                    Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, firstArg, extmethod);
                    eventInfo.AddEventHandler(EventList[i].unityObj, handler);
                }
                catch (System.Exception e)
                {
                    Debug.Log(e.Message);
                }
            }
        }
    }
}
