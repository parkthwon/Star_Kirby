using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBreath : MonoBehaviour
{
    [SerializeField] float breathStartSpeed = 10f;
    [SerializeField] float lifeTime = 0.3f;
    Vector3 moveDir;

    public void Set(Vector3 dir)
    {
        moveDir = dir;
        transform.position += Vector3.up * 0.55f + moveDir * 0.7f;
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        //정해진 방향으로 이동
        transform.position += moveDir * Time.deltaTime * breathStartSpeed;
        breathStartSpeed *= 0.98f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }
}
