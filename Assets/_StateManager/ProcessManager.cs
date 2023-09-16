using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
// using UnityEditorInternal;
#endif
namespace Automation
{
    public class ProcessManager : MonoBehaviour
    {
        public Process startThisProcess;
        [Tooltip("Assign a Text Mesh Pro component")]
        public TMP_Text popupTextMeshPro;  //used by subprocess
        public bool enableGuidedMode = true;
        public bool EnableGuidedMode
        {
            get { return enableGuidedMode; }
            set
            {
                enableGuidedMode = value;
                Process.isGuidedActive = value;
            }
        }
        // public bool autoGoToNextProcess = false;

        public List<Process> allActiveProcess = new List<Process>();
        public Process[] allProcess;
        private Process prevProcess;

        [DrawButton("Awake")]
        public void Awake()
        {
            Process.isGuidedActive = enableGuidedMode;
            // Debug.Log("Guided Mode is : " + (Process.isGuidedActive));
            //allProcess = Resources.FindObjectsOfTypeAll<Process>();
            allProcess = transform.GetComponentsInChildren<Process>();
            for (int i = 0; i < allProcess.Length; i++)
            {
                // Get all Active Process
                if (allProcess[i].IsActive) allActiveProcess.Add(allProcess[i]);

                allProcess[i].valueChanged += percentageChanges;
                allProcess[i].isActiveChanged += isActiveProcessChanged;
            }
            SetVariable();
            Invoke("StartProcess", 1f);
        }
        void StartProcess()
        {
            if (startThisProcess) startThisProcess.ForceEnable();
        }
        public void isActiveProcessChanged(Process process)
        {
            if (process.IsActive)
            {
                Process findProcess = allActiveProcess.Find(x => x == process);
                if (findProcess == null)
                {
                    allActiveProcess.Add(process);
                    if (process.GetComponent<PrettyHierarchy.PrettyObject>() == null)
                    {
                        setProcessHierarchyColor(process);
                    }
                }
            }
            else
            {
                allActiveProcess.Remove(process);
                removeProcessHierarchyColor(process);
            }

            prevProcess = process;
        }
        public void percentageChanges(Process process)
        {
            // When Process Complete
            if (process.CurrentPercentage == 100f && Process.isGuidedActive)
            {
                StartCoroutine(WhenProcessComplete(process));
            }
        }

        void setProcessHierarchyColor(Process process)
        {
            var prettyObj = process.gameObject.AddComponent<PrettyHierarchy.PrettyObject>();
            prettyObj.backgroundColor = new Color32(98, 233, 255, 255);  //blue color
            prettyObj.textColor = new Color32(0, 0, 0, 255);  //Black Color
        }

        void removeProcessHierarchyColor(Process process)
        {
            var prettyObj = process.gameObject.GetComponent<PrettyHierarchy.PrettyObject>();
            Destroy(prettyObj);
        }

        public IEnumerator WhenProcessComplete(Process process)
        {
            yield return new WaitForEndOfFrame();
            process.ForceDisable();
            process.ForceEnableNextProcess();
        }

        public void NextProcess()
        {
            Process[] currentActiveProcess = allActiveProcess.ToArray();

            // Disable Active Process
            for (int i = 0; i < currentActiveProcess.Length; i++)
            {
                if (currentActiveProcess[i].nextProcess.Count == 0) continue;

                currentActiveProcess[i].ForceEnableNextProcess();
                currentActiveProcess[i].ForceDisable();
            }
        }

        public void CompleteCurrentSubprocess()
        {
            Process[] currentActiveProcess = allActiveProcess.ToArray();

            for (int i = 0; i < currentActiveProcess.Length; i++)
            {
                if (currentActiveProcess[i].IsActive)
                {
                    Transform[] subprocessChild = currentActiveProcess[i].GetComponentsInChildren<Transform>();

                    for (int j = 1; j < subprocessChild.Length; j++)
                    {
                        if (subprocessChild[j].GetComponent<SubProcess>().isActive)
                        {
                            subprocessChild[j].GetComponent<SubProcess>()._SubProcessCompeleted();
                            return;
                        }
                    }
                }

            }
        }

        public void PrevProcess()
        {
            Process[] currentActiveProcess = allActiveProcess.ToArray();

            if (currentActiveProcess.Length == 0)
            {
                Process lastProcess = prevProcess;
                if (lastProcess.prevProcess.Count != 0)
                {
                    lastProcess.ForceReset();

                    // lastProcess.ForceEnablePrevProcess();
                    // lastProcess.ForceResetPrevProcess();

                    // lastProcess.ForceDisable();

                    lastProcess.ForceEnable();
                }
            }

            for (int i = 0; i < currentActiveProcess.Length; i++)
            {
                if (currentActiveProcess[i].prevProcess.Count == 0) continue;


                currentActiveProcess[i].ForceReset();


                currentActiveProcess[i].ForceResetPrevProcess();
                currentActiveProcess[i].ForceEnablePrevProcess();

                // allActiveProcess.AddRange(allActiveProcess[i].prevProcess);

                currentActiveProcess[i].ForceDisable();

                // Remove Process
                // allActiveProcess.Remove(currentActiveProcess[i]);
            }
        }

        private void SetVariable()
        {
            if (Application.isPlaying)
            {
                Process[] allChildProcess = GetComponentsInChildren<Process>(true);
                bool setPrevProcess = true;
                bool setNextProcess = true;
                for (int i = 0; i < allChildProcess.Length; i++)
                {
                    allChildProcess[i].prevProcess.Clear();
                    allChildProcess[i].nextProcess.Clear();
                    allChildProcess[i].DependOnProcess.Clear();


                    allChildProcess[i].ProcessName = allChildProcess[i].gameObject.name;
                    if (setPrevProcess)
                    {
                        if ((i - 1) >= 0)
                        {
                            // allChildProcess[i].prevProcess.Clear();
                            allChildProcess[i].prevProcess.Add(allChildProcess[i - 1]);
                        }
                    }
                    if (setNextProcess)
                    {
                        if ((i + 1) < allChildProcess.Length)
                        {
                            // allChildProcess[i].nextProcess.Clear();
                            allChildProcess[i].nextProcess.Add(allChildProcess[i + 1]);
                        }
                    }

                    allChildProcess[i].DependOnProcess.AddRange(allChildProcess[i].nextProcess);
                    allChildProcess[i].DependOnProcess.AddRange(allChildProcess[i].prevProcess);
                }
            }
        }

        [DrawButton("Enable Guided")]
        public void EnableGuided()
        {
            EnableGuidedMode = true;
        }
        [DrawButton("Disable Guided")]
        public void DisableGuided()
        {
            EnableGuidedMode = false;
        }
    }
    // ------------------------------------------------------------------------------
    //                               Editor Script
    // ------------------------------------------------------------------------------

#if UNITY_EDITOR

    //-------------------------------------------------------
    //-------------------------------------------------------

    [CustomEditor(typeof(ProcessManager))]
    [CanEditMultipleObjects]
    public class ProcessManagerEditor : Editor
    {
        ProcessManager myScript;
        private void OnEnable()
        {
            myScript = (ProcessManager)target;
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Prev Process"))
            {
                myScript.PrevProcess();
            }

            if (GUILayout.Button("Next Process"))
            {
                myScript.NextProcess();
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Enable Guided Mode"))
            {
                myScript.EnableGuided();
            }

            if (GUILayout.Button("Disable Guided Mode"))
            {
                myScript.DisableGuided();
            }

            GUILayout.EndHorizontal();
            if (GUILayout.Button("Next"))
            {
                myScript.CompleteCurrentSubprocess();
            }

            DrawDefaultInspector();

            // Draw Process Bar
            if (myScript.allActiveProcess != null)
                for (int i = 0; i < myScript.allActiveProcess.Count; i++)
                {
                    ProgressBar((myScript.allActiveProcess[i].CurrentPercentage / 100f), $"{myScript.allActiveProcess[i].ProcessName} {myScript.allActiveProcess[i].CurrentPercentage} % => {myScript.allActiveProcess[i].activeSubProcessName}");
                }


            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
        }
        void ProgressBar(float percentage, string label)
        {
            // Get a rect for the progress bar using the same margins as a textfield:
            Rect rect = GUILayoutUtility.GetRect(18, 18, EditorStyles.textField);

            // percentage must be between 0 to 1
            EditorGUI.ProgressBar(rect, percentage, label);
            EditorGUILayout.Space();
        }
    }
#endif
}
