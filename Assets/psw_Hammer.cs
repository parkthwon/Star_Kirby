using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class psw_Hammer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //적의 반대를 향하는 벡터
            Vector3 dir = other.transform.position - transform.position;
            dir.y = 0;
            dir.Normalize();
            PlayerManager.Instance.PHealth.Hit(dir, 1, false);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
    }
}
