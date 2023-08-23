using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class psw_Enemy_1 : MonoBehaviour
{
    public GameObject target;
    public Transform Player;
    public Animator anim;
    public float attackRange = 3;
    public GameObject coin; // 활성화할 게임 오브젝트
    public GameObject particle;
    public psw_bulletFactory psw_bullet;
    // Start is called before the first frame update

    bool needDestroy = false;
    float destroyTime = 0f;
    float destroyDelay = 1f;

    //suction
    public bool isChange = false;
    bool suction = false;
    public bool isStack = false;
    GameObject suctionObj = null;

    void Start()
    {
        anim = GetComponentInChildren<Animator>(); // 애니메이터 컴포넌트 취득
        target = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (isStack) return;

        if (suction && !suctionObj.activeSelf)
        {
            suction = false;
            anim.speed = 1;
            psw_bullet.enabled = true;
        }

        this.transform.LookAt(Player);
        Vector3 v = transform.forward;
        v.y = 0;
        transform.forward = v;
        // 거리 계산
        float distance = Vector3.Distance(this.transform.position, target.transform.position);
        if (distance <= attackRange)
        {
            anim.SetTrigger("Find");
        }

        if (needDestroy)
            destroyTime += Time.deltaTime;

        if (destroyTime > destroyDelay)
        {
            needDestroy = false;

            GameObject co = Instantiate(coin);
            co.transform.position = this.transform.position;
            ItemCoin itemcoin = co.GetComponent<ItemCoin>();
            itemcoin.GetItem();

            GameObject pa = Instantiate(particle);
            pa.transform.position = this.transform.position;
            Destroy(pa, 2);
            Destroy(this.gameObject);
        }
    }

    public void SuctionDie()
    { 
        GameObject co = Instantiate(coin);
        co.transform.position = this.transform.position;
        ItemCoin itemcoin = co.GetComponent<ItemCoin>();
        itemcoin.GetItem();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Suction"))
        {
            suctionObj = other.gameObject;
            suction = true;
            anim.speed = 0;
            psw_bullet.enabled = false;
        }
        else
        {
            anim.SetTrigger("damage");
            destroyTime = 0;
            needDestroy = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Suction"))
        {
            suction = false;
            anim.speed = 1;
            psw_bullet.enabled = true;
        }
    }
}
