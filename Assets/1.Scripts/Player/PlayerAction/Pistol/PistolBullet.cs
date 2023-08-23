using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolBullet : MonoBehaviour
{
    [SerializeField] float bulletSpeed = 10f;
    [SerializeField] float lifeTime = 3f;
    [SerializeField] Outline outline;
    [SerializeField] Color[] outlineColors;
    int curColor = 0;
    float curColorChangeTime = 0f;
    float colorChangeDelay = 0.5f;
    bool isGuide = false;   //유도
    Vector3 moveDir;
    Collider targetCollider;

    //이펙트
    [SerializeField] GameObject hitEffect;
    [SerializeField] GameObject nonHitEffect;

    public void Set(Vector3 dir)
    {
        isGuide = false;
        moveDir = dir;
    }

    public void Set(Collider target)
    {
        isGuide = true;
        targetCollider = target;
    }

    private void Start()
    {
        curColor = Random.Range(0, outlineColors.Length);
        outline.OutlineColor = outlineColors[curColor];

        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (isGuide && targetCollider != null)
        {
            moveDir = targetCollider.bounds.center - transform.position;
            transform.position += moveDir.normalized * Time.deltaTime * bulletSpeed;
        }
        else
        {
            //정해진 방향으로 이동
            transform.position += moveDir * Time.deltaTime * bulletSpeed;
        }

        //Color
        curColorChangeTime += Time.deltaTime;
        if (curColorChangeTime > colorChangeDelay)
        {
            curColorChangeTime = 0;
            int color = Random.Range(0, outlineColors.Length);
            if (color == curColor) color++;
            if (color >= outlineColors.Length) color = 0;
            curColor = color;
            outline.OutlineColor = outlineColors[curColor];
        }

        transform.LookAt(Camera.main.transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Boss") || other.gameObject.layer == LayerMask.NameToLayer("Hammer"))
        {
            other.transform.root.GetComponent<Boss_Ex>().DamageProcess();
            Destroy(Instantiate(hitEffect, transform.position, Quaternion.identity), 3);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Destroy(Instantiate(hitEffect, transform.position, Quaternion.identity), 3);
        }

        if (!other.CompareTag("CameraBasic"))
        {
            Destroy(Instantiate(nonHitEffect, transform.position, Quaternion.identity), 3);
            Destroy(gameObject);
        }
    }
}
