using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityAutomation;
using System;
[Serializable]
public class DriveUnityEvent : UnityEvent<float>
{
}
public class ActionTest : MonoBehaviour
{
    //[DrawValue(valueType: "methodParamTypes")]
    //public SelectedValue[] selectFunctionParameters;

    //public System.Type[] methodParamTypes;
    public Component myComponent;
    public double myDouble;
    public ulong myLongDouble;
    public Color myColor;
    public Color32 myColor32;
    public int myInt;
    public bool myBool;
    public string myString;
    public int[] myIntArr;
    public int GetMyInt{get;}
    public float myFloat;
    public float GetSetMyFloat{get; set;}
    public UnityEvent unityEvent;
    public delegate void customEvent();
    public event customEvent systemEvent;
    public BoxCollider boxCollider;
    public GameObject gm;

    public DriveUnityEvent ValueChanged = new DriveUnityEvent();

    [System.Serializable]
    public class ActionClass{
        public int myActionInt;
       
        public int[] myActionIntArr;
        public int GetMyActionInt{get;}
        public float myActionFloat;
        public float GetSetMyActionFloat{get; set;}
        public UnityEvent myActionUnityEvent;
        public void myFuncInsideClass()
        {
            
        }
        public void myFuncInsideClass(int a, int b)
        {

        }
        public int myFuncInsideClass2(int a, int b)
        {
            return a+b;
        }
    }
    
    public ActionClass[] actionClass;

    public void func()
    {
        Debug.Log("From ActionTest => Func()");
    }
    public void funcGameObject(GameObject gm)
    {
        Debug.Log("From ActionTest => " + gm.name);
    }
    public void funcComponent(Component comp)
    {
        Debug.Log("From ActionTest => " + comp.GetType());
    }
    public void funcMulti(int a, int b)
    {
        Debug.Log(a+b);
    }
    public int funcInt()
    {
        return 20;
    }
    public int funcInt(int a,int b)
    {
        return a+b;
    }
    public string funcString()
    {
        return "Hello World";
    }
    public int funcIntMulti(int a,int b)
    {
        return a+b;
    }
    public void checkUnityEvent(){
        Debug.Log("UnityEventCalled");
    }
    public int AddHash(Transform trans, BoxCollider col)
    {
        Debug.Log( trans.GetHashCode() + col.GetHashCode() );
        return (trans.GetHashCode() + col.GetHashCode());
    }
    public int printHash(Transform trans)
    {
        Debug.Log(trans.GetHashCode());
        return trans.GetHashCode();
    }
    public void printNum(int num)
    {
        Debug.Log(num);
    }
    [DrawButton("Call BothEvent")]
    public void Awake() {
        systemEvent += func;
        // unityEvent.AddListener(checkUnityEvent);

        systemEvent.Invoke();
        unityEvent.Invoke();

        // ValueChanged.AddListener(  delegate{Debug.Log("Hello");}  );
        // ValueChanged.Invoke(0);

    }
}
