using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class psw_Mark : MonoBehaviour
{
    public float speed = 5;
    public float currentTime = 0;
    public float delaytime = 1;
    private Vector3 currentPosition;
    bool isUp = false;
    public GameObject door;
    public GameObject psw_camera;
    BoxCollider boxCollider;
    // Start is called before the first frame update
    void Start()
    {
        currentPosition = transform.position;
        boxCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > delaytime)
        {
            if (isUp == true)
            {
                isUp = false;
            }   
            else
            {
                isUp = true;
            }
            currentTime = 0;
        }
        if (isUp == true)
        {
            // 나는 타겟을 조금씩 위로 올릴거다.
            currentPosition += Vector3.up * speed * Time.deltaTime;
        }
        else
        {
            // 나는 타겟을 조금씩 아래로 내릴거다.
            currentPosition += Vector3.down * speed * Time.deltaTime;
        }
        transform.position = currentPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        MeshRenderer mr = this.GetComponent<MeshRenderer>();
        mr.enabled = false;
        BoxCollider bx = this.GetComponent<BoxCollider>();
        bx.enabled = false;
    }
}
