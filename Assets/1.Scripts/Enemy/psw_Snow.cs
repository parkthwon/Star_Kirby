using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class psw_Snow : MonoBehaviour
{

    Rigidbody rb;

   
   // public string Player;
    public float speed = 5;
    public float sizespeed = 3;
    float size = 0;
    public float maxsize = 3; 
    public float slopeForce = 5; // 경사로 올라갈 때 가해지는 힘의 크기
    public GameObject particle;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.right * speed;
        //GameObject pa = Instantiate(particle);
        //pa.transform.position = this.transform.position;
        //Destroy(pa, 1);
    }

    // Update is called once per frame
    void Update()
    {
        size += sizespeed * Time.deltaTime;
        if (size > maxsize)
        {
            size = maxsize;
        }
        else
        {
            transform.localScale = Vector3.one * size;
            Vector3 dir = Vector3.right;
            // dir의 크기를 1로 만들고싶다.
            dir.Normalize();
            // 3. 그 방향으로 이동하고싶다. P = P + vt
            Vector3 velocity = dir * speed;
            rb.velocity = velocity;
        }
        if (Mathf.Abs(rb.velocity.x) > 3) // 가속도가 3 이상이라면
        rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * 3, rb.velocity.y); //가속도 제한
        if (rb.velocity.magnitude < 0.1f)
        {
            Destroy(gameObject);
            GameObject pa = Instantiate(particle);
            pa.transform.position = this.transform.position;
            Destroy(pa, 1);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        //Destroy(gameObject, delayTime);
        if (other.gameObject.CompareTag("Player"))
        {
            //적의 반대를 향하는 벡터
            Vector3 dir = other.transform.position - transform.position;
            dir.y = 0;
            dir.Normalize();
            PlayerManager.Instance.PHealth.Hit(dir, 1, true);
        }

        var rb = other.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Destroy(gameObject);
            GameObject pa = Instantiate(particle);
            pa.transform.position = this.transform.position;
            Destroy(pa, 1);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //DestroySelf(other.gameObject);

        Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Destroy(gameObject);
            GameObject pa = Instantiate(particle);
            pa.transform.position = this.transform.position;
            Destroy(pa, 1);
        }
    }

}
       
