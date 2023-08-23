using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class psw_Boss : MonoBehaviour
{
    public enum EState
    {
        Idle,
        Move,
        Attack
    }

    public GameObject target;
    public float targetDis;
    //0 : 대기, 1 : 이동, 2 : 공격
    public EState state = 0;

    //현재 시간
    float currentTime;

    void Start()
    {
        
    }

    void Update()
    {
        //만약에 현재상태가 0이라면
        if (state == EState.Idle)
        {
            //UpdateIdle 실행
            UpdateIdle();        
        }
        //그렇지않고 만약에 현재상태가 1이라면
        else if (state == EState.Move)
        {
            //UpdateMove 실행
            UpdateMove();
        }
        //그렇지않고 만약에 현재상태가 2라면
        else if (state == EState.Attack)
        {
            //UpdateAttack 실행
            UpdateAttack();
        }
    }

    //상태가 바뀔때 초기 셋팅하는 애들...
    void ChangeState(EState s)
    {
        //현재상태를 s 로 한다.
        state = s;

        currentTime = 0;

        if(state == EState.Attack)
        {
            currentTime = 1;
        }
    }

    private void UpdateAttack()
    {
        // 현재시간을 증가시킨다.
        currentTime += Time.deltaTime;
        //1초가 지나면
        if (currentTime > 1)
        {
            float dis = Vector3.Distance(target.transform.position, this.transform.position);
            //만약에 타겟과의 거리가 3보다 커지면
            if (dis > 3)
            {
                //Idle 상태로 바꾸고 싶다.
                ChangeState(EState.Idle);
            }
            else
            {
                print("공격");
                currentTime = 0;
            }
        }
    }

     //타겟을 향해서 이동하고 싶다.
    private void UpdateMove()
    {
        //this.transform.LookAt(GameObject.Find);

        //타겟을 향하는 방향을 구한다.
        Vector3 dir = target.transform.position - this.transform.position;
        
        dir.y = 0;
        //크기를 1로 만든다.
        dir.Normalize();
        //그 방향으로 이동한다.
        transform.position += dir * 5 * Time.deltaTime;

        // 타게과 나의 거리를 구하자.
        float dis = Vector3.Distance(target.transform.position, this.transform.position);
        // 만약에 구한 거리가 3보다 작다면        
        if (dis < 3)
        {
            // 공격상태로 바꾸고 싶다.
            ChangeState(EState.Attack);
        }
    }
    
    private void UpdateIdle()
    {
        // 현재시간을 증가시킨다.
        currentTime += Time.deltaTime;
        // 만약에 현재시간이 2초보다 커지면
        if (currentTime > 2)
        {
            // 현재상태를 이동으로 한다.
            ChangeState(EState.Move);
        }
    }
}
