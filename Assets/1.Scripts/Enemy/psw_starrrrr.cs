using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class psw_starrrrr : MonoBehaviour
{

    float currentTime;
    public float speed = 5.0f; // 원하는 총알 속도를 설정합니다.
    public float distanceTime = 1f;
    Rigidbody rb;
    public GameObject particle;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Vector3 dir = transform.forward + transform.up;
        dir.Normalize();
        rb.AddForce(dir * speed, ForceMode.Impulse);
    }


    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > 2.5f)
        {
            Destroy(gameObject);
        }
    }

    //private void OnCollisionEnter(Collision other)
    //{

    //    if (other.gameObject.CompareTag("Player"))
    //    {
    //        //적의 반대를 향하는 벡터
    //        Vector3 dir = other.transform.position - transform.position;
    //        dir.y = 0;
    //        dir.Normalize();
    //        PlayerManager.Instance.PHealth.Hit(dir, 1, true);
    //    }

    //    Destroy(gameObject, 3);
    //}




}