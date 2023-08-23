using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public enum CameraState
    {
        Change,
        BasicForward,
        BasicLeft,
        BasicRight,
        BossBasic,
        BossTopView,
        Die,
    }

    CameraState state;
    [System.NonSerialized] public CameraState prevState;
    public CameraState State
    {
        get { return state; }
        set
        {
            if (state == value)
                return;
            prevState = state;
            state = value;
            switch (value)
            {
                case CameraState.Change:
                    curDistance -= changeZOffset;
                    curYOffset = changeYOffset;
                    break;
                case CameraState.BasicForward:
                    curDistance = basicDistance;
                    curYOffset = defaultYOffset;
                    curAngle = basicAngle;
                    break;
                case CameraState.BasicLeft:
                    curDistance = basicDistance;
                    curYOffset = defaultYOffset;
                    curAngle = leftAngle;
                    break;
                case CameraState.BasicRight:
                    curDistance = basicDistance;
                    curYOffset = defaultYOffset;
                    curAngle = rightAngle;
                    break;
                case CameraState.BossBasic:
                    curDistance = bossBasicDistance;
                    curYOffset = defaultYOffset;
                    //보스 방향을 바라보는 벡터
                    Vector3 bossDir = (boss.position - PlayerManager.Instance.transform.position).normalized;
                    //벡터를 5도 왼쪽으로 회전
                    bossDir = Quaternion.Euler(0, -5, 0) * bossDir;
                    //회전한 벡터를 바라보는 각도로 변환
                    curAngle = Quaternion.LookRotation(bossDir).eulerAngles;
                    //보스전 각도로 바라보기
                    curAngle.x = bossBasicAngleOffset;
                    break;
                case CameraState.BossTopView:
                    curDistance = bossTopViewDistance;
                    curYOffset = defaultYOffset;
                    curAngle = transform.rotation.eulerAngles;
                    curAngle.x = bossTopViewAngleOffset;
                    break;
                case CameraState.Die:
                    curDistance = dieDistance;
                    curYOffset = dieYOffset;
                    curAngle = transform.rotation.eulerAngles;
                    curAngle.x = dieAngleOffset;
                    break;
            }
            curOffset = Quaternion.Euler(curAngle) * Vector3.back * curDistance + Vector3.up * curYOffset;
        }
    }

    //카메라 거리
    public readonly float basicDistance = 24f;
    readonly float changeZOffset = 6f;
    readonly float bossBasicDistance = 18f;
    readonly float bossTopViewDistance = 30f;
    readonly float dieDistance = 6;

    //카메라 각도
    readonly Vector3 basicAngle = new Vector3(20, 0, 0);  //바라보는 각도
    readonly Vector3 leftAngle = new Vector3(20, 45, 0);  //왼쪽으로 바라보는 각도
    readonly Vector3 rightAngle = new Vector3(20, -45, 0);  //왼쪽으로 바라보는 각도
    readonly float bossBasicAngleOffset = 10f;  //보스전시 바라보는 각도
    readonly float bossTopViewAngleOffset = 50f;  //보스전시 바라보는 각도
    readonly float dieAngleOffset = 5;

    //카메라가 타겟을 바라보는 위치 높이보정
    readonly float defaultYOffset = 1.5f;
    readonly float changeYOffset = 0.5f;
    readonly float dieYOffset = 0.5f;

    float curDistance;
    Vector3 curAngle;
    Vector3 curOffset;
    float curYOffset;

    //보스전
    Transform boss;
    Transform bossGround;

    // Start is called before the first frame update
    void Start()
    {
        State = CameraState.BasicForward;

        //transform.rotation = Quaternion.Euler(curAngle);
        //transform.position = targetPlayer.position + curOffset;
    }

    //변신할때만 사용
    private void LateUpdate()
    {
        if (PlayerManager.Instance.IsChange)
        {
            transform.position = Vector3.Lerp(transform.position, PlayerManager.Instance.PMovement.CameraViewPoint + curOffset, Time.unscaledDeltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(curAngle), Time.unscaledDeltaTime * 5);
        }

        if (PlayerManager.Instance.IsDie)
        {
            transform.position = Vector3.Lerp(transform.position, PlayerManager.Instance.PMovement.CameraViewPoint + curOffset, Time.unscaledDeltaTime * 1.5f);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(curAngle), Time.unscaledDeltaTime * 1.5f);
        }
    }

    private void FixedUpdate()
    {
        Vector3 targetPos;
        if (PlayerManager.Instance.IsStartMotion)
        {
            targetPos = PlayerManager.Instance.startCameraPoint;
            transform.position = Vector3.Lerp(transform.position, targetPos + curOffset, Time.fixedDeltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(curAngle), Time.fixedDeltaTime * 5);
            return;
        }

        targetPos = PlayerManager.Instance.PMovement.CameraViewPoint;
        if (state == CameraState.BossBasic)
        {
            //보스 방향을 바라보는 벡터
            Vector3 bossDir = (boss.position - PlayerManager.Instance.transform.position).normalized;
            //벡터를 5도 왼쪽으로 회전
            bossDir = Quaternion.Euler(0, -5, 0) * bossDir;
            //회전한 벡터를 바라보는 각도로 변환
            curAngle = Quaternion.LookRotation(bossDir).eulerAngles;
            //보스전 각도로 바라보기
            curAngle.x = bossBasicAngleOffset;
            curOffset = Quaternion.Euler(curAngle) * Vector3.back * curDistance + Vector3.up * curYOffset;
        }

        if (state == CameraState.BossTopView)
        {
            targetPos = (PlayerManager.Instance.PMovement.CameraViewPoint * 2 + boss.position + bossGround.position) / 4;
            targetPos.y = PlayerManager.Instance.PMovement.CameraViewPoint.y;
        }

        transform.position = Vector3.Lerp(transform.position, targetPos + curOffset, Time.fixedDeltaTime * 5);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(curAngle), Time.fixedDeltaTime * 5);
    }

    public void SetBossBasic(Transform boss)
    {
        this.boss = boss;
        State = CameraState.BossBasic;
    }

    public void SetBossTopView(Transform boss, Transform bossGround)
    {
        this.boss = boss;
        this.bossGround = bossGround;
        State = CameraState.BossTopView;
    }

    #region CameraShake
    /// <summary>
    /// 카메라 쉐이크
    /// </summary>
    /// <param name="shakePower"></param>
    /// <param name="shakeTime"></param>
    public void CameraShake(float shakePower, float shakeTime)
    {
        StartCoroutine(CameraShakeCoroutine(shakePower, shakeTime));
    }

    IEnumerator CameraShakeCoroutine(float shakePower, float shakeTime)
    {
        Vector3 originPos = transform.position;
        float curTime = 0f;
        while (curTime < shakeTime)
        {
            transform.position = originPos + Random.insideUnitSphere * shakePower;
            curTime += Time.unscaledDeltaTime;
            yield return null;
        }
        transform.position = originPos;
    }

    /// <summary>
    /// 카메라 쉐이크 한번만
    /// </summary>
    /// <param name="shakePower"></param>
    public void CameraShakeOnce(float shakePower)
    {
        StartCoroutine(CameraShakeOnceCoroutine(shakePower));
    }

    IEnumerator CameraShakeOnceCoroutine(float shakePower)
    {
        Vector3 originPos = transform.position;
        transform.position = originPos + Random.insideUnitSphere * shakePower;
        yield return null;
        transform.position = originPos;
    }

    /// <summary>
    /// 카메라 쉐이크 위방향으로
    /// </summary>
    /// <param name="shakePower"></param>
    public void CameraShakeUpOnce(float shakePower)
    {
        StartCoroutine(CameraShakeUpOnceCoroutine(shakePower));
    }

    IEnumerator CameraShakeUpOnceCoroutine(float shakePower)
    {
        Vector3 originPos = transform.position;
        transform.position = originPos + transform.up * shakePower;
        yield return null;
        transform.position = originPos;
    }
    #endregion
}
