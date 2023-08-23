using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerAction : MonoBehaviour
{
    public bool IsAction { get; set;}       //행동모션중
    public bool IsHardAction { get; set; }  //차지, 조준 등의 행동중
    public abstract void Set();
    public abstract void Unset();
    public abstract void KeyAction();
    public abstract void ChangeAnimationStart();
    public abstract void ChangeAnimationEnd();
}
