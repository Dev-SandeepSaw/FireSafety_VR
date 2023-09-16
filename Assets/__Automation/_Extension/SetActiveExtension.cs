using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SetActiveExtension
{
    /// <summary>
    /// Enable Disable GameObject
    /// Ignore If GameObject is null
    /// </summary>
    /// <param name="gameObject">GameObject</param>
    /// <param name="state">Enable Or Disable</param>
    public static void SetActive2(this GameObject gameObject, bool state)
    {
        if (gameObject == null) return;
            gameObject.SetActive(state);
    }

    /// <summary>
    /// Enable Disable GameObject
    /// Ignore If Component is null
    /// </summary>
    /// <param name="comp">Component</param>
    /// <param name="state">Enable Or Disable</param>
    public static void SetGameObjectActive(this Component comp, bool state)
    {
        if (comp == null) return;
            comp.gameObject.SetActive(state);
    }


    /// <summary>
    /// Enable Disable Array of GameObject
    /// </summary>
    /// <param name="gameObject">GameObject</param>
    /// <param name="state">Enable Or Disable</param>
    public static void SetActive2(this GameObject[] gameObject, bool state)
    {
        if (gameObject == null) return;
        int count = gameObject.Length;
        if (count == 0) return;
        for (int i = 0; i < count; i++)
        {
            if (gameObject[i] == null) return;
            gameObject[i].SetActive(state);
        }
    }
    /// <summary>
    /// Enable Disable List of GameObject
    /// </summary>
    /// <param name="gameObject">GameObject</param>
    /// <param name="state">Enable Or Disable</param>
    public static void SetActive2(this List<GameObject> gameObject, bool state)
    {
        if (gameObject == null ) return;
        int count = gameObject.Count;
        if (count == 0) return;
        for (int i = 0; i < count; i++)
        {
            if (gameObject[i] == null) return;
            gameObject[i].SetActive(state);
        }
    }
}
