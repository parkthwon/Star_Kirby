using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSB_Boss3 : MonoBehaviour
{
    // state 구성
    //1.플레이어에게 다가가서 망치로 내려찍기 내려찍으면 별이 나온다
    //2.공중에서 점프 후 빙글빙글 돌다가 내려찍으면 원형으로 퍼지는 원거리 공격
    //3.anystate - 플레이어의 공격을 받으면 넘어진다
    //4.HP가 40%남으면 무기1을 떨어뜨린다
    //5.공중에 떴다가 커비 방향으로 망치 무기1을 내려찍는다
    //6.기둥무기를 가지고 오는 시네머신
    //7.공중에 떴다가 커비 방향으로 무기2를 내려찍는다
    //8.HP가 20% 남으면 연속으로 바닥에 커비방향으로 무기2를 3번 내려찍는다
    //9.옆쪽으로 휙 무기를 커비 방향으로 휘두른다
    //10.anystate - Die

    SSB_BossHP bossHP;

    public enum BossState
    {
        Idle,
        Move,
        Attack, //1.
        TimeLimit,
        JumpSpin,//2.
        Attack2,//2.
        HammerMove,//2
        Move3,//2
        Charge,//2
        Charge2,//2
        RotAttack,//2
        LerpJump1,//3단점프1
        JumpStop1,//점프스탑1
        LerpJump2,//3단점프2
        LerpJump3,//3단점프3
        Damage,//3.
        Drop,//4.
    };

    public BossState m_state = BossState.Idle;
    //리지드바디
    Rigidbody rb;
    //바닥에 닿았는지
    bool isGrounded = false;
    //해머를 가지고 있는지
    bool isHammer = false;
    //첫번째 각도
    Quaternion originRot;
    //애니메이션 콘트롤러 가져오기
    public Animator anim;


    void Start()
    {
        //리지드 바디
        rb = GetComponent<Rigidbody>();
        //Move때 저장할 해머 초기 회전값
        //originRot = hammer.transform.rotation;
        //컴포넌트 가져오기
        bossHP = GetComponent<SSB_BossHP>();
        //애니메이터 컴포넌트 가져오기
        anim = GetComponentInChildren<Animator>();

    }
    Vector3 jumpUpPos;
    // Update is called once per frame
    void Update()
    {
        switch (m_state)
        {
            case BossState.Idle:
                Idle();
                break;
            case BossState.Move:
                Move();
                break;
            case BossState.Attack:
                Attack();
                break;
            case BossState.HammerMove:
                HammerMove();
                break;
            case BossState.TimeLimit:
                TimeLimit();
                break;
            case BossState.JumpSpin:
                JumpSpin();
                break;
            case BossState.Attack2:
                Attack2();
                break;
            case BossState.Move3:
                Move3();
                break;
            case BossState.Charge:
                Charge();
                break;
            //case BossState.Charge2:
            //    Charge2();
                //break;
            case BossState.RotAttack:
                RotAttack();
                break;
            case BossState.LerpJump1:
                LerpJump1();
                break;
            case BossState.JumpStop1:
                JumpStop1();
                break;
            case BossState.LerpJump2:
                LerpJump2();
                break;
            case BossState.LerpJump3:
                LerpJump3();
                break;
            case BossState.Damage:
                Attack3();
                break;
            case BossState.Drop:
                Drop();
                break;
        }
        //IsHammer가 true일때
        if (isHammer == true)
        {
            //게임오브젝트 해머를 활성화한다
            hammer.SetActive(true);

        }
    }

    //플레이어와 일정거리 이상 가까워지면 상태를 Move로 변경한다
    //필요속성 : 타겟, 일정시간 , 현재시간
    public GameObject target;
    public float creatTime;
    float currentTime = 0;

    private void Idle()
    {
        //3초 후에 상태를 Move로 변경한다

        //필요속성 : 일정시간
        creatTime = 3;
        //1.3초가 지나면
        currentTime += Time.deltaTime;
        if (currentTime > creatTime)
        {
            //3. 상태를 Move로 변경한다
            m_state = BossState.Move;
            currentTime = 0;

        }
    }

    //필요속성 : 일정거리 , 스피드
    float moveRange = 4;
    public float speed = 12;
    private void Move()
    {
        isHammer = true;
        Quaternion firstRot = Quaternion.Euler(0, 0, 0);
        Quaternion secondRot = Quaternion.Euler(0, 0, 30);
        currentTime += Time.deltaTime;
        anim.SetTrigger("Move");
        //플레이어와 일정거리 이상 가까워지면 상태를 어택으로 전환한다
        //필요속성 : 타겟쪽으로 방향 , 처음 플레이어와의 거리 , 일정거리
        Vector3 dir = target.transform.position - transform.position;
        float distance = dir.magnitude;
        dir.Normalize();
        //바닥에 붙어있도록 한다
        dir.y = 0;
        //방향쪽으로 이동한다
        transform.position += dir * speed * Time.deltaTime;
        //타겟방향으로 몸 회전시키기 LookRotation은 해당벡터를 바라보는 함수
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);

        if (currentTime < 2)
        {

            // hammer.transform.localRotation = Quaternion.Lerp(firstRot, secondRot, currentTime / 2);
        }

        //플레이어와 일정거리 이상 가까워지면 상태를 어택으로 전환한다
        //일정거리 이상 좁혀지면
        if (distance < moveRange)
        {
            //상태를 어택으로 전환한다
            m_state = BossState.Attack;

        }

    }
    //타겟이랑 일정거리 이상 좁혀지면 weapon을 들어서 내려찍는다
    //필요속성 : Hammer
    public GameObject hammer;
    float attackRange = 1.5f;
    private void Attack()
    {
        Quaternion secondRot = Quaternion.Euler(0, 0, 30);
        Quaternion thirdRot = Quaternion.Euler(0, 0, 80);
        currentTime += Time.deltaTime;

        //플레이어쪽으로 이동한다 
        Vector3 dir = target.transform.position - transform.position;
        float distance = dir.magnitude;
        //바닥에 붙어있도록 한다
        dir.y = 0;
        dir.Normalize();
        //타겟의 위치를 저장한다
        Vector3 targetRot = target.transform.position;
        //타겟방향으로 몸 회전시키기 LookRotation은 해당벡터를 바라보는 함수
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
        //이동하게하기
        transform.position += dir * speed * Time.deltaTime;

        if (distance < attackRange)
        {
            //hammer.transform.localRotation = Quaternion.Lerp(secondRot, thirdRot, currentTime / 1);
            m_state = BossState.HammerMove;

        }
    }
    //현재 위치를 저장한다
    Vector3 noJumpPos;
    //회전 x값
    Quaternion rotX;

    private void HammerMove()
    {
        currentTime += Time.deltaTime;


        //플레이어쪽으로 이동한다 
        Vector3 dir = target.transform.position - transform.position;
        float distance = dir.magnitude;
        dir.y = 0;
        dir.Normalize();
        transform.position += dir * speed * Time.deltaTime;
        speed = 0;
        anim.SetTrigger("HammerAttack");
        if (currentTime > 3)
        {
            m_state = BossState.TimeLimit;
        }

    }


    void TimeLimit()
    {
        //anim.SetTrigger("Move");
        //yVelocity = 20;
        //m_state = BossState.JumpSpin;
        // 내 위치에서 위로 5만큼 떨어진 위치를 구하고싶다.
        jumpPos = transform.position + Vector3.up * jumpPower;

        //시작지점 설정
        startPos = transform.position;
        //도착지점 설정
        endPos = target.transform.position + (target.transform.position - transform.position); //나의 경우는 반지름이 플레이어의 거리만큼 더 커져야 하므로 target-me를 더 더함
        //가운데 지점
        center = Vector3.Lerp(startPos, endPos, 0.5f);
        //가운데 지점의 y축을 내려줘야 반지름의 거리가 짧은 쪽으로 돌기 때문에 세로로 이동한다
        center += new Vector3(0, -1, 0);
        //현재 시간을 0으로 한다.
        ratio = 0;


        m_state = BossState.JumpSpin;
        PlayerManager.Instance.FCamera.SetBossTopView(transform, GameManager.Instance.bossGround);
    }

    Vector3 jumpPos;
    float jumpPower = 5.5f;

    bool isJump = false; //점프를 한다
    Vector3 startPos; //시작
    Vector3 endPos; //끝
    Vector3 center; //가운데
    float ratio;
    private void JumpSpin()
    {
        ratio += Time.deltaTime / 2;  //2초안에 가라

        if (ratio > 0.5f)
        {
            ratio = 0.5f; // 0.5를 넘어가면 멈춰야한다. 그래야 반지름까지만 올라가서 멈출 수 있다.        
        }
        anim.SetTrigger("Jump");
        transform.position = Vector3.Slerp(startPos - center, endPos - center, ratio);// center값을 맨첨에 빼줘서 0의 위치로 이동
        transform.position += center;//다시 값을 더해준다

        if (ratio == 0.5f)   // || target.transform.position.x == transform.position.x
        {
            print("ratio == 0.5f");
            m_state = BossState.Attack2;
        }
    }

    //바닥에 내려찍는 속도
    float attackSpeed = 20;
    //현재시간
    float curTime = 0;
    //타겟저장시간
    float targetTime = 0.5f;
    //지나간 시간
    float creTime = 1;
    //타겟의 위치
    Vector3 targetPosition;
    //초기 힘의 크기
    float originForce;

    private void Attack2()//바닥에 내려찍기
    {

        curTime += Time.deltaTime;//시간이 흐르고
        //현재시간이 타겟저장시간을 초과하고 현재시간이 지나간 시간보다 적거나 같을 때
        if (curTime > targetTime && curTime <= creTime)
        {
            //플레이어의 위치를 기억한다
            targetPosition = target.transform.position;
        }
        if (curTime > creTime)
        {
            anim.SetTrigger("Idle");
            //바닥으로 내려찍는다
            Vector3 dir = Vector3.down;
            dir.Normalize();
            rb.AddForce(dir * attackSpeed, ForceMode.Impulse);
           
            if (curPos.y < 1f) //바닥에 박히는 것 막기
            {
                curPos = transform.position; //위치
                curPos.y = 0;
                transform.position = curPos;


            }
            //타겟방향으로 몸 회전시키기 LookRotation은 해당벡터를 바라보는 함수
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
            //고정되어야할 회전값
            rotX = transform.rotation;
            rotX.x = 0;
            rotX.z = 0;
            noJumpBackPos = back.transform.position; //back의 전 위치를 저장한다
            m_state = BossState.Move3;
            PlayerManager.Instance.FCamera.SetBossBasic(transform);
        }
    }
    private void Move3()
    {
        currentTime += Time.deltaTime;
        if (currentTime > 3) //3초 뒤에 움직이도록
        {
            float speed = 10;
            float moveRange = 2;

            //플레이어와 일정거리 이상 가까워지면 상태를 어택으로 전환한다
            //필요속성 : 타겟쪽으로 방향 , 처음 플레이어와의 거리 , 일정거리
            anim.SetTrigger("Move");
            Vector3 dir = target.transform.position - transform.position;
            float distance = dir.magnitude;
            dir.Normalize();
            //바닥에 붙어있도록 한다
            dir.y = 0;
            //방향쪽으로 이동한다
            transform.position += dir * speed * Time.deltaTime;
            //타겟방향으로 몸 회전시키기 LookRotation은 해당벡터를 바라보는 함수
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);


            //플레이어와 일정거리 이상 가까워지면 상태를 어택으로 전환한다
            //일정거리 이상 좁혀지면
            if (distance < moveRange)
            {
                //상태를 어택으로 전환한다
                m_state = BossState.Charge;
                //현재위치를 저장한다 ->  Charge에서 쓸 위치
                pastPos = transform.position;
            }
        }
    }
    Vector3 pastPos; // 이전 위치;
    Vector3 upPos; // 현재 위치;
    float currentRotTime; // 현재 돌아가는 시간;
    float creatRotTime; // 특정 돌아가는 시간;
    private void Charge() //차징할때 위아래로 진동하면서 
    {
        float speed = 10;
        float creatTime = 0.05f; //특정시간
        bool isUp = false; //올라갔는지

        //위 아래로 진동하게 하기
        //현재시간이 흐르고
        currentTime += Time.deltaTime;
        //일정시간을 초과하면
        if (currentTime > creatTime)
        {
            if (isUp == true)//isUp이 true면
            {
                isUp = false; //false로 바꾼다
            }
            else
            {
                isUp = true; //isUp을 true로 바꾼다
            }
            currentTime = 0; //계속 반복할 수 있도록 현재시간을 0으로 바꾼다.
        }

        if (isUp == true)
        {
            //보스를 위로 올린다
            transform.position += Vector3.up * speed * Time.deltaTime;
            //현재 위치를 저장한다 -> 밑에서 lerp로 연결하기 위해
            upPos = transform.position;
            pastPos = transform.position;
            pastPos.y = 0;
        }
        else
        {
            //transform.position = pastPos;
            transform.position = Vector3.Lerp(upPos, pastPos, 0.5f);
            //보스를 기존 포지션으로 가져온다.
        }

        if (rotateRight == true) // 회전하는게 true라면
        {
            transform.Rotate(0, 1, 0);
            rotValue += 1;

            if (rotValue > 80)
            {
                rotateRight = false;
                transform.Rotate(0, -(rotValue - 80), 0);
                curPos = transform.position;
                curPos.y = 0;
                transform.position = curPos;
                m_state = BossState.RotAttack;
            }
        }



    }
    bool rotateRight = true;
    float rotValue = 0;

    //현재 뒤로 갈 위치를 저장한다.
    Vector3 noJumpBackPos;
    Vector3 backJumpPos;
    public Transform back;
    Vector3 curPos;

    bool rotateLeft = true;
    //private void Charge2() // 호달달달
    //{
        
    //    curTime += Time.deltaTime;
    //    if (curTime > 1)
    //    {
    //        if (rotateLeft)
    //        {
    //            transform.Rotate(0, -0.5f, 0);
    //            rotValue -= 0.5f;
    //        }
    //        if (rotValue < 0)
    //        {
    //            print("호더ㅏㄹ달");
    //            rotateLeft = false;
    //            m_state = BossState.RotAttack;
    //        }
    //    }
    //}
    private void RotAttack()//360도 회전하게 한다
    {
        currentTime += Time.deltaTime;
        //animation 재생
        anim.SetTrigger("360Attack");
        if (currentTime > 2.5)
        {
            m_state = BossState.LerpJump1;
        }

        //1단점프 전 target위치를 저장한다
        isJump = true;
        startPos = transform.position;
        endPos = target.transform.position + (target.transform.position - transform.position);
        center = Vector3.Lerp(startPos, endPos, 0.5f);
        center += new Vector3(0, -1, 0);
        ratio = 0;
        //플레이어의 위치를 기억한다
        targetPosition = target.transform.position;
    }

    private void LerpJump1()//1단점프
    {

        ratio += Time.deltaTime / 1; //1초만에 가라
        if (ratio > 1f)
        {
            anim.SetTrigger("Jump");
            ratio = 0.5f; // 0.5를 넘어가면 멈춰야함 - 반지름까지만 올라가서 멈춰야 하므로
            m_state = BossState.JumpStop1;
        }
        transform.position = Vector3.Slerp(startPos - center, endPos - center, ratio);
        transform.position += center;

        targetPosition = target.transform.position;

    }


    private void JumpStop1()
    {
        anim.SetTrigger("Idle");
        //바닥으로 내려찍는다
        Vector3 dir = targetPosition - transform.position;
        dir.Normalize();
        rb.AddForce(dir * attackSpeed, ForceMode.Impulse);
        if (curPos.y < 2)
        {
            curPos = transform.position; //위치
            curPos.y = 0;
            transform.position = curPos;
            m_state = BossState.LerpJump2;
        }

    }
    private void LerpJump2()
    {
        ratio += Time.deltaTime / 2;  //2초안에 가라

        if (ratio > 0.5f)
        {
            ratio = 0.5f; // 0.5를 넘어가면 멈춰야한다. 그래야 반지름까지만 올라가서 멈출 수 있다.        
        }
        anim.SetTrigger("Jump");
        transform.position = Vector3.up;// center값을 맨첨에 빼줘서 0의 위치로 이동
        transform.position += center;//다시 값을 더해준다

        if (ratio == 0.5f)   // || target.transform.position.x == transform.position.x
        {
            print("ratio == 0.5f");
            m_state = BossState.RotAttack;
        }
    }

    private void LerpJump3()
    {

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
        else
        {

        }
    }

    private void Attack3()
    {
        anim.SetTrigger("Idle");
        //바닥으로 내려찍는다
        Vector3 dir = targetPosition - transform.position;
        dir.Normalize();
        rb.AddForce(dir * attackSpeed, ForceMode.Impulse);
        if (curPos.y < 2)
        {
            curPos = transform.position; //위치
            curPos.y = 0;
            transform.position = curPos;
            m_state = BossState.LerpJump2;
        }
    }

    private void Drop()
    {

    }


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = true;
            curPos = transform.position;
            curPos.y = 0;
            transform.position = curPos;
        }

        if (other.gameObject.CompareTag("Player"))
        {
            Vector3 dir = other.transform.position - transform.position;
            dir.y = 0;
            dir.Normalize();
            PlayerManager.Instance.PHealth.Hit(dir, 1, true);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = false;
        }
    }

}