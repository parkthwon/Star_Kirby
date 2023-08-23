using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class psw_AutoSnow : MonoBehaviour
{
    float currentTime;
    public float makeTime = 1f;
    public GameObject Snow;
    public Transform SnowPosition;
    public float speed = 3;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > makeTime)
        {
            GameObject snow = Instantiate(Snow);
            snow.transform.position = SnowPosition.position;

            Vector3 direction = new Vector3(1f, 0f, 0f); // X축으로 이동하는 방향

            // 방향을 조정할 수 있는 컴포넌트를 가져옴
            Rigidbody2D snowRigidbody = snow.GetComponent<Rigidbody2D>();
            if (snowRigidbody != null)
            {
                // 방향을 설정
                snowRigidbody.velocity = direction.normalized * speed;
            }

            currentTime = 0;
        }
    }
}
