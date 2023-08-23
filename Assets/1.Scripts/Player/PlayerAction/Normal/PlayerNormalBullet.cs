using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerNormalBullet : MonoBehaviour
{
    //발사 방향
    Vector3 direction;
    Vector3 bulletRotate;

    SphereCollider col;
    Rigidbody rigid;

    public void Set(Vector3 direction)
    {
        this.direction = direction;
        ////기존 콜라이더 비활성화
        //GetComponent<Collider>().enabled = false;
        //새로운 원콜라이더 추가
        col = this.AddComponent<SphereCollider>();
        col.radius = 0.75f;
        col.isTrigger = true;

        rigid = this.AddComponent<Rigidbody>();
        rigid.useGravity = false;
        rigid.constraints = RigidbodyConstraints.FreezeAll;

        //회전 방향 설정
        bulletRotate = new Vector3(1, 1, 1);

        //레이어 변경
        gameObject.layer = LayerMask.NameToLayer("PlayerBullet");
    }

    // Update is called once per frame
    void Update()
    {
        //날아가기
        transform.position += direction * Time.deltaTime * PlayerManager.Instance.Data.spitItemSpeed;
        //랜덤하게 회전하기
        transform.Rotate(bulletRotate * Time.deltaTime * 180f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Boss") || other.gameObject.layer == LayerMask.NameToLayer("Hammer"))
        {
            other.transform.root.GetComponent<Boss_Ex>().DamageProcess();
            Destroy(gameObject);
            //Destroy(Instantiate(hitEffect, transform.position, Quaternion.identity), 3);
        }
        //레이어가 스테이지 인경우
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            //닿은 지점과 중심간의 벡터를 구한다.
            Vector3 vec = other.ClosestPoint(transform.position) - transform.position;
            if (vec.magnitude < 0.5)
                Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        //충돌한 대상이 적인 경우
        //적에게 데미지를 준다.

        //충돌한 대상이 물건인 경우
        //물건 파괴
    }
}
