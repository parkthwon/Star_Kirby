using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class psw_rtmap : MonoBehaviour
{
    public float speed = 5;
    public float rotSpeed = 200;
    public float rz;
    float currentTime;
    public bool Rotation = true;
    bool rotationDirection = true;
    // Start is called before the first frame update
    void Start()
    {
        currentTime = 0;
        rz = -20;
    }

    //void CanRotation()
    //{
    //    rz = Mathf.Clamp(rz, -20, 20);
    //}

    // Update is called once per frame
    void Update()
    {
        //rz += Time.deltaTime * speed;
        //transform.rotation = Quaternion.Euler(0, 0, rz );
        //rz = Mathf.Clamp(rz, -20, 20);
        
        
        if (Rotation)
        {
            currentTime += Time.deltaTime;
            if (currentTime > 5f)
            {
                rotationDirection = !rotationDirection; // 회전 방향을 반대로 변경
                currentTime = 0;
            }

            // 회전 방향에 따라 rz 값 증가 또는 감소
            if (rotationDirection)
            {
                rz += Time.deltaTime * speed;
            }
            else
            {
                rz -= Time.deltaTime * speed;
            }

            // rz 값을 -20과 20 사이로 제한
            rz = Mathf.Clamp(rz, -20, 20);
            transform.rotation = Quaternion.Euler(0, 0, rz);
        }
    }
}
