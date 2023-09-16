using System.Security.Principal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
#endif
public class MonoEvent : MonoBehaviour
{
    public delegate void customEvent();
    public event customEvent onAwake;
    public event customEvent onEnable;
    public event customEvent onStart;
    public event customEvent onDisable;
    public event customEvent onTriggerEnter;
    public event customEvent onTriggerExit;
    public event customEvent onCollisionEnter;
    public event customEvent onCollisionExit;
    private void OnEnable() {
        if(onEnable != null)
            onEnable.Invoke();

        Debug.Log("OnEnable");
    }
    private void Awake() {
        if(onAwake != null)
            onAwake.Invoke();
        Debug.Log("Awake From Mono Event");
    }
    private void Start() {
        if(onStart != null)
            onStart.Invoke();
    }
    private void OnDisable() {
        if(onDisable != null)
            onDisable.Invoke();
    }
    private void OnTriggerEnter(Collider other) {
        if(onTriggerEnter != null)
            onTriggerEnter.Invoke();
    }
    private void OnTriggerExit(Collider other) {
        if(onTriggerExit != null)
            onTriggerExit.Invoke();
    }
    private void OnCollisionEnter(Collision other) {
        if(onCollisionEnter != null)
            onCollisionEnter.Invoke();
    }
    private void OnCollisionExit(Collision other) {
        if(onCollisionExit != null)
            onCollisionExit.Invoke();
    }
    #if UNITY_EDITOR
    // public static bool isOrderSet = true;
    // private void OnValidate() {
    //     if(isOrderSet && !Application.isPlaying)
    //     {
    //         MonoScript monoScript = MonoScript.FromMonoBehaviour(this);
    //         int currentExecutionOrder = MonoImporter.GetExecutionOrder(monoScript);
    //         MonoImporter.SetExecutionOrder(monoScript, 10);

    //         isOrderSet = false;
    //     }
    // }
    #endif
}
