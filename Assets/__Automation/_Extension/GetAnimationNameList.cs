using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GetAnimationNameList{
    public static string[] AnimationList(this Animation animComp)
    {
        if(animComp == null) return null;

        List<string> animNameList = new List<string>();
        foreach (AnimationState animState in animComp)
        {
            if(animState == null) return null;
            animNameList.Add(animState.clip.name);
        }
        if(animNameList.Count == 0) return null;
        return animNameList.ToArray();
    }
}
