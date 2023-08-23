using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class psw_bullet : MonoBehaviour
{ 
    public float speed = 10;
//    public float angle = 45f;
//    public float gravity = 9.5f;
    float currentTime;
    //Vector3 initialVelocity = Quaternion.Euler(angle, 0f, 0f) * transform.forward * speed;
    public float distanceTime = 1f;
    //public float time = 0;
    // Start is called before the first frame update
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
       transform.position += transform.forward * speed * Time.deltaTime;
        // 1. 시간이 흐르다가
        currentTime += Time.deltaTime;
        // 2. 만약 현재시간이 생성시간이 되면
        if (currentTime > distanceTime)
        {
            Destroy(gameObject);
            currentTime = 0;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
       
        if (other.gameObject.CompareTag("Player"))
        {
            //적의 반대를 향하는 벡터
            Vector3 dir = other.transform.position - transform.position;
            dir.y = 0;
            dir.Normalize();
            PlayerManager.Instance.PHealth.Hit(dir, 1, false);
        }

        Destroy(gameObject);
    }
}
