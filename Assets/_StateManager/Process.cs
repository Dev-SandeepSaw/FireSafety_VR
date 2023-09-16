using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAutomation;
using System;
namespace Automation
{
    public class Process : MonoBehaviour
    {
        #region Variables
        public bool ignoreGuidedMode = false;
        public static bool isGuidedActive = false;
        public string ProcessName;
        public string activeSubProcessName;
        public event Action<Process> valueChanged;
        public event Action valueChanged2;
        public event Action<Process> isActiveChanged;
        public event Action<Process> resetCalled;

        public event Action<Process> finished;

        [Range(0f, 100f)]
        [SerializeField]
        private float currentPercentage;
        public float CurrentPercentage
        {
            get
            {
                return currentPercentage;
            }
            set
            {
                float valueToBeChanged = value;

                if (valueToBeChanged != currentPercentage)
                {
                    currentPercentage = value;
                    if (valueChanged != null) valueChanged.Invoke(this);
                    if (valueChanged2 != null) valueChanged2.Invoke();
                    percentageChanged();

                    Debug.Log(ProcessName + " - Value => " + value);
                }
            }
        }
        [SerializeField]
        private bool isActive = false;
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                if (isActive != value)
                {
                    isActive = value;
                    // Call Action Event
                    if (isActiveChanged != null)
                        isActiveChanged.Invoke(this);
                }
            }
        }
        public int noOfTimesMin = 0;
        public int noOfTimesMax = 0;
        private bool? isAtMax = false;
        private bool? prevIsAtMax = false;

        // Depend On Process

        public List<Process> DependOnProcess = new List<Process>();

        // Enable Disable Condition
        // [ReorderableList(noFoldout = true)]
        public AllCondition[] enableCondition;
        // [ReorderableList(noFoldout = true)]
        public AllCondition[] disableCondition;


        // What Happen When Enable or Disable
        // [ReorderableList(noFoldout = true)]
        [Header("0%")]
        public UnityEvent2[] whenEnable;
        // [ReorderableList(noFoldout = true)]
        [Header("100%")]
        public UnityEvent2[] whenDisable;

        // When Reset
        // [ReorderableList(noFoldout = true)]
        public UnityEvent2[] whenReset;

        // Next and Prev State

        public List<Process> prevProcess = new List<Process>();

        public List<Process> nextProcess = new List<Process>();
        #endregion

        private void Awake()
        {
            if (ProcessName == "") Debug.LogError(gameObject.name + "Process Name Can't be null");
            // Cache all variable
            enableCondition.SetIfElseConditionVariable();
            disableCondition.SetIfElseConditionVariable();
            whenEnable.SetVariable();
            whenDisable.SetVariable();
            whenReset.SetVariable();

            //Depends On
            for (int i = 0; i < DependOnProcess.Count; i++)
            {
                DependOnProcess[i].valueChanged += CheckEnableCondition;
                DependOnProcess[i].valueChanged += CheckDisableCondition;
            }

            // Activate
            if (IsActive) ForceEnable();
            else ForceDisable();

            // at Min Max
            if (currentPercentage == 100f)
            {
                isAtMax = true;
                prevIsAtMax = true;
            }
            else if (currentPercentage == 0f)
            {
                isAtMax = false;
                prevIsAtMax = false;
            }
        }

        private void percentageChanged()
        {

            // CheckEnableCondition();
            // CheckDisableCondition();

            if (currentPercentage == 100f)
            {
                isAtMax = true;

                if (prevIsAtMax != isAtMax)
                {
                    prevIsAtMax = true;
                    noOfTimesMin++;
                }
            }
            else if (currentPercentage == 0f)
            {
                isAtMax = false;

                if (prevIsAtMax != isAtMax)
                {
                    prevIsAtMax = false;
                    noOfTimesMax++;
                }
            }
        }

        // Call when percentage value changes
        public void CheckEnableCondition(Process process = null)
        {
            if ((isGuidedActive && !ignoreGuidedMode) || isActive) return;
            bool result = enableCondition.checkAllCondition();
#if UNITY_EDITOR
            Debug.Log(ProcessName + " => Enable " + result);
#endif
            if (result)
            {
                whenEnable.Invoke();
                IsActive = true;
            }
        }
        // Call when percentage value changes
        public void CheckDisableCondition(Process process = null)
        {
            if ((isGuidedActive && !ignoreGuidedMode) || !isActive) return;
            bool result = disableCondition.checkAllCondition();
#if UNITY_EDITOR
            Debug.Log(ProcessName + " => Disable " + result);
#endif
            if (result)
            {
                whenDisable.Invoke();
                IsActive = false;
                finished?.Invoke(this);
            }
        }

        //--------------------------------------------

        [DrawButton("Activate This Process")]
        public void ForceEnable()
        {
            // var prettyObj = this.gameObject.AddComponent<PrettyHierarchy.PrettyObject>();
            // prettyObj.backgroundColor = new Color32(0, 233, 255, 255);  //blue color
            // prettyObj.textColor = new Color32(0, 0, 0, 255);  //Black Color
            IsActive = true;
            whenEnable.Invoke();
        }
        public void ForceDisable()
        {
            IsActive = false;
            whenDisable.Invoke();
            finished?.Invoke(this);
        }
        public void ForceReset()
        {
            whenReset.Invoke();
            currentPercentage = 0;
            if (resetCalled != null)
                resetCalled.Invoke(this);
        }
        public void ForceResetPrevProcess()
        {
            for (int i = 0; i < prevProcess.Count; i++)
            {
                prevProcess[i].ForceReset();
            }
        }
        public void ForceEnableNextProcess()
        {
            for (int i = 0; i < nextProcess.Count; i++)
            {
                nextProcess[i].ForceEnable();
            }
        }
        public void ForceEnablePrevProcess()
        {
            for (int i = 0; i < prevProcess.Count; i++)
            {
                prevProcess[i].ForceEnable();
            }
        }
        public void ForceDisableNextProcess()
        {
            for (int i = 0; i < nextProcess.Count; i++)
            {
                nextProcess[i].ForceDisable();
            }
        }
        public void ForceDisablePrevProcess()
        {
            for (int i = 0; i < prevProcess.Count; i++)
            {
                prevProcess[i].ForceDisable();
            }
        }
        public void _addPercentage(float value)
        {
            CurrentPercentage += value;
        }
        public void _subtractPercentage(float value)
        {
            CurrentPercentage -= value;
        }
        public void _addPercentageIfActive(float value)
        {
            if (isActive)
                CurrentPercentage += value;
        }
        public void _subtractPercentageIfActive(float value)
        {
            if (isActive)
                CurrentPercentage -= value;
        }
        public void _setCurrentPercentage(float value)
        {
            CurrentPercentage = value;
        }
        public void _setCurrentPercentageIfActive(float value)
        {
            if (isActive)
                CurrentPercentage = value;
        }
        public void _setCurrentPercentageIfActive(float value, float delay)
        {
            StartCoroutine(SetPercentageIfActive(value, delay));
        }

        IEnumerator SetPercentageIfActive(float value, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (isActive)
                CurrentPercentage = value;
        }
    }
}

