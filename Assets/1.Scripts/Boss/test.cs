using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    bool moveRight = false;
    float moveDist = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    if(Input.GetKeyDown(KeyCode.Alpha1)) //5까지만 오른쪽으로 이동하게 한다
        {
            moveRight = true;
        }
            if(moveRight)
        {
            transform.Translate(transform.right * 2 * Time.deltaTime);
            moveDist += 2 * Time.deltaTime;
            
            if(moveDist > 5)
            {
                moveRight = false;
                transform.Translate(-transform.right * (moveDist - 5));
            }
        }

    }
}
