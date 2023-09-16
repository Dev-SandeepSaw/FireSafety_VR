using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityAutomation;

namespace Automation
{
    [DisallowMultipleComponent]
    public class UnityAction : MonoBehaviour
    {

        public AllEventList[] EventList;

        public UnityEvent2[] ChooseYourAction;
        private void Awake()
        {
            // ChooseYourAction.SetVariable();
            EventList.SetVariable(ChooseYourAction);
            // Debug.Log("Awake From UnityAction");
        }
        public void _CallActionAll()
        {
            ChooseYourAction.Invoke();
        }
        public void _CallMultiAction(string indexString)
        {
            ChooseYourAction.InvokeMultipleIndex(indexString);
        }
        public void _CallAction(int index)
        {
            ChooseYourAction[index].Invoke();
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            ChooseYourAction.SetVariable();
        }
#endif
    }

    // --------------------------------------------------------------------------------------------
    //                                    Inspector Editor Script 
    //---------------------------------------------------------------------------------------------
#if UNITY_EDITOR
    [CustomEditor(typeof(UnityAction))]
    [CanEditMultipleObjects]
    public class UnityActionAddButton : Editor
    {
        public int sliderInt = 0;
        public override void OnInspectorGUI()
        {
            UnityAction myScript = (UnityAction)target;
            //myScript.SetChooseYourActionVariable();

            // using (new EditorGUI.DisabledScope(!Application.isPlaying))
            // {
            //     if(!Application.isPlaying)
            //     if (GUILayout.Button("Set Variables"))
            //     {
            //        //myScript.SetChooseYourActionVariable();
            //        Debug.Log(myScript.ChooseYourAction[0].parameters[0].returnType);
            //     }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Call Action : " + sliderInt))
            {
                if (!Application.isPlaying)
                    myScript.ChooseYourAction.SetVariable();
                myScript.ChooseYourAction[sliderInt].Invoke();
            }
            if (GUILayout.Button("Call All Action "))
            {
                if (!Application.isPlaying)
                    myScript.ChooseYourAction.SetVariable();
                myScript.ChooseYourAction.Invoke();
            }
            GUILayout.EndHorizontal();

            if (myScript.ChooseYourAction != null)
                if (myScript.ChooseYourAction.Length > 1)
                    sliderInt = EditorGUILayout.IntSlider("Index", sliderInt, 0, myScript.ChooseYourAction.Length - 1);
            // }

            DrawDefaultInspector();
        }
    }
#endif
}