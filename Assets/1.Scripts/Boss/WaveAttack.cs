using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveAttack : MonoBehaviour
{
    private void OnParticleTrigger()
    {
        if (PlayerManager.Instance.transform.position.y > 1) return;  //높은 곳에서는 맞지 않음

        //적의 반대를 향하는 벡터
        Vector3 dir = PlayerManager.Instance.transform.position - transform.position;
        dir.y = 0;
        dir.Normalize();
        PlayerManager.Instance.PHealth.Hit(dir, 2, true);
    }
}