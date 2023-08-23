using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSB_Boss1 : MonoBehaviour
{
    // state 구성
    //1.플레이어에게 다가가서 망치로 내려찍기 내려찍으면 별이 나온다
    //2.공중에서 점프 후 빙글빙글 돌다가 내려찍으면 원형으로 퍼지는 원거리 공격
    //2-1.뒤로 점프해서 원래 있던 자리로 간다
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
        JumpStop,//2
        Move2,//2.
        Attack2,//2.
        HammerMove,//2
        Back,//2-1.
        Back2,//2-1.
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


    void Start()
    {
        //리지드 바디
        rb = GetComponent<Rigidbody>();
        //Move때 저장할 해머 초기 회전값
        originRot = hammer.transform.rotation;
        //컴포넌트 가져오기
        bossHP = GetComponent<SSB_BossHP>();

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
            case BossState.JumpStop:
                JumpStop();
                break;
            case BossState.Move2:
                Move2();
                break;
            case BossState.Attack2:
                Attack2();
                break;
            case BossState.Back:
                Back();
                break;
            case BossState.Back2:
                Back2();
                break;
            case BossState.Damage:
                Damage();
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
    float attackRange = 2f;
    private void Attack()
    {
        //플레이어쪽으로 이동한다 
        Vector3 dir = target.transform.position - transform.position;
        float distance = dir.magnitude;
        dir.Normalize();
        //타겟의 위치를 저장한다
        Vector3 targetRot = target.transform.position;
        //바닥에 붙어있도록 한다
        dir.y = 0;
        //타겟방향으로 몸 회전시키기 LookRotation은 해당벡터를 바라보는 함수
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
        //이동하게하기
        transform.position += dir * speed * Time.deltaTime;
        //rb.MovePosition(rb.position + dir * speed * Time.deltaTime);
        //attackRange가 1, attackRange보다 현재 거리가 좁으면 
        if (distance < attackRange)
        {
            //가까이 다가갔으면 멈추고 
            speed = 0;
            m_state = BossState.HammerMove;
            noJumpPos = transform.position; //점프 전 위치를 저장한다
            
        }
    }
    //현재 위치를 저장한다
    Vector3 noJumpPos;
    //회전 x값
    Quaternion rotX;

    private void HammerMove()
    {
    

        Vector3 dir = target.transform.position - transform.position;
        dir.Normalize();

        //해머활성화
        isHammer = true;
        Quaternion secontRot = Quaternion.Euler(-90, 0, 0);
        Quaternion thirdRot = Quaternion.Euler(20, 0, 0);
        currentTime += Time.deltaTime;
        //타겟방향으로 몸 회전시키기 LookRotation은 해당벡터를 바라보는 함수
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
        //고정되어야할 회전값
        rotX = transform.rotation;
        rotX.x = 0;
        //타겟방향으로 몸 회전시킬때 x축 회전되는 것 막기
        transform.rotation = rotX;

        if (currentTime < 2)
        {
            hammer.transform.localRotation = Quaternion.Lerp(secontRot, thirdRot, (currentTime / 2) * 20);
        }
        //3초 뒤에 TimeLimit
        if (currentTime >= 3)
        {
           
            m_state = BossState.TimeLimit;
        }
    }


    void TimeLimit()
    {

        m_state = BossState.JumpSpin;
        // 내 위치에서 위로 5만큼 떨어진 위치를 구하고싶다.
        jumpPos = transform.position + Vector3.up * jumpPower;
    }
    Vector3 jumpPos;
    float jumpPower = 5.5f;
    private void JumpSpin()
    {
        Vector3 dir = target.transform.position - transform.position;
        dir.Normalize();
        //lerp로 현재위치에서 위로 5만큼 속도로 점프한다.
        transform.position = Vector3.Lerp(transform.position, jumpPos, 6 * Time.deltaTime);
        //타겟방향으로 몸 회전시키기 LookRotation은 해당벡터를 바라보는 함수
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
        //타겟방향으로 몸 회전시킬때 x축 회전되는 것 막기
        //고정되어야할 회전값
        rotX = transform.rotation;
        rotX.x = 0;
        transform.rotation = rotX;
        //위치를 비교했을때 현재위치와 위로 5로 떨어진 지점이 0.1보다 가까워졌다면
        if (transform.position.y >= jumpPos.y - 0.1f && transform.position.y >= 5)//jumpPos.y보다 내 y가 올라갔다면
        {
            m_state = BossState.JumpStop;
        }
    }

    private void JumpStop()
    {
        m_state = BossState.Move2;
    }

    private void Move2()
    {
        Vector3 dir = target.transform.position - transform.position;
        dir.Normalize();
    

        //타겟방향으로 몸 회전시키기 LookRotation은 해당벡터를 바라보는 함수
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
        //고정되어야할 회전값
        rotX = transform.rotation;
        rotX.x = 0;
        //타겟방향으로 몸 회전시킬때 x축 회전되는 것 막기
        transform.rotation = rotX;

        //만약 플레이어와의 거리가 일정거리 이상 가까워지면  
        float distance = Vector3.Distance(this.transform.position, target.transform.position);
        currentTime += Time.deltaTime;
        // 플레이어의 현재위치를 저장한다
        Vector3 targetPos = target.transform.position;
        if (currentTime > 1)
        {

            transform.position = Vector3.Lerp(transform.position, targetPos, 10 * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 1f)
            {
                //상태를 attack2로 바꾸자
                currentTime = 0;
                m_state = BossState.Attack2;
            }
        }
    }

    //바닥에 내려찍는 속도
    float attackSpeed = 50;
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

    private void Attack2()
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
            
            //바닥으로 내려찍는다
            Vector3 dir = targetPosition - transform.position;
            dir.Normalize();
            rb.AddForce(dir * attackSpeed, ForceMode.Impulse);
            if(curPos.y <2)
            {
                curPos = transform.position; //위치
                curPos.y = 0;
                transform.position = curPos;


            }
            noJumpBackPos = back.transform.position; //back의 전 위치를 저장한다
            m_state = BossState.Back;
        }
    }
    //현재 뒤로 갈 위치를 저장한다.
    Vector3 noJumpBackPos;
    Vector3 backJumpPos;
    public Transform back;
    Vector3 curPos;
    private void Back()//뒤로 점프하기
    {
        Vector3 dir = targetPosition - transform.position;
        dir.Normalize();


        //2초동안 올라가고 
        currentTime += Time.deltaTime;
        if (currentTime > 2)
        {
            // 내 위치에서 위로 5만큼 떨어진 위치를 구하고싶다.
            backJumpPos = transform.position + (Vector3.up * 5) + (transform.forward * -2);

            //타겟방향으로 몸 회전시키기 LookRotation은 해당벡터를 바라보는 함수
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
            //고정되어야할 회전값
            rotX = transform.rotation;
            rotX.x = 0;
            rotX.z = 0;

            //lerp로 현재위치에서 점프한다.
            transform.position = Vector3.Lerp(transform.position, backJumpPos, 10 * Time.deltaTime);
            //앞방향을 타겟쪽으로 하기
            transform.forward = target.transform.position;
            if (transform.position.y > 4f)
            {
                //y값이 4가 넘으면 y값이 5로 고정되게끔하기
                curPos = transform.position;
                curPos.y = 5;
                m_state = BossState.Back2;

            }

        }

    }
    private void Back2() //백점프
    {
        Vector3 curPos;
        Vector3 dir = Vector3.down;
        int speed = 3;

        currentTime += Time.deltaTime;

        if (currentTime > 2.2f && currentTime < 2.4f)//lerp로 현재위치에서 뒤로 점프한다.
        {
            transform.position = Vector3.Lerp(transform.position, noJumpBackPos, speed * Time.deltaTime);
        }
        if (currentTime > 2.4f && currentTime < 2.8f)//아래방향으로 내려간다
        {
            transform.position += dir * 2 * Time.deltaTime;
        }
        if (currentTime > 2.8f)//현재위치의 y값이 2보다 작다면 y값을 0으로 만들고 스피드도 0으로
        {
            speed = 2;

            transform.position += dir * speed * Time.deltaTime;
            curPos = transform.position;
            if (curPos.y < 2)
            {
                curPos.y = 0;
                transform.position = curPos; //현재 위치 y값을 0으로 움직이게 만든다.
                speed = 0;
                dir.y = 0;
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
            Destroy(gameObject, 3);
            //anim.SetTrigger("Die");
        }
        else
        {

        }
    }

    private void Damage()
    {

    }

    private void Drop()
    {

    }

    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = true;
            curPos = transform.position;
            curPos.y = 0;
            transform.position = curPos;
        }

        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {

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
