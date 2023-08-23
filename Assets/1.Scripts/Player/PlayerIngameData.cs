using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIngameData : MonoBehaviour
{
    public static PlayerIngameData Instance;

    float hp = 0;
    public float HP { get { return hp; } set { hp = value; } }

    int coin = 300;
    public int Coin { get { return coin; } set { if (value < 0) coin = 0; else coin = value; } }

    PlayerManager.CHANGETYPE changeType = PlayerManager.CHANGETYPE.Normal;
    public PlayerManager.CHANGETYPE ChangeType { get { return changeType; } set { changeType = value; } }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            DestroyImmediate(gameObject);
    }
}
