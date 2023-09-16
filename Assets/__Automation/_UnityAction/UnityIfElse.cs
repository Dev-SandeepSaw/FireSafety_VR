using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using UnityEngine.Events;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityAutomation;

namespace Automation
{

    [DisallowMultipleComponent]
    public class UnityIfElse : MonoBehaviour
    {
        public AllEventList[] EventList;

        public AllCondition[] IfElseCondition;

        //--------------If True ---------------------------

        public UnityEvent2[] whenTrue;
        //--------------If False ---------------------------

        public UnityEvent2[] whenFalse;
        private void Awake()
        {
            // IfElseCondition.SetIfElseConditionVariable();
            // whenTrue.SetVariable();
            // whenFalse.SetVariable();
            EventList.SetVariable(null, this);

        }
        public void _RunUnityIfElse()
        {
            bool result = IfElseCondition.checkAllCondition();
            if (result)
            {
                whenTrue.Invoke();
            }
            else
            {
                whenFalse.Invoke();
            }
        }
        public void _RunUnityIfElseTrue()
        {
            bool result = IfElseCondition.checkAllCondition();
            if (result)
            {
                whenTrue.Invoke();
            }
        }
        public void _RunUnityIfElseFalse()
        {
            bool result = IfElseCondition.checkAllCondition();
            if (!result)
            {
                whenFalse.Invoke();
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            // IfElseCondition.SetIfElseConditionVariable();

            // MonoScript monoScript = MonoScript.FromMonoBehaviour(this);
            // int currentExecutionOrder = MonoImporter.GetExecutionOrder(monoScript);
            // MonoImporter.SetExecutionOrder(monoScript, -10);
        }
        public void Check(int index)
        {
            IfElseCondition.SetIfElseConditionVariable();
            IfElseCondition.checkAllCondition();
        }
#endif
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(UnityIfElse))]
    [CanEditMultipleObjects]
    public class UnityIfElseAddButton : Editor
    {
        string[] andOrArray = new string[] { " ", "&&", "||", "(", ")", "&& (", "|| (", ") &&", ") ||", ") && (", ") || (" };

        public override void OnInspectorGUI()
        {
            UnityIfElse myScript = (UnityIfElse)target;

            // Display Summery of all Conditions 
            string textToDispaly = "";
            bool openBracketFound = false;

            if (myScript.IfElseCondition == null) { DrawDefaultInspector(); return; }
            for (int i = 0; i < myScript.IfElseCondition.Length; i++)
            {
                MultipleCondition andor = myScript.IfElseCondition[i].AndOR;
                textToDispaly += andOrArray[(int)andor] + "  " + i + "  ";

                if (andOrArray[(int)andor].Contains("(")) openBracketFound = true;
                if (andOrArray[(int)andor].Contains(")")) openBracketFound = false;
            }
            if (openBracketFound) textToDispaly += " )";
            EditorGUILayout.HelpBox(textToDispaly, MessageType.None);


            EditorGUILayout.BeginHorizontal();
            // if(GUILayout.Button("Check"))
            // {
            // 	myScript.Check(0);
            // }
            if (GUILayout.Button("_RunUnityIFElse"))
            {
                // if(!Application.isPlaying)
                //     myScript.IfElseCondition.SetIfElseConditionVariable();
                myScript._RunUnityIfElse();
                // myScript.IfElseCondition[0].isVariableSet = false;
                // myScript.whenTrue[0].isVariableSet = false;
                // myScript.whenFalse[0].isVariableSet = false;
            }
            if (GUILayout.Button("_RunUnityIFElseTrue"))
            {
                // if(!Application.isPlaying)
                //     myScript.IfElseCondition.SetIfElseConditionVariable();
                myScript._RunUnityIfElseTrue();
                // myScript.IfElseCondition[0].isVariableSet = false;
                // myScript.whenTrue[0].isVariableSet = false;
                // myScript.whenFalse[0].isVariableSet = false;
            }
            if (GUILayout.Button("_RunUnityIFElseFalse"))
            {
                // if(!Application.isPlaying)
                //     myScript.IfElseCondition.SetIfElseConditionVariable();
                myScript._RunUnityIfElseFalse();
                // myScript.IfElseCondition[0].isVariableSet = false;
                // myScript.whenTrue[0].isVariableSet = false;
                // myScript.whenFalse[0].isVariableSet = false;
            }
            EditorGUILayout.EndHorizontal();
            DrawDefaultInspector();
        }
    }
#endif
}