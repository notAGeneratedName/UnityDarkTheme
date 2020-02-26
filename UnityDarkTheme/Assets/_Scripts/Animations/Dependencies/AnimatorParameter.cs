using System;
using MightyAttributes;
using UnityEngine;

[Serializable]
public class AnimatorParameter
{
    public string parameterName;

    [ReadOnly, AnimatorParameter("parameterName", true)]
    public int ID;

#if UNITY_EDITOR
    public AnimatorParameter(string parameterName)
    {
        this.parameterName = parameterName;
        ID = Animator.StringToHash(parameterName);
    }
#endif
}