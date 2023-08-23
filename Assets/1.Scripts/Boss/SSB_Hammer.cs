using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSB_Hammer : MonoBehaviour
{
    //처음 로테이션 값
    Quaternion originRot;
    //두번째 로테이션 값
    Quaternion secondRot;
    //세번째 로테이션 값
    Quaternion thirdRot;
    //회전 속도
    public float rotSpeed = 90;
    //회전했는지
    bool isRotating = false;
    //현재시간
    float currentTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        originRot = transform.rotation;
        secondRot = Quaternion.Euler(-90, 0, 0);
        thirdRot = Quaternion.Euler(20, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Rotate()
    {
        if(isRotating == true) //만약에 회전했다면
        {
            currentTime += Time.deltaTime;
            if (currentTime < 2 )
            {
                //x축으로 -90도까지 올렸다가
                transform.localRotation = Quaternion.Lerp(originRot, secondRot, Time.deltaTime * 5);
            }
            if (currentTime > 4)
            {
                //x축으로 20도로 변경한다
                transform.localRotation = Quaternion.Lerp(secondRot, thirdRot, Time.deltaTime * 5);
            }

        }

    }
}
