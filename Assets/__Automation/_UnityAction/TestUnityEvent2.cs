using UnityEngine;
using UnityEngine.Events;
using UnityAutomation;
using UnityEngine.Profiling;
using UnityEditor;
public class TestUnityEvent2 : MonoBehaviour
{
    public UnityEvent2[] myUnityEvent;
    public UnityEvent unityEvent;
    private void Awake() {
        myUnityEvent.SetVariable();        
    }
    // private void Update() {
    //     Profiler.BeginSample("UnityEvent2");
    //     myUnityEvent.Invoke();
    //     Profiler.EndSample();

    //     Profiler.BeginSample("UnityEvent");
    //     unityEvent.Invoke();
    //     Profiler.EndSample();

    //     // EditorApplication.isPaused = true;

    //     if(Time.time >= 10f)
    //     {
    //         // Profiler.EndSample();
    //         EditorApplication.isPaused = true;
    //     }
    // }

    [DrawButton("Call Both Event")]
    public void CallBothEvent() {
        myUnityEvent.Invoke();
        unityEvent.Invoke();
    }
}