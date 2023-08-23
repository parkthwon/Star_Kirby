using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class psw_AutoCoin : MonoBehaviour
{
    public GameObject Coin;
    public Transform CoinPosition;
    private GameObject Player;
    CharacterController cc;
    // Start is called before the first frame update
    void Start()
    {
        Player = PlayerManager.Instance.gameObject;
        cc = Player.GetComponent<CharacterController>();
        //BoxCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            other.gameObject.SetActive(false);
        }

    }
}
