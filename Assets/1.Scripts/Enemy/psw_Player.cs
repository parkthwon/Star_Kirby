using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������� �Է¿� ���� �յ��¿�� �̵��ϰ�ʹ�.
// ����ڰ� ������ư�� ������ ������ �ٰ�ʹ�.
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
        // ��ü���� CharacterController�� ������ �ʹ�.
        cc = gameObject.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // �߷��� ���� y�ӵ��� �ۿ��ؾ��Ѵ�.
        // 9.81 m/s
        yVelocity += gravity * Time.deltaTime;



        // ���� ���� ���ִ� �׸��� ����ڰ� ������ư�� ������
        if (cc.isGrounded && Input.GetButtonDown("Jump"))
        {
            // JumpPower�� y�ӵ��� �ۿ��ؾ��Ѵ�.
            yVelocity = jumpPower;
        }

        // ������ y�ӵ��� dir�� y�׸� �ݿ��Ǿ���Ѵ�.


        currentTime += 10 * Time.deltaTime;

        //������� �Է¿� ���� �����¿�� �̵��ϰ�ʹ�.

        // 1. ������� �Է¿� ����
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        print(h + "," + v);
        // 2. ��ȭ�¿�� ������ �����
        Vector3 dir = h * Vector3.right + v * Vector3.forward;
      
        // dir�� ũ�⸦ 1�� ����� �ʹ�.
        dir.Normalize();
        // 3. �� �������� �̵��ϰ�ʹ�. P = P + vt
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
