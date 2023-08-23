using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.AI;

public class Boss_Ex : MonoBehaviour
{
    Rigidbody rb;
    public Animator anim;
    public float speed = 5;
    SSB_BossHP bossHP;
    public Transform Player;

    // 

    int attackAniType = 0;

    float footsoundTime = 0f;
    float footsoundDelay = 0.5f;

    public enum EState
    {
        Idle,
        Move,
        Attack,
        Move1,
        Jump,
        Idle1,
        Move2,
        Attack2
    }

    public GameObject target;
    public float targetDis;
    // euem 숫자에 따른 상태 변화
    public EState state = 0;

    // 현재 시간
    float currentTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        bossHP = GetComponent<SSB_BossHP>();
        target = GameObject.Find("Player");
    }

    void Update()
    {
        switch (state)
        {
            case EState.Idle:
                //UpdateIdle 실행
                UpdateIdle();
                break;
            case EState.Move:
                //UpdateMove 실행
                UpdateMove();
                break;
            case EState.Attack:
                //UpdateAttack 실행
                UpdateAttack();
                break;
        }


        //else if (state == EState.Move1)
        //{
        //    //UpdateAttack 실행
        //    UpdateMove1();

        //}
        //else if (state == EState.Jump)
        //{
        //    //UpdateAttack 실행 
        //    UpdateJump();
        //}
        //if (state == EState.Idle1)
        //{
        //    //UpdateIdle 실행
        //    UpdateIdle1();
        //}
        //else if (state == EState.Move2)
        //{
        //    //UpdateAttack 실행
        //    UpdateMove2();

        //}
        //else if (state == EState.Attack2)
        //{
        //    //UpdateAttack 실행
        //    UpdateAttack2();
        //}
    }

    // 상태가 바뀔 때 초기 셋팅하는 애들...
    void ChangeState(EState s)
    {
        // 현재 상태로  s로 한다.
        state = s;

        currentTime = 0;


        switch (state)
        {
            case EState.Idle:
                anim.SetBool("Move", false);
                anim.SetTrigger("Idle");
                break;
            case EState.Attack:
                anim.SetBool("Move", false);
                currentTime = 4;
                break;
            case EState.Move:
                anim.SetBool("Move", true);
                footsoundTime = 0;
                break;
        }


        //else if (state == EState.Jump)
        //{
        //    currentTime = 1;

        //}
        //else if (state == EState.Attack2)
        //{
        //    currentTime = 1;
        //}
        //else if (state == EState.Idle)
        //{
        //    anim.SetTrigger("Idle");
        //}
        //else if (state == EState.Move)
        //{
        //    anim.SetTrigger("Move");
        //}
    }

    private void UpdateIdle()
    {
        // 현재 시간을 증가시킨다.
        currentTime += Time.deltaTime;
        // 만약에 현재시간이 2초보다 커지면
        if (currentTime > 1.5f)
        {
            // 현재 상태를 이동으로 한다.
            ChangeState(EState.Move);
        }
    }

    // 타켓을 향해서 이동하고 싶다.
    private void UpdateMove()
    {
        footsoundTime += Time.deltaTime;
        if (footsoundTime > footsoundDelay)
        {
            footsoundTime = 0;
            SoundManager.Instance.PlaySFX("DededeFoot");
        }

        transform.LookAt(target.transform);

        // 타켓을 향하는 방향을 구한다.
        Vector3 dir = target.transform.position - this.transform.position;
        dir.y = 0;
        // 크기를 1로 만든다.
        dir.Normalize();
        // 그 방향으로 이동한다.
        transform.position += dir * speed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, transform.eulerAngles.z);
        // 타켓과 나의 거리를 구하자.
        float dis = Vector3.Distance(target.transform.position, this.transform.position);
        // 만약에 구한 거리가 3보다 작다면
        if (dis < 3)
        {
            // 공격상태로 바꾸고 싶다.
            ChangeState(EState.Attack);
        }
    }

    private void UpdateAttack()
    {
        // 현재 시간을 증가시킨다.
        currentTime += Time.deltaTime;
        // 1초가 지나면
        if (currentTime > 4)
        {
            float dis = Vector3.Distance(target.transform.position, this.transform.position);

            if (attackAniType == 2)
                PlayerManager.Instance.FCamera.SetBossBasic(transform);

            // 만약에 타켓과의 거리가 3보다 커지면
            if (dis > 3)
            {
                // Idle 상태로 바꾸고 싶다.
                ChangeState(EState.Idle);
            }
            else
            {
                if (attackAniType == 0)
                {
                    anim.SetTrigger("Attack");
                    attackAniType = 1;
                    currentTime = 1;
                    SoundManager.Instance.PlaySFX("DededeAttack1",0.8f);
                }
                else if (attackAniType == 1)
                {
                    anim.SetTrigger("Attack1");
                    PlayerManager.Instance.FCamera.SetBossTopView(transform, GameManager.Instance.bossGround);
                    attackAniType = 2;
                    currentTime = 0;
                    SoundManager.Instance.PlaySFX("DededeAttack2",1.7f);
                }
                else if (attackAniType == 2)
                {
                    anim.SetTrigger("Attack2");
                    attackAniType = 0;
                    currentTime = 1;
                    SoundManager.Instance.PlaySFX("DededeAttack3",1f);
                }

            }
        }
    }

    public void DamageProcess()
    {
        //적 체력을 1 감소하고 싶다
        bossHP.HP--;
        //만약 체력이 0 이하라면
        if (bossHP.HP <= 0)
        {
            print("체력 0");
            //state 변경하기
            //m_state = BossState.Die;
            //파괴하고싶다.3초 뒤에
            StartCoroutine(GameClearManager.Instance.GameClear());
            Destroy(gameObject, 3);
            //anim.SetTrigger("Die");
        }
    }



    //private void UpdateMove1()
    //{

    //    transform.LookAt(target.transform);

    //    // 타켓을 향하는 방향을 구한다.
    //    Vector3 dir = target.transform.position - this.transform.position;
    //    dir.y = 0;
    //    // 크기를 1로 만든다.
    //    dir.Normalize();
    //    // 그 방향으로 이동한다.
    //    transform.position += dir * speed * Time.deltaTime;
    //    transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, transform.eulerAngles.z);
    //    // 타켓과 나의 거리를 구하자.
    //    float dis = Vector3.Distance(target.transform.position, this.transform.position);
    //    // 만약에 구한 거리가 3보다 작다면
    //    if (dis < 3)
    //    {
    //        // 공격상태로 바꾸고 싶다.
    //        ChangeState(EState.Move2);
    //    }
    //}

    //private void UpdateMove2()
    //{

    //    transform.LookAt(target.transform);

    //    // 타켓을 향하는 방향을 구한다.
    //    Vector3 dir = target.transform.position - this.transform.position;
    //    dir.y = 0;
    //    // 크기를 1로 만든다.
    //    dir.Normalize();
    //    // 그 방향으로 이동한다.
    //    transform.position += dir * speed * Time.deltaTime;
    //    transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, transform.eulerAngles.z);
    //    // 타켓과 나의 거리를 구하자.
    //    float dis = Vector3.Distance(target.transform.position, this.transform.position);
    //    // 만약에 구한 거리가 3보다 작다면
    //    if (dis < 3)
    //    {
    //        // 공격상태로 바꾸고 싶다.
    //        ChangeState(EState.Attack2);
    //    }
    //}

    //private void UpdateAttack2()
    //{
    //    // 현재 시간을 증가시킨다.
    //    currentTime += Time.deltaTime;
    //    // 1초가 지나면
    //    if (currentTime > 1)
    //    {
    //        float dis = Vector3.Distance(target.transform.position, this.transform.position);
    //        // 만약에 타켓과의 거리가 3보다 커지면
    //        if (dis > 3)
    //        {
    //            // Idle 상태로 바꾸고 싶다.
    //            ChangeState(EState.Idle1);
    //        }
    //        else
    //        {
    //            anim.SetTrigger("Attack2");
    //            currentTime = 0;
    //        }
    //    }
    //}

    //private void UpdateIdle1()
    //{

    //    // 현재 시간을 증가시킨다.
    //    currentTime += Time.deltaTime;
    //    // 만약에 현재시간이 2초보다 커지면
    //    if (currentTime > 1.2f)
    //    {
    //        // 현재 상태를 이동으로 한다.
    //        ChangeState(EState.Jump);
    //    }
    //}

    //private void UpdateJump()
    //{
    //    // 현재 시간을 증가시킨다.
    //    currentTime += Time.deltaTime;
    //    // 1초가 지나면
    //    if (currentTime > 1.2f)
    //    {
    //        float dis = Vector3.Distance(target.transform.position, this.transform.position);
    //        // 만약에 타켓과의 거리가 3보다 커지면
    //        if (dis > 3)
    //        {
    //            // Idle 상태로 바꾸고 싶다.
    //            ChangeState(EState.Move1);
    //        }
    //        else
    //        {
    //            anim.SetTrigger("Jump");
    //            currentTime = 0;
    //        }
    //    }
    //}
}
