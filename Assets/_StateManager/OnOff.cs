using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAutomation;

[System.Serializable]
public class TypeDetails
{
    public string chooseType;
    public bool enableDisable;
}
public class OnOff : MonoBehaviour
{
    public float onDelay = 0f;
    public float offDelay = 0f;
    [HideInInspector]
    public List<string> ChooseTypes = new List<string>();

    public GameObject gm;

    public Transform[] excludeGameObjects;
    [Space]

    [OnOff("ChooseTypes")]

    public List<TypeDetails> typeList = new List<TypeDetails>();

    public UnityEvent2[] WhenOn;
    public UnityEvent2[] WhenOff;
    Transform[] allTransform;


    // ------------------------------------
    List<PropertyInfo> propertyList = new List<PropertyInfo>();
    List<Component> componentList = new List<Component>();
    List<bool> valueList = new List<bool>();

    private void Awake()
    {
        StoreType();
        StoreValue();
    }

    [DrawButton("On")]
    public void _On()
    {
        // if(!gm.activeInHierarchy)
        // {
        Invoke("OnWithDelay", onDelay);
        // }
        // else
        //     StartCoroutine(OnOffWithDelay(onDelay,true));
    }

    [DrawButton("Off")]
    public void _Off()
    {
        // if(!gm.activeInHierarchy)
        // {
        Invoke("OffWithDelay", offDelay);
        // }
        // else
        //     StartCoroutine(OnOffWithDelay(offDelay,false));
    }

    void OnWithDelay()
    {
        for (int i = 0; i < propertyList.Count; i++)
        {
            propertyList[i].SetValue(componentList[i], valueList[i]);
        }
        WhenOn.Invoke();
    }
    void OffWithDelay()
    {
        for (int i = 0; i < propertyList.Count; i++)
        {
            propertyList[i].SetValue(componentList[i], !valueList[i]);
        }
        WhenOff.Invoke();
    }
    // IEnumerator OnOffWithDelay(float delay, bool onOff)
    // {
    //     yield return new WaitForSeconds(delay);

    //     if(onOff) OnWithDelay();
    //     else OffWithDelay();
    // }

    public void StoreValue()
    {
        propertyList.Clear();
        componentList.Clear();
        valueList.Clear();

        for (int i = 0; i < typeList.Count; i++)
        {
            for (int y = /* (excludeThisGameObject) ? 1 : */ 0; y < allTransform.Length; y++)
            {
                Component[] comps = allTransform[y].GetComponents(typeList[i].chooseType.ToType());

                for (int z = 0; z < comps.Length; z++)
                {
                    if (comps[z] != null)
                    {
                        PropertyInfo propertyInfo = comps[z].GetType().GetProperty("enabled");
                        if (propertyInfo != null)
                        {
                            propertyList.Add(propertyInfo);
                            componentList.Add(comps[z]);
                            valueList.Add(typeList[i].enableDisable);
                        }
                    }
                }
            }
        }
    }
    public void StoreType()
    {
        ChooseTypes.Clear();
        if (gm == null) gm = this.gameObject;
        Transform[] allGmTransform = gm.GetComponentsInChildren<Transform>(true);

        List<Transform> transfromToBeAdded = new List<Transform>();

        for (int i = 0; i < allGmTransform.Length; i++)
        {
            bool isFound = false;
            if (excludeGameObjects != null)
                for (int y = 0; y < excludeGameObjects.Length; y++)
                {
                    if (allGmTransform[i].Equals(excludeGameObjects[y])) isFound = true;
                }

            if (!isFound) transfromToBeAdded.Add(allGmTransform[i]);
        }

        allTransform = transfromToBeAdded.ToArray();

        for (int i = /* (excludeThisGameObject) ? 1 : */ 0; i < allTransform.Length; i++)
        {
            Component[] allComponent = allTransform[i].GetComponents<Component>();

            for (int y = 0; y < allComponent.Length; y++)
            {
                string typeName = allComponent[y].GetType().ToString();

                // typeName = typeName.Replace("UnityEngine.","");

                if (typeName.Contains("UnityEngine.MeshFilter")) continue;
                if (typeName.Contains("UnityEngine.Transform")) continue;

                if (!ChooseTypes.Contains(typeName))
                {
                    ChooseTypes.Add(typeName);
                }
            }
        }

    }
#if UNITY_EDITOR
    [DrawButton("Update Value")]
    public void OnValidate()
    {
        StoreType();
        Invoke("StoreValue", 0.5f);
    }
#endif
}
