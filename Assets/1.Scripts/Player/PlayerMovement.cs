using DG.Tweening;
using System.Collections;
using System.Globalization;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    PlayerData playerData;

    Vector3 cameraViewPoint;
    public Vector3 CameraViewPoint { get { return cameraViewPoint; } }
    Vector3 screenBounds;
    float objectHeight = 2f;

    //이동
    Rigidbody rb;
    GroundChecker gc;
    Vector3 velocity;
    Vector3 planeVelocity;  //xz plane velocity
    Quaternion lastFixedRotation;
    Quaternion nextFixedRotation;
    public Quaternion NextFixedRotation { set { nextFixedRotation = value; } }

    //점프
    bool isJump = false;
    public bool IsJump { get { return isJump; } }
    bool jumpFlag = false;
    float jumpFlagTime;

    Vector3 lastSafeAreaPosition;   //마지막 안전지대 위치

    //날기
    bool isFly = false;
    public bool IsFly { get { return isFly; } }
    bool flyFlag = false;
    bool isCanFly = true;
    float flyActiveTime;    //비행 활성화 시간
    float flyActionDelay;   //날개짓 딜레이

    //내뱉기
    bool isBreathAttack = false;
    public bool IsBreathAttack { get { return isBreathAttack; } }
    float breathAttackDelay;    //내뱉기 공격 딜레이
    [SerializeField] GameObject breathFactory;

    //피격
    Vector3 hitDir; //밀려나는 방향

    //사다리
    bool isLadder = false;  //사다리를 타고 있는지
    public bool IsLadder { get { return isLadder; } }
    bool isCloseLadder = false; //사다리 근처에 있는지
    bool isCanLadder = true;    //사다리를 탈 수 있는지
    float ladderDelay;
    Vector3 ladderUpAxis;   //사다리를 올라가는 축

    //걷기 사운드 딜레이
    float footSoundTime = 0;
    float footSoundDelay = 0.5f;

    //게임오버
    [SerializeField] Transform gameoverPivot;

    public void Set(PlayerData data)
    {
        playerData = data;
        rb = GetComponent<Rigidbody>();
        gc = GetComponent<GroundChecker>();

        velocity = new Vector3(0, 0, 0);
        cameraViewPoint = transform.position;
        lastFixedRotation = transform.rotation;
        nextFixedRotation = transform.rotation;

        jumpFlag = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerManager.Instance.IsDie) return;
        //인풋이 없는 경우
        if (!GameManager.Input.isInput)
        {
            planeVelocity = Vector3.zero;
            footSoundTime = 0;
            jumpFlag = false;
            flyFlag = false;
        }

        if (jumpFlag && jumpFlagTime > 0)
        {
            jumpFlagTime -= Time.deltaTime;
            if (jumpFlagTime <= 0)
            {
                jumpFlag = false;
            }
        }

        //날고 있는 상태
        if (isFly)
        {
            flyActiveTime -= Time.deltaTime;
            if (flyActiveTime <= 0)
                isCanFly = false;
        }

        //사다리
        if (isLadder)
        {
            //사다리 중간으로 이동
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(ladderUpAxis.x, transform.position.y, ladderUpAxis.z), Time.deltaTime * 5);

            if (planeVelocity.z > 0.1f)
            {
                //올라가기
                rb.velocity = new Vector3(0, 4, 0);
                PlayerManager.Instance.Anim.speed = 1;
                PlayerManager.Instance.Anim.SetBool("ClimbDown", false);
            }
            else if (planeVelocity.z < -0.1f)
            {
                //내려가기
                rb.velocity = new Vector3(0, -4, 0);
                PlayerManager.Instance.Anim.speed = 1;
                PlayerManager.Instance.Anim.SetBool("ClimbDown", true);
            }
            else
            {
                //애니메이션 일시정지
                rb.velocity = Vector3.zero;
                PlayerManager.Instance.Anim.speed = 0;
                PlayerManager.Instance.Anim.SetBool("ClimbDown", false);
            }
        }
        else if (!isCanLadder)
        {
            ladderDelay -= Time.deltaTime;
            if (ladderDelay <= 0)
                isCanLadder = true;
        }

        if (PlayerManager.Instance.IsChange)
        {
            //상태 해제
            isJump = false;
            isFly = false;
            isBreathAttack = false;
            PlayerManager.Instance.Anim.SetBool("Run", false);
            PlayerManager.Instance.Anim.SetBool("Jump", false);

            velocity = Vector3.zero;
            //카메라를 바라보는 방향
            Vector3 lookCameraVec = -Camera.main.transform.forward;
            lookCameraVec.y = 0;

            nextFixedRotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookCameraVec), playerData.rotateSpeed * Time.unscaledDeltaTime);
            transform.rotation = nextFixedRotation;
        }
        else
        {
            float interpolationAlpha = (Time.time - Time.fixedTime) / Time.fixedUnscaledDeltaTime;
            //cc.Move(Vector3.Lerp(lastFixedPosition, nextFixedPosition, interpolationAlpha) - transform.position);
            transform.rotation = Quaternion.Slerp(lastFixedRotation, nextFixedRotation, interpolationAlpha);
        }
    }

    private void FixedUpdate()
    {
        if (PlayerManager.Instance.IsStartMotion || PlayerManager.Instance.IsDie)
            return;

        if (isBreathAttack)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        if (PlayerManager.Instance.IsHit)
        {
            planeVelocity = hitDir * playerData.hitPower;
        }

        //lastFixedPosition = nextFixedPosition;
        lastFixedRotation = nextFixedRotation;

        if (isLadder)
        {
            velocity = Vector3.zero;
            //카메라 앞방향 보기
            nextFixedRotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward), playerData.rotateSpeed * Time.fixedDeltaTime);
        }
        else
        {
            if (PlayerManager.Instance.IsUnChange && !PlayerManager.Instance.IsHit)
            {
                velocity = Vector3.zero;
            }
            else
            {
                float yVelocity = GetYVelocity();
                velocity = new Vector3(planeVelocity.x, planeVelocity.y + yVelocity, planeVelocity.z);
                if (planeVelocity != Vector3.zero && !PlayerManager.Instance.IsHit)
                    nextFixedRotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(planeVelocity.x, 0, planeVelocity.z)), playerData.rotateSpeed * Time.fixedDeltaTime);
                //nextFixedPosition += velocity * Time.fixedDeltaTime;
            }

            if (planeVelocity == Vector3.zero)
            {
                footSoundTime = 0;
                PlayerManager.Instance.Anim.SetBool("Run", false);
            }
            else
            {
                footSoundTime += Time.fixedDeltaTime;
                if (footSoundTime >= footSoundDelay)
                {
                    footSoundTime = 0;
                    SoundManager.Instance.PlaySFX("Foot");
                }
                PlayerManager.Instance.Anim.SetBool("Run", true);
            }

            rb.velocity = velocity;
        }
    }

    void LateUpdate()
    {
        if (PlayerManager.Instance.IsDie) return;

        //카메라가 봐야할 위치
        if (isJump || isFly)
        {
            cameraViewPoint.x = transform.position.x;
            cameraViewPoint.z = transform.position.z;
        }
        else
        {
            cameraViewPoint.x = transform.position.x;
            cameraViewPoint.y = transform.position.y;
            cameraViewPoint.z = transform.position.z;
        }

        if (isFly)
        {
            screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, Screen.height, PlayerManager.Instance.FCamera.basicDistance));
            Vector3 viewPos = transform.position;
            if (viewPos.y > screenBounds.y - objectHeight)
                viewPos.y = screenBounds.y - objectHeight;

            if (viewPos.y > cameraViewPoint.y + playerData.flyMaxHeight)
                viewPos.y = cameraViewPoint.y + playerData.flyMaxHeight;

            transform.position = viewPos;
        }
    }

    private float GetYVelocity()
    {
        if (gc.IsGrounded())     //땅인 경우
        {
            PlayerManager.Instance.Anim.SetBool("Ground", true);

            if (gc.IsSafeGround)
                lastSafeAreaPosition = transform.position;

            if (isFly)
            {
                isFly = false;
                StartCoroutine(BreathAttackCoroutine());
            }

            isJump = false;
            PlayerManager.Instance.Anim.SetBool("Jump", false);
            isCanFly = true;
            if (jumpFlag)
            {
                //시작 y높이 
                isJump = true;
                SoundManager.Instance.PlaySFXOnce("Jump");
                PlayerManager.Instance.Anim.SetBool("Jump", true);
                jumpFlagTime = playerData.jumpFlagTime;
                return playerData.jumpPower;
            }

            return Mathf.Max(0.0f, velocity.y);
        }
        else                    //땅이 아닌 경우
        {
            PlayerManager.Instance.Anim.SetBool("Ground", false);

            if (jumpFlag)
            {
                return playerData.jumpPower;
            }
            if (flyFlag)
            {
                if (isCanFly)
                {
                    if (!isFly)
                    {
                        isFly = true;
                        flyActionDelay = playerData.flyActionDelay;
                        SoundManager.Instance.PlaySFX("Fly");
                        PlayerManager.Instance.Anim.SetTrigger("Fly");
                        PlayerManager.Instance.Anim.SetBool("Jump", false);
                        flyActiveTime = playerData.flyTime;
                        return playerData.flyPower;
                    }
                    else
                    {
                        flyActionDelay -= Time.deltaTime;
                        if (flyActionDelay <= 0)
                        {
                            flyActionDelay = playerData.flyActionDelay;
                            SoundManager.Instance.PlaySFX("Fly");
                            PlayerManager.Instance.Anim.SetTrigger("Fly");
                            return playerData.flyPower;
                        }
                    }
                }
                //비행중 위로 힘을 더 못주는 상태
                else if (isFly)
                {
                    flyActionDelay -= Time.deltaTime;
                    if (flyActionDelay <= 0)
                    {
                        flyActionDelay = playerData.flyActionDelay;
                        SoundManager.Instance.PlaySFX("Fly");
                        PlayerManager.Instance.Anim.SetTrigger("Fly");
                        return 0;
                    }
                }
            }

            float fallSpeed = velocity.y + playerData.gravity * Time.fixedDeltaTime;
            if (isFly && fallSpeed < playerData.maxFlyFallSpeed)
                fallSpeed = playerData.maxFlyFallSpeed;
            else if (fallSpeed < playerData.maxFallSpeed)
                fallSpeed = playerData.maxFallSpeed;
            return fallSpeed;
        }
    }

    public void keyMove()
    {
        if (PlayerManager.Instance.IsChange || PlayerManager.Instance.IsUnChange || PlayerManager.Instance.IsHit || PlayerManager.Instance.IsStartMotion) return;

        float h = Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow) ? 0 : Input.GetKey(KeyCode.RightArrow) ? 1 : Input.GetKey(KeyCode.LeftArrow) ? -1 : 0;
        float v = Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.DownArrow) ? 0 : Input.GetKey(KeyCode.UpArrow) ? 1 : Input.GetKey(KeyCode.DownArrow) ? -1 : 0;

        Vector3 dir = new Vector3(h, 0, v);
        dir = Camera.main.transform.TransformDirection(dir);
        dir.y = 0;
        if (gc.IsGrounded() && gc.IsOnSlope())
        {
            dir = gc.AdjustDirectionToSlope(dir);
            if (dir.y > 0) dir.y = 0;
            dir.Normalize();

            if (isLadder && dir.z < 0)
            {
                isLadder = false;
                PlayerManager.Instance.Anim.SetBool("Climb", false);
                PlayerManager.Instance.Anim.SetBool("ClimbDown", false);
                PlayerManager.Instance.Anim.speed = 1;
            }
        }
        else
        {
            dir.Normalize();
        }

        planeVelocity = dir * playerData.speed * GetMoveSpeedRatio();

        ////향하는 방향으로 균일한 속도로 회전
        //if (dir != Vector3.zero)
        //    transform.forward = Vector3.RotateTowards(transform.forward, dir, playerData.rotateSpeed * Time.deltaTime, 0f);

        if (Input.GetKeyDown(KeyCode.X) && !PlayerManager.Instance.PMouth.IsSuction)
        {
            //땅인경우 점프
            if (gc.IsGrounded())
            {
                jumpFlag = true;
            }
            //사다리인경우
            else if (isLadder)
            {
                jumpFlag = true;
                isLadder = false;
                PlayerManager.Instance.Anim.SetBool("Climb", false);
                PlayerManager.Instance.Anim.SetBool("ClimbDown", false);
                PlayerManager.Instance.Anim.speed = 1;
                //잠깐 사다리를 못타는 상태
                isCanLadder = false;
                ladderDelay = 0.3f;
            }
            //입에 아무것도 없는 경우 날기가능
            else if (PlayerManager.Instance.PMouth.Stack == PlayerMouth.MOUTHSTACK.None)
            {
                flyFlag = true;
                flyActionDelay = 0;
            }
        }
        else if (Input.GetKeyUp(KeyCode.X))
        {
            jumpFlag = false;
            flyFlag = false;
        }

        //날고있는 경우
        if (isFly)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                isFly = false;
                isCanFly = false;
                PlayerManager.Instance.Anim.SetTrigger("FlyCancel");
                StartCoroutine(BreathAttackCoroutine());
            }
        }

        //사다리
        if (isCanLadder && !isLadder && isCloseLadder && planeVelocity.z > 0)
        {
            isLadder = true;
            PlayerManager.Instance.Anim.SetBool("Climb", true);
            if (isJump)
            {
                isJump = false;
                PlayerManager.Instance.Anim.SetBool("Jump", false);
            }
            if (isFly)
            {
                isFly = false;
                PlayerManager.Instance.Anim.SetTrigger("FlyCancel");
            }
        }
    }

    float GetMoveSpeedRatio()
    {
        if (PlayerManager.Instance.ChangeType != PlayerManager.CHANGETYPE.Normal)
        {
            if (PlayerManager.Instance.PActionManager.GetCurAction().IsAction) return 0.3f;
            if (PlayerManager.Instance.PActionManager.GetCurAction().IsHardAction) return 0f;
        }
        if (PlayerManager.Instance.PMouth.IsSuction) return 0.3f;
        if (isFly) return 0.5f;
        return 1f;
    }

    IEnumerator BreathAttackCoroutine()
    {
        isBreathAttack = true;
        breathAttackDelay = 0.2f;
        //탄환 생성
        Instantiate(breathFactory, transform.position, Quaternion.identity).GetComponent<PlayerBreath>().Set(transform.forward);
        SoundManager.Instance.PlaySFXOnce("Sigh");
        while (breathAttackDelay > 0f)
        {
            breathAttackDelay -= Time.deltaTime;
            yield return null;
        }
        isBreathAttack = false;
    }

    public void Hit(Vector3 hitDir)
    {
        velocity = Vector3.zero;
        this.hitDir = hitDir;
        if (hitDir == Vector3.zero)
            transform.position = lastSafeAreaPosition;
    }

    public void Die()
    {
        Vector3 lookCameraVec = -Camera.main.transform.forward;
        lookCameraVec.y = 0;
        transform.rotation = Quaternion.LookRotation(lookCameraVec);
    }

    public void DieMovement()
    {
        cameraViewPoint = transform.position;
        //위로 크게 한번 작게 한번 튕긴다.
        transform.DOMoveY(transform.position.y + 5, 0.5f).SetEase(Ease.OutCubic).SetLoops(2, LoopType.Yoyo).SetUpdate(true).OnComplete(() =>
        {
            transform.DOMoveY(transform.position.y + 2, 0.25f).SetEase(Ease.OutCubic).SetLoops(2, LoopType.Yoyo).SetUpdate(true).OnComplete(() =>
            {
                transform.DOMoveY(transform.position.y + 0.5f, 0.1f).SetEase(Ease.OutCubic).SetLoops(2, LoopType.Yoyo).SetUpdate(true).OnComplete(() =>
                {
                    transform.position = cameraViewPoint;
                });
            });
        });

        PlayerManager.Instance.PlayerModel.transform.parent = gameoverPivot;
        gameoverPivot.DORotate(new Vector3(0, 0, -1080), 1.5f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() =>
        {
            PlayerManager.Instance.Anim.speed = 1;
            PlayerManager.Instance.Anim.SetTrigger("Die");
            gameoverPivot.DORotate(new Vector3(-70, 0, -45), 0.2f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetUpdate(true);
        });
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Item"))
        {
            Item item = collision.gameObject.GetComponent<Item>();
            if (item != null)
            {
                item.GetItem();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CameraBasic"))
        {
            PlayerManager.Instance.FCamera.State = FollowCamera.CameraState.BasicForward;
        }

        //사다리
        if (other.CompareTag("Ladder"))
        {
            isCloseLadder = true;
            ladderUpAxis = other.transform.position + Vector3.back * 0.5f;
        }

        if (other.CompareTag("ChangeScene"))
        {
            SoundManager.Instance.PlaySFX("SceneChange");
            PlayerIngameData.Instance.HP = PlayerManager.Instance.PHealth.HP;
            PlayerIngameData.Instance.ChangeType = PlayerManager.Instance.ChangeType;
            SoundManager.Instance.BGMVolume = 0;
            StartCoroutine(SceneChanger.Instance.ChangeSceneStart("Boss"));
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("CameraBasic"))
        {
            PlayerManager.Instance.FCamera.State = FollowCamera.CameraState.BasicForward;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CameraBasic"))
        {
            PlayerManager.Instance.FCamera.State = FollowCamera.CameraState.BasicRight;
        }

        //사다리
        if (other.CompareTag("Ladder"))
        {
            isLadder = false;
            isCloseLadder = false;
            PlayerManager.Instance.Anim.SetBool("Climb", false);
            PlayerManager.Instance.Anim.SetBool("ClimbDown", false);
            PlayerManager.Instance.Anim.speed = 1;
        }
    }
}