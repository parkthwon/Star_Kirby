using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 사용자의 입력에 따라서 앞뒤좌우로 이동하고싶다.
// 사용자가 점프버튼을 누르면 점프를 뛰고싶다.
public class psw_Player : MonoBehaviour
{
    public float jumpPower = 10;
    public float gravity = -9.81f;
    float yVelocity;

    CharacterController cc;

    public float speed = 5;

    float currentTime;
    // Start is called before the first frame update
    void Start()
    {
        // 본체에게 CharacterController를 얻어오고 싶다.
        cc = gameObject.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // 중력의 힘이 y속도에 작용해야한다.
        // 9.81 m/s
        yVelocity += gravity * Time.deltaTime;



        // 만약 땅에 서있다 그리고 사용자가 점프버튼을 누르면
        if (cc.isGrounded && Input.GetButtonDown("Jump"))
        {
            // JumpPower가 y속도에 작용해야한다.
            yVelocity = jumpPower;
        }

        // 결정된 y속도를 dir의 y항목에 반영되어야한다.


        currentTime += 10 * Time.deltaTime;

        //사용자의 입력에 따라 상하좌우로 이동하고싶다.

        // 1. 사용자의 입력에 따라
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        print(h + "," + v);
        // 2. 상화좌우로 방향을 만들고
        Vector3 dir = h * Vector3.right + v * Vector3.forward;
      
        // dir의 크기를 1로 만들고 싶다.
        dir.Normalize();
        // 3. 그 방향으로 이동하고싶다. P = P + vt
        Vector3 velocity = dir * speed;
        transform.position += velocity * Time.deltaTime;
       
        velocity.y = yVelocity;



        cc.Move(velocity * Time.deltaTime);
       
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 from = transform.position;
        Vector3 to = from + Vector3.up * yVelocity;
        Gizmos.DrawLine(from, to);
    }
}
