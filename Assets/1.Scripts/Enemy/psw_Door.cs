using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class psw_Door : MonoBehaviour
{
    public float speed = 5;
    public GameObject target;
    public GameObject pswcamera;
    public GameObject[] coins;
    public GameObject spawnEffect;

    // Start is called before the first frame update
    void Start()
    {

    }

    Vector3 dir = Vector3.up;
    float currentTime;
    bool CanDoor = true;
    // Update is called once per frame
    void Update()
    {

        // 태어날 때 Mark를 찾고 싶다.
        target = GameObject.Find("Mark");
        if (target.GetComponent<MeshRenderer>().enabled == false && CanDoor)
        {
            Time.timeScale = 0;
            PlayerManager.Instance.Anim.updateMode = AnimatorUpdateMode.Normal;
            dir = Vector3.up;
            transform.position += dir * speed * Time.unscaledDeltaTime;
            currentTime += Time.unscaledDeltaTime;
            pswcamera.SetActive(true);
            if (currentTime > 2)
            {
                pswcamera.SetActive(false);
                Time.timeScale = 1;
                PlayerManager.Instance.Anim.updateMode = AnimatorUpdateMode.UnscaledTime;
                CanDoor = false;
                currentTime = 0;
            }
        }
        if (CanDoor == false)
        {
            currentTime += Time.deltaTime;

            if (coins[0] != null)
                if (currentTime > 2f && !coins[0].activeSelf)
                {
                    coins[0].SetActive(true);
                    Instantiate(spawnEffect, coins[0].transform.position, Quaternion.identity);
                }
            if (coins[1] != null)
                if (currentTime > 2.2f && !coins[1].activeSelf)
                {
                    coins[1].SetActive(true);
                    Instantiate(spawnEffect, coins[1].transform.position, Quaternion.identity);
                }
            if (coins[2] != null)
                if (currentTime > 2.4f && !coins[2].activeSelf)
                {
                    coins[2].SetActive(true);
                    Instantiate(spawnEffect, coins[2].transform.position, Quaternion.identity);
                }
            if (coins[3] != null)
                if (currentTime > 2.6f && !coins[3].activeSelf)
                {
                    coins[3].SetActive(true);
                    Instantiate(spawnEffect, coins[3].transform.position, Quaternion.identity);
                }
            if (coins[4] != null)
                if (currentTime > 2.8f && !coins[4].activeSelf)
                {
                    coins[4].SetActive(true);
                    Instantiate(spawnEffect, coins[4].transform.position, Quaternion.identity);
                }
        }
    }
}
