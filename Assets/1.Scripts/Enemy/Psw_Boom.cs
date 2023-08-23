using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Psw_Boom : MonoBehaviour
{
   
    float currentTime;
    public float speed = 5.0f; // 원하는 총알 속도를 설정합니다.
    public float distanceTime = 1f;
    Rigidbody rb;
    public GameObject particle;

    bool needDestroy = false;
    float destroyTime = 0f;
    float destroyDelay = 3f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Vector3 dir = transform.forward + transform.up;
        dir.Normalize();
        rb.AddForce(dir * speed, ForceMode.Impulse);
        
       // rb.AddTorque(Vector3.right * 100, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        // 1. 시간이 흐르다가
        //transform.position += transform.forward * speed * Time.deltaTime;
        // 1. 시간이 흐르다가
        //currentTime += Time.deltaTime;
        //// 2. 만약 현재시간이 생성시간이 되면
        //if (currentTime > distanceTime)
        //{
        //    currentTime = 0;
        //}        if (suction && !suctionObj.activeSelf)

        if (needDestroy)
            destroyTime += Time.deltaTime;

        if (destroyTime > destroyDelay)
        {
            needDestroy = false;

            GameObject pa = Instantiate(particle);
            pa.transform.position = this.transform.position;
            Destroy(pa, 1);
            Destroy(this.gameObject);
            SoundManager.Instance.PlaySFX("EnemyBomb");
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

        needDestroy = true;
    }
}
