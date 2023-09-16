using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Reflection;
using System;

namespace UnityAutomation
{
    #region UnityEventManager
    [System.Serializable]
    public class AllEventList
    {
        public UnityEngine.Object unityObj;
        [FieldList("unityObj", addEvent: true, disableName: true)]
        public string selectedEvent;
    }
    [System.Serializable]
    public class EventList
    {
        public bool isVariableSet = false;
        public UnityEngine.Object unityObj;
        [FieldList("unityObj", addEvent: true, disableName: true)]
        public string selectedEvent;

        public UnityEvent2[] doAllOfThese;
    }
    #endregion
    #region AllCondition If-Else
    [System.Serializable]
    public class AllCondition
    {
        // public bool isVariableSet = false;
        // && || ( )
        public MultipleCondition AndOR;
        public UnityEngine.Object unityObj;
        [FieldList("unityObj", addField: true, addProperty: true, addFunction: true, addValueReturnType: true, disableName = true)]
        public string selectField;

        public Parameters[] parameters1;
        // is ==, >=, <=, != 
        public Operator @operator;
        public ValueOrGM compareWith;
        public UnityEngine.Object unityObj2;
        [FieldList("unityObj2", addField: true, addProperty: true, addFunction: true, addValueReturnType: true, disableName = true)]
        public string selectField2;

        public Parameters[] parameters2;
        [DrawValue("unityObj", "selectField")]
        public SelectedValue valueCompare;

        // For storage
        //----------First Variable-------
        public FieldInfo selectedField1;
        public PropertyInfo selectedProperty1;
        public MethodInfo selectedMethodInfo1;
        public Type[] methodParamTypes1;
        public Type returnType1;

        //------- Second Variable-----------
        public PropertyInfo selectedProperty2;
        public FieldInfo selectedField2;
        public MethodInfo selectedMethodInfo2;
        public Type[] methodParamTypes2;
        public Type returnType2;
    }
    #endregion

    #region All UnityAction Variable
    //-------------------------Set Value | Call Function | Trigger Event----------------------------------
    [System.Serializable]
    public class UnityEvent2
    {
        // public bool isVariableSet = false;
        public UnityEngine.Object unityObj;
        [FieldList("unityObj", addField: true, addProperty: true, onlyCanWrite: true, addEvent: true, addFunction: true, addValueReturnType: true, addVoidReturnType: true, disableName = true)]
        public string selectField;
        //----
        public ValueOrGM valueOrGM;
        //--
        public UnityEngine.Object unityObj2;
        [FieldList("unityObj2", addField: true, addProperty: true, addFunction: true, addValueReturnType: true, disableName = true)]
        public string selectField2;

        public Parameters[] parameters;
        //--
        [DrawValue("unityObj", "selectField")]
        public SelectedValue setValue;
        //---------------------------- For storage --------------------------
        //--PropertyField
        public FieldInfo selectedField;
        public PropertyInfo selectedProperty;
        //--Func
        public MethodInfo selectedMethodInfo;
        public Type[] methodParamTypes;
        public Type returnType;
        //--Event
        public EventInfo selectedEventInfo;
        public UnityEvent selectedUnityEvent;

        //---------------------------------------------------------------
        //------PropertyField
        public FieldInfo selectedField2;
        public PropertyInfo selectedProperty2;
    }
    #endregion
    //----------------------------All Enum----------------------------------
    public enum ValueOrGM
    {
        Value,
        Field
    }

    public enum MultipleCondition
    {
        NULL,
        AND,
        OR,
        OpenBracket,
        CloseBracket,
        ANDOpenBracket,
        OROpenBracket,
        CloseBracketAND,
        CloseBracketOR,
        CloseBracketANDOpenBracket,
        CloseBracketOROpenBracket
    }
    public enum Operator
    {
        EqualTo,
        NotEqualTo,
        GreaterThan,
        GreaterThanEqualTo,
        LessThan,
        LessThanEqualTo,
    }
    //---------------------------SelectedValue------------------------------------------
    [System.Serializable]
    public class SelectedValue
    {
        public Type valueType;
        public UnityEngine.Object uniObject;
        public string sysObject;
    }
    //----------------------------Function Parameters----------------------------------
    [System.Serializable]
    public class Parameters
    {
        public ValueOrGM giveParameterAs;

        public UnityEngine.Object unityObj;
        [FieldList(unityObj: "unityObj", addField: true, addProperty: true, addFunction: true, addValueReturnType: true, disableName = true)]
        public string selectField;


        [DrawValue(valueType: "methodParamTypes")]
        public SelectedValue[] selectFunctionParameters;

        [DrawValue(valueType: "returnType")]
        public SelectedValue stringArg;

        // For Storage
        public FieldInfo selectedField;
        public PropertyInfo selectedProperty;
        public MethodInfo selectedMethodInfo;
        public Type[] methodParamTypes;
        public Type returnType;
    }
}
