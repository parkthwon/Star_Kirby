using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    [Header("Data")]
    [SerializeField] PlayerData playerData;
    public PlayerData Data { get { return playerData; } }

    [SerializeField] FollowCamera followCamera;
    public FollowCamera FCamera { get { return followCamera; } }

    [SerializeField] GameObject playerModel;
    public GameObject PlayerModel { get { return playerModel; } }
    float defaultModelScale = 0.25f;

    Animator anim;
    public Animator Anim { get { return anim; } }

    //시작모션
    bool isStartMotion = false;
    public bool IsStartMotion { get { return isStartMotion; } }
    [SerializeField] GameObject starObj;
    public Vector3 startCameraPoint;

    //피격상태
    bool isHit = false;
    public bool IsHit { get { return isHit; } }
    float hitTime; //조작불가능시간,밀려나는 시간

    //사망
    bool isDie = false;
    public bool IsDie { get { return isDie; } }
    [SerializeField] Canvas playerUICanvas;

    //변신
    public enum CHANGETYPE
    {
        Normal,
        Pistol,
    }
    CHANGETYPE changeType;
    public CHANGETYPE ChangeType { get { return changeType; } }
    bool isChange = false;
    public bool IsChange { get { return isChange; } }
    bool isUnChange = false;
    public bool IsUnChange { get { return isUnChange; } }
    [SerializeField] TextMeshProUGUI changeNameText;
    [SerializeField] GameObject[] changeBubbles;
    [SerializeField] ParticleSystem changeEffect;

    //캐릭터 컴포넌트
    PlayerMovement playerMovement;
    public PlayerMovement PMovement { get { return playerMovement; } }
    PlayerHealth playerHealth;
    public PlayerHealth PHealth { get { return playerHealth; } }
    PlayerCoin playerCoin;
    public PlayerCoin PCoin { get { return playerCoin; } }
    PlayerActionManager playerActionManager;
    public PlayerActionManager PActionManager { get { return playerActionManager; } }
    [SerializeField] PlayerMouth playerMouth;
    public PlayerMouth PMouth { get { return playerMouth; } }

    private void Awake()
    {
        Instance = this;
        changeType = CHANGETYPE.Normal;
        playerMovement = GetComponent<PlayerMovement>();
        playerHealth = GetComponent<PlayerHealth>();
        playerCoin = GetComponent<PlayerCoin>();
        playerActionManager = GetComponent<PlayerActionManager>();

        anim = playerModel.GetComponent<Animator>();
    }

    void Start()
    {
        GameManager.Input.keyaction += KeyAction;

        playerMovement.Set(playerData);
        GameManager.Input.keyaction += playerMovement.keyMove;

        playerHealth.Set(playerData);

        playerCoin.Set();

        changeType = PlayerIngameData.Instance.ChangeType;
        NameChange();
        playerMouth.Set(changeType);
        GameManager.Input.keyaction += playerMouth.KeyAction;

        playerActionManager.Set(changeType);
        if (changeType != CHANGETYPE.Normal)
            GameManager.Input.keyaction += playerActionManager.GetCurAction().KeyAction;

        anim.updateMode = AnimatorUpdateMode.UnscaledTime;

        isStartMotion = GameManager.Instance.isStartMotion;
        if (isStartMotion)
        {
            transform.position = GameManager.Instance.startPos + new Vector3(0, 10, -5);
            startCameraPoint = GameManager.Instance.startPos;
            starObj.SetActive(true);
            anim.SetTrigger("StartMotion");
            SoundManager.Instance.PlaySFX("KirbyStar", 1f);
            transform.DOMove(GameManager.Instance.startPos, 1f).SetEase(Ease.Linear).SetDelay(1f).OnComplete(() =>
            {
                starObj.SetActive(false);
                anim.SetTrigger("StartMotionEnd");
                transform.DOMoveZ(GameManager.Instance.startPos.z + 3, 0.5f).SetEase(Ease.Linear);
                transform.DOMoveY(GameManager.Instance.startPos.y + 1.5f, 0.25f).SetEase(Ease.OutQuad).OnUpdate(() =>
                {
                    startCameraPoint = new Vector3(transform.position.x, startCameraPoint.y, transform.position.z);
                }).OnComplete(() =>
                {
                    transform.DOMoveY(GameManager.Instance.startPos.y, 0.25f).SetEase(Ease.InQuad).OnComplete(() =>
                    {
                        isStartMotion = false;
                    });
                });
            });
        }
    }

    private void Update()
    {
        if (isHit)
        {
            hitTime -= Time.deltaTime;
            if (hitTime <= 0)
                isHit = false;
        }
    }

    void KeyAction()
    {
        if (isChange || isUnChange || isStartMotion || changeType == CHANGETYPE.Normal) return;
        if (Input.GetKeyDown(KeyCode.S))
        {
            UnChange(-transform.forward, false);
        }
    }

    public void Hit(Vector3 hitDir)
    {
        isHit = true;
        hitTime = playerData.hitTime;

        followCamera.CameraShake(0.1f, 0.1f);
        playerMovement.Hit(hitDir);
    }

    public void ChangeScale(float scale)
    {
        playerModel.transform.DOScale(defaultModelScale * scale, 0.2f).SetEase(Ease.OutQuart);
    }

    #region 변신
    //변신하면 호출되는 함수(나중에 사용)
    public void Change(CHANGETYPE type, bool isSwallow = false)
    {
        if (type == changeType) return;

        if (isSwallow)
        {
            playerModel.transform.DOScaleY(defaultModelScale * 1.3f, 0.07f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                playerModel.transform.DOScale(new Vector3(2f, 0.3f, 2f) * defaultModelScale, 0.2f).SetEase(Ease.OutCubic).OnComplete(() =>
                {
                    playerModel.transform.DOScale(Vector3.one * defaultModelScale, 0.08f).SetEase(Ease.OutCubic).OnComplete(() =>
                    {
                        ChangeSet(type);
                    });
                });
            });
        }
        else
        {
            ChangeSet(type);
        }
    }

    void ChangeSet(CHANGETYPE type)
    {
        //기존 액션 해제
        if (changeType != CHANGETYPE.Normal)
            GameManager.Input.keyaction -= playerActionManager.GetCurAction().KeyAction;
        //액션정보 변경
        changeType = type;
        NameChange();
        playerMouth.Set(changeType);
        playerActionManager.Set(changeType);
        //새 액션 설정
        if (changeType != CHANGETYPE.Normal)
        {
            GameManager.Input.keyaction += playerActionManager.GetCurAction().KeyAction;
            //변신 애니메이션
            StartCoroutine(ChangeCoroutine());
        }
    }

    void NameChange()
    {
        switch (changeType)
        {
            case CHANGETYPE.Normal:
                changeNameText.text = "커비";
                break;
            case CHANGETYPE.Pistol:
                changeNameText.text = "노블 레인저";
                break;
        }
    }

    IEnumerator ChangeCoroutine()
    {
        //float s = GetViewportSize();
        Time.timeScale = 0;
        anim.SetTrigger("ChangeStart");
        playerActionManager.ChangeAnimationStart();
        yield return new WaitForSecondsRealtime(0.2f);
        SoundManager.Instance.PlaySFX("KirbyChange");
        yield return new WaitForSecondsRealtime(0.55f);
        //파티클
        Vector3 newScale = Vector3.one * GetViewportSize() * 10;
        foreach (RectTransform rect in changeEffect.GetComponentsInChildren<RectTransform>())
            rect.localScale = newScale;
        changeEffect.Play();
        yield return new WaitForSecondsRealtime(0.75f);
        ChangeEndEffect();
        anim.SetTrigger("ChangeEnd");
        yield return new WaitForSecondsRealtime(0.5f);
        ChangeEnd();
        playerActionManager.ChangeAnimationEnd();
        Time.timeScale = 1;
    }

    public void ChangeStart()
    {
        isChange = true;
        followCamera.State = FollowCamera.CameraState.Change;
        GameManager.Instance.PlayerChangeStart();
    }

    public void ChangeEndEffect()
    {
        followCamera.State = followCamera.prevState;
        GameManager.Instance.PlayerChangeEnd();
    }

    public void ChangeEnd()
    {
        isChange = false;
    }

    //변신해제
    public void UnChange(Vector3 bubbleDir, bool isHit)
    {
        if (isUnChange) return;
        if (changeType == CHANGETYPE.Normal) return;

        if (isHit)
        {
            ChangeBubble bubble = Instantiate(changeBubbles[(int)changeType - 1], transform.position + Vector3.up, Quaternion.identity).GetComponent<ChangeBubble>();
            bubble.Set(changeType, bubbleDir);
            //기존 액션 해제
            Change(CHANGETYPE.Normal);
        }
        else
        {
            isUnChange = true;
            StartCoroutine(UnChangeCoroutine(bubbleDir));
        }
    }

    IEnumerator UnChangeCoroutine(Vector3 bubbleDir)
    {
        yield return new WaitForSeconds(0.2f);
        //버블 생성
        ChangeBubble bubble = Instantiate(changeBubbles[(int)changeType - 1], transform.position + Vector3.up, Quaternion.identity).GetComponent<ChangeBubble>();
        bubble.Set(changeType, bubbleDir);
        //기존 액션 해제
        Change(CHANGETYPE.Normal);
        Vector3 newScale = Vector3.one * GetViewportSize() * 10;
        foreach (RectTransform rect in changeEffect.GetComponentsInChildren<RectTransform>())
            rect.localScale = newScale;
        changeEffect.Play();
        yield return new WaitForSeconds(0.1f);
        isUnChange = false;
    }

    float GetViewportSize()
    {
        Bounds bounds = GetComponent<Collider>().bounds;
        Vector3 min = GameManager.Instance.mainCamera.WorldToViewportPoint(bounds.min);
        Vector3 max = GameManager.Instance.mainCamera.WorldToViewportPoint(bounds.max);
        Vector3 diff = max - min;
        diff.z = 0;
        return diff.magnitude;
    }
    #endregion

    public void PlayerDie()
    {
        isDie = true;
        //애니메이션 멈춤
        anim.speed = 0;
        playerMovement.Die();
        StartCoroutine(PlayerDieCoroutine());
    }

    IEnumerator PlayerDieCoroutine()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        followCamera.State = FollowCamera.CameraState.Die;  //카메라 설정
        GameManager.Instance.PlayerDie();
        playerMovement.DieMovement();

        yield return new WaitForSecondsRealtime(2f);
        //플레이어UI캔버스의 UI우선순위를 제일 후순위로 한다.
        playerUICanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        playerUICanvas.sortingOrder = 10;

        //씬 재시작
        yield return StartCoroutine(SceneChanger.Instance.DieRestartSceneStart());

        //코인 감소 모션
        yield return playerCoin.Die();
        //씬 재시작
        SceneChanger.Instance.RestartScene();
    }
}
