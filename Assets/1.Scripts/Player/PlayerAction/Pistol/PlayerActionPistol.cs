using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActionPistol : PlayerAction
{
    [SerializeField] Transform rightPistol;
    [SerializeField] Transform leftPistol;
    [SerializeField] Transform rightFirePos;
    [SerializeField] Transform leftFirePos;
    [SerializeField] LineRenderer rightLine;
    [SerializeField] LineRenderer leftLine;
    [SerializeField] GameObject bulletFactory;

    [SerializeField] RectTransform aimRT;
    [SerializeField] Image imageAim;
    [SerializeField] Image imageLockOn;

    //변신 총 위치
    [SerializeField] Transform kirbyBoneArmRight;
    [SerializeField] Transform kirbyBoneArmLeft;

    enum CHARGELEVEL
    {
        None,
        Level1,
        Level2,
        Level3
    }
    CHARGELEVEL chargeLevel = CHARGELEVEL.None;

    //공격 시전중
    bool isFire = false;

    //발사키 입력중
    bool isFireKey = false;
    //차지 시간
    float chargeTime = 0.0f;
    float charge1Time = 0.5f;
    float charge2Time = 1.0f;
    float charge3Time = 1.5f;
    [SerializeField] SpriteRenderer[] chargeSpriteRenderer;
    [SerializeField] Sprite[] chargeSprite;

    //에임 활성화
    bool isAim = false;
    //에임 이동 방향
    Vector3 aimDir = Vector3.zero;
    //에임 이동 속도
    float aimSpeed = 3f;
    //오른쪽 에임 방향
    Vector3 rightAimDir = Vector3.zero;
    //왼쪽 에임 방향
    Vector3 leftAimDir = Vector3.zero;

    //락온
    bool isLockOn = false;
    //락온 거리
    float lockOnDistance = 50f;
    //락온 타겟
    Collider lockOnTarget = null;
    //락온 해제 벡터
    Vector3 lockOnReleaseVec = Vector3.zero;
    //락온 해제 거리
    float lockOnReleaseDistance = 60f;
    Coroutine lockOnCoroutine = null;

    //총 장착상태
    bool isEquip = false;
    float equipTime = 1f;
    float equipDelay = 1f;
    readonly Vector3 rightPistolEquipPos = new Vector3(0.4f, 0.5f, 0.55f);
    readonly Vector3 rightPistolUnEquipPos = new Vector3(0.18f, 0.5f, -0.5f);
    readonly Vector3 rightPistolEquipRot = new Vector3(0, -90, 0);
    readonly Vector3 rightPistolUnEquipRot = new Vector3(0, 0, 45);
    readonly Vector3 leftPistolEquipPos = new Vector3(-0.4f, 0.5f, 0.55f);
    readonly Vector3 leftPistolUnEquipPos = new Vector3(-0.18f, 0.5f, -0.5f);
    readonly Vector3 leftPistolEquipRot = new Vector3(0, -90, 0);
    readonly Vector3 leftPistolUnEquipRot = new Vector3(0, -180, 45);


    // Update is called once per frame
    void Update()
    {
        //인풋이 없는 경우
        if (!GameManager.Input.isInput)
        {
            if (isFireKey)
                Fire();

            if (isAim) aimDir = Vector3.zero;
        }

        if (isFireKey)
        {
            equipTime = 0f;
            chargeTime += Time.deltaTime;
            switch (chargeLevel)
            {
                case CHARGELEVEL.None:
                    if (chargeTime > charge1Time)
                    {
                        chargeLevel = CHARGELEVEL.Level1;
                        chargeSpriteRenderer[0].gameObject.SetActive(true);
                        chargeSpriteRenderer[0].transform.LookAt(Camera.main.transform);
                        chargeSpriteRenderer[0].sprite = chargeSprite[0];
                        chargeSpriteRenderer[0].color = Color.yellow;
                        chargeSpriteRenderer[1].gameObject.SetActive(true);
                        chargeSpriteRenderer[1].transform.LookAt(Camera.main.transform);
                        chargeSpriteRenderer[1].sprite = chargeSprite[0];
                        chargeSpriteRenderer[1].color = Color.yellow;
                        //에임 활성화
                        if (!isAim) ActiveAim();
                    }
                    break;
                case CHARGELEVEL.Level1:
                    {
                        //차지 연출
                        //노란색원
                        //0.3->0.1
                        float percent = 1 - (chargeTime - charge1Time) / 0.2f;
                        if (percent < 0) percent = 0;
                        chargeSpriteRenderer[0].transform.LookAt(Camera.main.transform);
                        chargeSpriteRenderer[0].transform.localScale = Vector3.one * (percent * 0.2f + 0.1f);
                        chargeSpriteRenderer[1].transform.LookAt(Camera.main.transform);
                        chargeSpriteRenderer[1].transform.localScale = Vector3.one * (percent * 0.2f + 0.1f);

                        if (chargeTime > charge2Time)
                        {
                            chargeLevel = CHARGELEVEL.Level2;
                            chargeSpriteRenderer[0].color = Color.white;
                            chargeSpriteRenderer[1].color = Color.white;
                        }
                    }
                    break;
                case CHARGELEVEL.Level2:
                    {
                        //차지 연출
                        //하얀색원
                        //0.4->0.1
                        float percent = 1 - (chargeTime - charge2Time) / 0.2f;
                        if (percent < 0) percent = 0;
                        chargeSpriteRenderer[0].transform.LookAt(Camera.main.transform);
                        chargeSpriteRenderer[0].transform.localScale = Vector3.one * (percent * 0.3f + 0.1f);
                        chargeSpriteRenderer[1].transform.LookAt(Camera.main.transform);
                        chargeSpriteRenderer[1].transform.localScale = Vector3.one * (percent * 0.3f + 0.1f);

                        if (chargeTime > charge3Time)
                        {
                            chargeLevel = CHARGELEVEL.Level3;
                            chargeSpriteRenderer[0].sprite = chargeSprite[1];
                            chargeSpriteRenderer[1].sprite = chargeSprite[1];
                        }
                    }
                    break;
                case CHARGELEVEL.Level3:
                    {
                        //차지 연출
                        //하얀색별
                        //0.5->0.1
                        float percent = 1 - (chargeTime - charge3Time) / 0.2f;
                        if (percent < 0) percent = 0;
                        chargeSpriteRenderer[0].transform.LookAt(Camera.main.transform);
                        chargeSpriteRenderer[0].transform.localScale = Vector3.one * (percent * 0.4f + 0.1f);
                        chargeSpriteRenderer[1].transform.LookAt(Camera.main.transform);
                        chargeSpriteRenderer[1].transform.localScale = Vector3.one * (percent * 0.4f + 0.1f);
                    }
                    break;
            }

            //에임 활성화 상태
            if (isAim)
                UpdateAim();
        }

        //총 위치 변경
        if (isEquip && equipTime < equipDelay)
        {
            equipTime += Time.deltaTime;
            if (equipTime > equipDelay)
                UnEquip();
        }
    }

    public override void Set()
    {
        gameObject.SetActive(true);
    }

    public override void Unset()
    {
        isFire = false;
        IsAction = false;
        IsHardAction = false;
        isFireKey = false;
        DeactiveAim();
        chargeTime = 0;
        chargeLevel = CHARGELEVEL.None;
        gameObject.SetActive(false);
    }

    public override void KeyAction()
    {
        if (isFire) return;
        if (PlayerManager.Instance.IsChange || PlayerManager.Instance.IsUnChange || PlayerManager.Instance.IsHit || PlayerManager.Instance.IsStartMotion) return;
        if (PlayerManager.Instance.PMovement.IsFly || PlayerManager.Instance.PMovement.IsBreathAttack) return;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (!isEquip) Equip();
            isFireKey = true;
            IsHardAction = true;
            isAim = false;
            isLockOn = false;
            chargeLevel = CHARGELEVEL.None;
            rightLine.enabled = false;
            leftLine.enabled = false;
            imageAim.gameObject.SetActive(false);
            chargeTime = 0;
        }
        else if (Input.GetKeyUp(KeyCode.Z))
        {
            Fire();
        }

        //에임 이동
        if (isAim)
        {
            float h = Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow) ? 0 : Input.GetKey(KeyCode.RightArrow) ? 1 : Input.GetKey(KeyCode.LeftArrow) ? -1 : 0;
            float v = Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.DownArrow) ? 0 : Input.GetKey(KeyCode.UpArrow) ? 1 : Input.GetKey(KeyCode.DownArrow) ? -1 : 0;
            aimDir = new Vector3(h, v, 0);
            aimDir.Normalize();
            aimDir *= aimSpeed;
        }
    }

    #region 에임
    void ActiveAim()
    {
        isAim = true;
        aimRT.gameObject.SetActive(true);
        imageAim.gameObject.SetActive(true);
        imageAim.color = Color.white;
        imageLockOn.gameObject.SetActive(false);
        imageLockOn.color = Color.red;
        rightLine.enabled = true;
        leftLine.enabled = true;
        //에임 초기위치 설정
        //플레이어의 정면에서 최대 10미터까지 떨어진 곳
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 10f))
        {
            //월드좌표를 스크린좌표로 변환
            Vector3 imagePos = Camera.main.WorldToScreenPoint(hitInfo.point);
            aimRT.position = imagePos;
        }
        else
        {
            Vector3 imagePos = Camera.main.WorldToScreenPoint(ray.GetPoint(10f));
            aimRT.position = imagePos;
        }
    }

    void DeactiveAim()
    {
        isAim = false;
        aimRT.gameObject.SetActive(false);
        imageAim.gameObject.SetActive(false);
        imageLockOn.gameObject.SetActive(false);
        chargeSpriteRenderer[0].gameObject.SetActive(false);
        chargeSpriteRenderer[1].gameObject.SetActive(false);
        rightLine.enabled = false;
        leftLine.enabled = false;
    }

    void UpdateAim()
    {
        if (isLockOn)
        {
            lockOnReleaseVec += aimDir;
            if (lockOnReleaseVec.magnitude > lockOnReleaseDistance)
                LockOnRelease();
            else if (lockOnTarget != null)
            {
                UpdateLockOn();
                UpdateAimPlayer(lockOnTarget.bounds.center);
            }
        }
        else
        {
            aimRT.position += aimDir;
            Ray ray = Camera.main.ScreenPointToRay(aimRT.position);
            // Raycast를 수행하여 가장 먼저 만나는 Collider를 찾습니다.
            RaycastHit[] hits = Physics.RaycastAll(ray);
            Array.Sort(hits, (RaycastHit x, RaycastHit y) => x.distance.CompareTo(y.distance));
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.isTrigger || hit.collider.CompareTag("Player")) continue;

                Debug.Log(hit.collider.name);
                UpdateAimPlayer(hit.point);
                if (hit.transform.CompareTag("Enemy") || hit.transform.CompareTag("Boss"))
                {
                    Collider collider = hit.collider;
                    //콜라이더의 중심점을 구한다.
                    Vector3 center = collider.bounds.center;
                    //중심점의 위치를 구한다.
                    Vector3 centerPos = Camera.main.WorldToScreenPoint(center);
                    centerPos.z = 0;
                    Vector3 imagePos = aimRT.position;
                    imagePos.z = 0;

                    //중심점과 광선과의 거리를 구한다.
                    float distance = Vector3.Distance(centerPos, imagePos);
                    if (distance < lockOnDistance)
                        LockOn(hit.collider);
                }

                break;
            }
        }
    }

    void UpdateAimPlayer(Vector3 lookPoint)
    {
        //에임 방향을보는 캐릭터의 Y축회전각도 구하기
        Vector3 dir = lookPoint - PlayerManager.Instance.transform.position;
        dir.Normalize();
        float rotY = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        PlayerManager.Instance.PMovement.NextFixedRotation = Quaternion.Euler(0, rotY, 0);

        //라인그리기
        rightLine.SetPosition(0, rightFirePos.position);
        rightLine.SetPosition(1, lookPoint + Vector3.right * 0.1f);
        leftLine.SetPosition(0, leftFirePos.position);
        leftLine.SetPosition(1, lookPoint + Vector3.left * 0.1f);

        //발사 방향 설정
        rightAimDir = lookPoint + Vector3.right * 0.1f - rightFirePos.position;
        rightAimDir.Normalize();
        leftAimDir = lookPoint + Vector3.left * 0.1f - leftFirePos.position;
        leftAimDir.Normalize();
    }

    /// <summary>
    /// 락온기능
    /// </summary>
    void LockOn(Collider lockOnCollider)
    {
        isLockOn = true;
        lockOnTarget = lockOnCollider;
        lockOnReleaseVec = Vector3.zero;
        if (lockOnCoroutine != null)
            StopCoroutine(lockOnCoroutine);
        lockOnCoroutine = StartCoroutine(LockOnCoroutine());
    }

    IEnumerator LockOnCoroutine()
    {
        float lockOnTime = 0;
        imageLockOn.gameObject.SetActive(true);
        //에임 이미지를 락온타겟중심으로 이동
        while (true)
        {
            yield return null;
            if (lockOnTarget == null)
                break;

            aimRT.position = Vector3.Lerp(aimRT.position, Camera.main.WorldToScreenPoint(lockOnTarget.bounds.center), 0.05f);
            imageAim.color = new Color(1, 1, 1, 1 - lockOnTime * 2);
            imageLockOn.color = new Color(1, 0, 0, lockOnTime * 2);
            lockOnTime += Time.deltaTime;
            if (lockOnTime > 0.5f)
                break;
        }

        imageAim.gameObject.SetActive(false);
        aimRT.position = Camera.main.WorldToScreenPoint(lockOnTarget.bounds.center);
    }

    /// <summary>
    /// 락온 업데이트
    /// </summary>
    void UpdateLockOn()
    {
        if (lockOnTarget != null)
            aimRT.position = Camera.main.WorldToScreenPoint(lockOnTarget.bounds.center);
    }

    /// <summary>
    /// 락온 해제
    /// </summary>
    void LockOnRelease()
    {
        aimRT.position = Camera.main.WorldToScreenPoint(lockOnTarget.bounds.center) + lockOnReleaseVec;  //거리보정
        isLockOn = false;
        lockOnTarget = null;
        if (lockOnCoroutine != null)
            StopCoroutine(lockOnCoroutine);
        lockOnCoroutine = StartCoroutine(LockOnReleaseCoroutine());

    }

    IEnumerator LockOnReleaseCoroutine()
    {
        float lockOnTime = 0;
        imageAim.gameObject.SetActive(true);
        //에임 이미지를 락온타겟중심으로 이동
        while (true)
        {
            yield return null;
            imageAim.color = new Color(1, 1, 1, lockOnTime * 2);
            imageLockOn.color = new Color(1, 0, 0, 1 - lockOnTime * 2);
            lockOnTime += Time.deltaTime;
            if (lockOnTime > 0.5f)
                break;
        }
        imageLockOn.gameObject.SetActive(false);
    }
    #endregion

    #region 발사
    void Fire()
    {
        switch (chargeLevel)
        {
            case CHARGELEVEL.Level1:
                StartCoroutine(ChargeFireCoroutine(3));
                break;
            case CHARGELEVEL.Level2:
                StartCoroutine(ChargeFireCoroutine(5));
                break;
            case CHARGELEVEL.Level3:
                StartCoroutine(ChargeFireCoroutine(9));
                break;
            default:
                StartCoroutine(BasicFireCoroutine());
                break;
        }

        isFire = true;
        isFireKey = false;
        DeactiveAim();
        chargeTime = 0;
        chargeLevel = CHARGELEVEL.None;
    }

    IEnumerator BasicFireCoroutine()
    {
        IsAction = true;
        IsHardAction = false;
        yield return new WaitForSeconds(0.1f);
        Instantiate(bulletFactory, rightFirePos.position, Quaternion.identity).GetComponent<PistolBullet>().Set(transform.forward);
        RightPistolRecoil();
        SoundManager.Instance.PlaySFX("Shot");
        yield return new WaitForSeconds(0.1f);
        Instantiate(bulletFactory, leftFirePos.position, Quaternion.identity).GetComponent<PistolBullet>().Set(transform.forward);
        LeftPistolRecoil();
        SoundManager.Instance.PlaySFX("Shot");
        isFire = false;
        IsAction = false;
        IsHardAction = false;
    }

    IEnumerator ChargeFireCoroutine(int bullet)
    {
        for (int i = 0; i < bullet; i++)
        {
            yield return new WaitForSeconds(0.1f);
            PistolBullet pistolBullet = Instantiate(bulletFactory).GetComponent<PistolBullet>();
            SoundManager.Instance.PlaySFX("Shot");
            if (i % 2 == 0)
            {
                pistolBullet.transform.position = rightFirePos.position;
                if (isLockOn)
                    pistolBullet.Set(lockOnTarget);
                else
                    pistolBullet.Set(rightAimDir);
                RightPistolRecoil();
            }
            else
            {
                pistolBullet.transform.position = leftFirePos.position;
                if (isLockOn)
                    pistolBullet.Set(lockOnTarget);
                else
                    pistolBullet.Set(leftAimDir);
                LeftPistolRecoil();
            }
            PlayerManager.Instance.FCamera.CameraShakeUpOnce(0.1f);
        }
        isFire = false;
        IsHardAction = false;
    }
    #endregion

    #region Gun position (Anim)
    public override void ChangeAnimationStart()
    {
        rightPistol.parent = kirbyBoneArmRight;
        rightPistol.localScale = new Vector3(0.04f, 0.04f, 0.04f);
        rightPistol.localPosition = new Vector3(0, 0.01f, 0);
        rightPistol.localRotation = Quaternion.Euler(new Vector3(0, 0, 90));

        leftPistol.parent = kirbyBoneArmLeft;
        leftPistol.localScale = new Vector3(0.04f, 0.04f, 0.04f);
        leftPistol.localPosition = new Vector3(0, 0.01f, 0);
        leftPistol.localRotation = Quaternion.Euler(new Vector3(180, 0, -90));
    }

    public override void ChangeAnimationEnd()
    {
        rightPistol.parent = transform;
        rightPistol.localScale = Vector3.one;
        leftPistol.parent = transform;
        leftPistol.localScale = Vector3.one;
        UnEquipImmediately();
    }

    //장착
    void Equip()
    {
        isEquip = true;
        //총을 손 위치로 이동
        rightPistol.DOKill();
        rightPistol.localRotation = Quaternion.Euler(rightPistolEquipRot);
        rightPistol.DOLocalMove(rightPistolEquipPos, 0.1f).From(new Vector3(0.6f, 0.5f, -0.05f)).SetEase(Ease.Linear);

        leftPistol.DOKill();
        leftPistol.localRotation = Quaternion.Euler(leftPistolEquipRot);
        leftPistol.DOLocalMove(leftPistolEquipPos, 0.1f).From(new Vector3(-0.6f, 0.5f, -0.05f)).SetEase(Ease.Linear);
    }

    void UnEquip()
    {
        isEquip = false;

        //오른손총 원위치
        rightPistol.DOLocalRotate(rightPistolUnEquipRot, 0.2f).SetEase(Ease.Linear);
        rightPistol.DOLocalMove(rightPistolUnEquipPos, 0.2f).SetEase(Ease.Linear);

        //왼손총 원위치
        leftPistol.DOLocalRotate(leftPistolUnEquipRot, 0.2f).SetDelay(0.2f).SetEase(Ease.Linear);
        leftPistol.DOLocalMove(leftPistolUnEquipPos, 0.2f).SetDelay(0.2f).SetEase(Ease.Linear);
    }

    void UnEquipImmediately()
    {
        isEquip = false;

        //오른손총 원위치
        rightPistol.DOKill();
        rightPistol.localRotation = Quaternion.Euler(rightPistolUnEquipRot);
        rightPistol.localPosition = rightPistolUnEquipPos;

        //왼손총 원위치
        leftPistol.DOKill();
        leftPistol.localRotation = Quaternion.Euler(leftPistolUnEquipRot);
        leftPistol.localPosition = leftPistolUnEquipPos;
    }

    void RightPistolRecoil()
    {
        //위치 초기화
        rightPistol.DOKill();
        //rightPistol.localRotation = Quaternion.Euler(rightPistolEquipRot);
        //rightPistol.localPosition = rightPistolEquipPos;
        rightPistol.DOLocalRotate(new Vector3(rightPistolEquipRot.x, rightPistolEquipRot.y, 50), 0.08f).SetEase(Ease.OutBack).OnComplete(() =>
        { rightPistol.DOLocalRotate(rightPistolEquipRot, 0.05f).SetDelay(0.05f).SetEase(Ease.Linear); });
    }

    void LeftPistolRecoil()
    {
        //위치 초기화
        leftPistol.DOKill();
        //leftPistol.localRotation = Quaternion.Euler(leftPistolEquipRot);
        //leftPistol.localPosition = leftPistolEquipPos;
        leftPistol.DOLocalRotate(new Vector3(leftPistolEquipRot.x, leftPistolEquipRot.y, 50), 0.08f).SetEase(Ease.OutBack).OnComplete(() =>
        { leftPistol.DOLocalRotate(leftPistolEquipRot, 0.05f).SetDelay(0.05f).SetEase(Ease.Linear); });
    }
    #endregion
}
