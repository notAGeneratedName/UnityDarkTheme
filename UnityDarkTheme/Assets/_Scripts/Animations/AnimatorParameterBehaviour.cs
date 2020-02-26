using MightyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class AnimatorParameterBehaviour: MonoBehaviour
{
    public Animator animator;
    public int parameterIndex;

    [Nest(NestOption.ContentOnly), ButtonArray(ArrayOption.ContentOnly), Fold("Parameters")]
    public AnimatorParameter[] parameters;

    public bool ignoreInactive;

#if UNITY_EDITOR
    [Button]
    private void AnimatorFromSelf() => animator = GetComponent<Animator>();
#endif

    private bool CanSet() => ignoreInactive || gameObject.activeSelf;

    #region Setters

    #region SynchedTrigger

    public void SetTrigger() => SetTrigger(parameterIndex);

    public void SetTrigger(int index)
    {
        if (index < parameters.Length)
            SetTrigger(parameters[index]);
    }

    public void SetTrigger(AnimatorParameter param) => SetTrigger(animator, param);
    public void SetTrigger(Animator anim, int index) => SetTrigger(anim, parameters[index]);
    public void SetTrigger(Animator anim, AnimatorParameter param) => SetAnimatorTrigger(anim, param.ID);

    private void SetAnimatorTrigger(Animator anim, int triggerID)
    {
#if UNITY_EDITOR
        if (!EditorApplication.isPlaying) return;
#endif
        if (CanSet())
            anim.SetTrigger(triggerID);
    }

    #endregion /SynchedTrigger

    #region Bool

    public void SetBool(bool value) => SetBool(parameterIndex, value);

    public void SetBool(int index, bool value)
    {
        if (index < parameters.Length)
            SetBool(parameters[index], value);
    }

    public void SetBool(AnimatorParameter param, bool value) => SetBool(animator, param, value);
    public void SetBool(Animator anim, int index, bool value) => SetBool(anim, parameters[index], value);
    public void SetBool(Animator anim, AnimatorParameter param, bool value) => SetAnimatorBool(anim, param.ID, value);

    private void SetAnimatorBool(Animator anim, int boolID, bool value)
    {
        if (CanSet())
            anim.SetBool(boolID, value);
    }

    #endregion /Bool

    #region Integer

    public void SetInteger(int value) => SetInteger(parameterIndex, value);

    public void SetInteger(int index, int value)
    {
        if (index < parameters.Length)
            SetInteger(parameters[index], value);
    }

    public void SetInteger(AnimatorParameter param, int value) => SetInteger(animator, param, value);
    public void SetInteger(Animator anim, int index, int value) => SetInteger(anim, parameters[index], value);
    public void SetInteger(Animator anim, AnimatorParameter param, int value) => SetAnimatorInteger(anim, param.ID, value);

    private void SetAnimatorInteger(Animator anim, int integerID, int value)
    {
        if (CanSet())
            anim.SetInteger(integerID, value);
    }

    #endregion /Integer

    #region Float

    public void SetFloat(float value) => SetFloat(parameterIndex, value);

    public void SetFloat(int index, float value)
    {
        if (index < parameters.Length)
            SetFloat(parameters[index], value);
    }

    public void SetFloat(AnimatorParameter param, float value) => SetFloat(animator, param, value);
    public void SetFloat(Animator anim, int index, float value) => SetFloat(anim, parameters[index], value);
    public void SetFloat(Animator anim, AnimatorParameter param, float value) => SetAnimatorFloat(anim, param.ID, value);

    private void SetAnimatorFloat(Animator anim, int floatID, float value)
    {
        if (CanSet())
            anim.SetFloat(floatID, value);
    }

    #endregion /Float

    #endregion /SynchedTrigger

    #region Getters

    #region Bool

    public bool GetBool() => GetBool(parameterIndex);
    public bool GetBool(int index) => index < parameters.Length && GetBool(parameters[index]);
    public bool GetBool(AnimatorParameter param) => GetBool(animator, param);
    public bool GetBool(Animator anim, int index) => GetBool(anim, parameters[index]);
    public bool GetBool(Animator anim, AnimatorParameter param) => GetAnimatorBool(anim, param.ID);
    private static bool GetAnimatorBool(Animator anim, int boolID) => anim.GetBool(boolID);

    #endregion

    #region Integer

    public int GetInteger() => GetInteger(parameterIndex);
    public int GetInteger(int index) => index < parameters.Length ? GetInteger(parameters[index]) : 0;
    public int GetInteger(AnimatorParameter param) => GetInteger(animator, param);
    public int GetInteger(Animator anim, int index) => GetInteger(anim, parameters[index]);
    public int GetInteger(Animator anim, AnimatorParameter param) => GetAnimatorInteger(anim, param.ID);
    private static int GetAnimatorInteger(Animator anim, int intID) => anim.GetInteger(intID);

    #endregion /Integer

    #region Float

    public float GetFloat() => GetFloat(parameterIndex);
    public float GetFloat(int index) => index < parameters.Length ? GetFloat(parameters[index]) : 0;
    public float GetFloat(AnimatorParameter param) => GetFloat(animator, param);
    public float GetFloat(Animator anim, int index) => GetFloat(anim, parameters[index]);
    public float GetFloat(Animator anim, AnimatorParameter param) => GetAnimatorFloat(anim, param.ID);
    private static float GetAnimatorFloat(Animator anim, int intID) => anim.GetFloat(intID);

    #endregion /Float

    #endregion /Getters
}