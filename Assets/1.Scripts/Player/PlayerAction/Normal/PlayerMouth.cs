using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMouth : MonoBehaviour
{
    public enum MOUTHSTACK
    {
        None,
        Object,
    }
    MOUTHSTACK stack = MOUTHSTACK.None;
    public MOUTHSTACK Stack { get { return stack; } }
    Stack<GameObject> stackList = new Stack<GameObject>();

    [SerializeField] GameObject suction;
    [SerializeField] ParticleSystem suctionEffect;
    bool canUse = true;
    bool isSuction = false;
    public bool IsSuction
    {
        get { return isSuction; }
        set
        {
            if (isSuction == value) return;
            isSuction = value;

            if (value)
            {
                suction.SetActive(true);
                suctionEffect.Play();
                audioSource.Play();
            }
            else
            {
                suction.SetActive(false);
                suctionEffect.Stop();
                audioSource.Stop();
            }
        }
    }
    bool canSuction = true;
    float suctionDelay = 0f;

    //입 위치
    [SerializeField] Transform mouthTransform;
    //먹은 물건
    GameObject stackObject;

    //사운드(구간반복을 위해 별도배치)
    AudioSource audioSource;
    [SerializeField] float audioStartPoint;
    [SerializeField] float audioEndPoint;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        //인풋이 없는 경우
        if (!GameManager.Input.isInput)
            IsSuction = false;

        //빨아들이기 사운드
        if (isSuction)
        {
            if (audioSource.time > audioEndPoint)
                audioSource.time = audioStartPoint;
        }

        //빨아들이기 쿨타임
        if (!canSuction)
        {
            suctionDelay -= Time.deltaTime;
            if (suctionDelay < 0f)
                canSuction = true;
        }
    }

    public void Set(PlayerManager.CHANGETYPE type)
    {
        if (type == PlayerManager.CHANGETYPE.Normal)
            canUse = true;
        else
            canUse = false;
    }

    public void KeyAction()
    {
        if (PlayerManager.Instance.IsChange || PlayerManager.Instance.IsUnChange || PlayerManager.Instance.IsHit || PlayerManager.Instance.IsStartMotion) return;
        if (PlayerManager.Instance.PMovement.IsFly || PlayerManager.Instance.PMovement.IsBreathAttack) return;
        if (!canUse) return;

        if (stack != MOUTHSTACK.None)
        {
            //삼키기
            if (Input.GetKeyDown(KeyCode.A))
            {
                switch (stack)
                {
                    case MOUTHSTACK.Object:
                        PlayerManager.Instance.ChangeScale(1f);
                        Destroy(stackObject);
                        break;
                        //case Stack.Enemy_Pistol:
                        //    break;
                }
                stack = MOUTHSTACK.None;
            }

            //뱉기(발사만 가능함)
            if (Input.GetKeyDown(KeyCode.Z))
            {
                switch (stack)
                {
                    case MOUTHSTACK.Object:
                        //바라보는 방향으로 물건 뱉기
                        GameObject playerBullet = new GameObject("PlayerBullet");
                        while (stackList.Count > 0)
                        {
                            GameObject child = stackList.Pop();
                            child.SetActive(true);
                            child.transform.parent = playerBullet.transform;
                            child.transform.localPosition = Vector3.zero;
                        }
                        playerBullet.transform.position = mouthTransform.position + transform.forward;
                        playerBullet.AddComponent<PlayerNormalBullet>().Set(transform.forward);

                        //물건을 뱉은 후 쿨타임
                        canSuction = false;
                        suctionDelay = PlayerManager.Instance.Data.suctionDelay;

                        //크기 원상복구
                        PlayerManager.Instance.ChangeScale(1f);
                        break;
                }
                stack = MOUTHSTACK.None;
            }
        }
        else
        {
            //빨아들이기
            if (Input.GetKey(KeyCode.Z) && canSuction)
                IsSuction = true;
            else
                IsSuction = false;
        }
    }

    public void SetStack(GameObject suctionObejct)
    {
        //해당 오브젝트가 물건인 경우
        if (suctionObejct.layer == LayerMask.NameToLayer("MoveableObj") || suctionObejct.layer == LayerMask.NameToLayer("Star"))
        {
            //태그가 버블인 경우
            if (suctionObejct.CompareTag("Bubble"))
            {
                if (PlayerManager.Instance.ChangeType == PlayerManager.CHANGETYPE.Normal)
                {
                    PlayerManager.Instance.PMouth.IsSuction = false;
                    suctionObejct.GetComponent<ChangeBubble>().GetItem();
                }
                Destroy(suctionObejct);
            }
            else
            {
                if (suctionObejct.layer == LayerMask.NameToLayer("Star"))
                {
                    suctionObejct.GetComponent<psw_starrrrr>().enabled = false;
                }
                AddStack(suctionObejct);
            }
        }
        else if (suctionObejct.layer == LayerMask.NameToLayer("Enemy"))
        {
            psw_Enemy_1 enemy1 = suctionObejct.GetComponent<psw_Enemy_1>();
            psw_EnemyDestroy enemy2 = suctionObejct.GetComponent<psw_EnemyDestroy>();

            if (enemy1 != null)
            {
                enemy1.isStack = true;
                if (enemy1.isChange)
                {
                    PlayerManager.Instance.ChangeStart();
                    PlayerManager.Instance.Change(PlayerManager.CHANGETYPE.Pistol, true);
                    Destroy(suctionObejct);
                }
                else
                {
                    AddStack(suctionObejct);
                }
            }
            else if (enemy2 != null)
            {
                enemy2.isStack = true;
                AddStack(suctionObejct);
            }
        }
    }

    void AddStack(GameObject suctionObejct)
    {
        stack = MOUTHSTACK.Object;
        stackObject = suctionObejct;
        //물건을 비활성화 상태로 가지고 있는다.
        suctionObejct.transform.parent = transform;
        suctionObejct.transform.localPosition = Vector3.zero;
        suctionObejct.SetActive(false);
        PlayerManager.Instance.ChangeScale(1.2f);

        stackList.Push(suctionObejct);

        if (isSuction)
            IsSuction = false;
    }
}
