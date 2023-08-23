using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class psw_EnemyDestroy : MonoBehaviour
{

    public Animator anim;
    public GameObject coin; // 활성화할 게임 오브젝트
    public GameObject particle;
    public psw_AutoSnow psw_snow;

    bool needDestroy = false;
    float destroyTime = 0f;
    float destroyDelay = 2f;

    //suction
    bool suction = false;
    public bool isStack = false;
    GameObject suctionObj = null;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (suction && !suctionObj.activeSelf)
        {
            suction = false;
            anim.speed = 1;
            psw_snow.enabled = true;
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
            psw_snow.enabled = false;
        }
        else
        {
            anim.SetTrigger("Damaged");
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
            psw_snow.enabled = true;
        }
    }
}