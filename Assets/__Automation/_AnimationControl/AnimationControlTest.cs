using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControlTest : MonoBehaviour
{
    [Range(0f,500f)]
    public float myFloat;
    public int myInt;
    private void Start() {
        GetComponent<AnimationControl>()._StartBinding(0);
    }
}
