using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using effects;
using Automation;

public class SetProcessVariable : MonoBehaviour
{
    public bool setProcessName = true;
    public bool setPrevProcess = true;
    public bool setNextProcess = true;
    public bool setDependOnProcess = true;
    [DrawButton("Set Variable for Guided")]
    public void SetVariableForGuided()
    {
        Process[] allChildProcess = GetComponentsInChildren<Process>(true);

        for (int i = 0; i < allChildProcess.Length; i++)
        {
            allChildProcess[i].prevProcess.Clear();
            allChildProcess[i].nextProcess.Clear();
            allChildProcess[i].DependOnProcess.Clear();

            if (setProcessName)
            {
                allChildProcess[i].ProcessName = allChildProcess[i].gameObject.name;
            }
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
            if (setDependOnProcess)
            {
                // if( (i-1) >= 0)
                //     allChildProcess[i].DependOnProcess.Add( allChildProcess[i-1] );

                // allChildProcess[i].DependOnProcess.Clear();

                allChildProcess[i].DependOnProcess.AddRange(allChildProcess[i].nextProcess);
                allChildProcess[i].DependOnProcess.AddRange(allChildProcess[i].prevProcess);
            }
        }
    }
}
