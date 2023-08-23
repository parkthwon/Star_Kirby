using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionManager : MonoBehaviour
{
    [SerializeField] PlayerAction actionPistol;

    PlayerAction curAction;

    // Start is called before the first frame update
    public void Set(PlayerManager.CHANGETYPE type)
    {
        //기존 액션 설정해제
        if (curAction != null)
            curAction.Unset();

        switch (type)
        {
            case PlayerManager.CHANGETYPE.Normal:
                curAction = null;
                break;
            case PlayerManager.CHANGETYPE.Pistol:
                curAction = actionPistol;
                break;
        }

        //새 액션 설정
        if (curAction != null)
            curAction.Set();
    }

    public PlayerAction GetCurAction()
    {
        return curAction;
    }

    public void ChangeAnimationStart()
    {
        if (curAction == null) return;
        curAction.ChangeAnimationStart();
    }

    public void ChangeAnimationEnd()
    {
        if (curAction == null) return;
        curAction.ChangeAnimationEnd();
    }
}
