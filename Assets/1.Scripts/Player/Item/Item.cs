using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Item : MonoBehaviour
{
    public enum ITEMTYPE
    {
        None,       //물건
        Coin,       //코인
        Health,     //체력
        Change,     //변신
    }
    public ITEMTYPE itemType;

    [SerializeField] Collider coll;
    Rigidbody rigid;

    private void Awake()
    {
        //coll = GetComponent<Collider>();
        rigid = GetComponent<Rigidbody>();
    }

    public virtual void GetItem()
    {
        coll.enabled = false;
        rigid.isKinematic = true;
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        //물리 충돌 비활성화
    //        GetComponent<Collider>().enabled = false;
    //        GetComponent<Rigidbody>().isKinematic = true;
    //        Item item = collision.gameObject.GetComponent<Item>();
    //        switch (item.itemType)
    //        {
    //            case ItemType.Coin:
    //                //playerData.coin
    //                break;
    //            case ItemType.Health:
    //                //playerData.health
    //                break;
    //            case ItemType.Change:
    //                if (changeType != PlayerManager.Instance.changeType)
    //                    PlayerManager.Instance.ChangeStart();
    //                PlayerManager.Instance.Change(PlayerManager.ChangeType.Pistol);
    //                break;
    //        }
    //    }
    //}
}
