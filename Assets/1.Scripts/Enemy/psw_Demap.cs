using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class psw_Demap : MonoBehaviour
{
    public GameObject objectPrefab;
    private GameObject Player;
    BoxCollider boxCollider;
    float currentTime;
    //사라질때 시간 체크 시작
    bool startDisappear = false;
    bool startBox = true;
    public ParticleSystem particle;
    void Start()
    {
        Player = PlayerManager.Instance.gameObject;
        boxCollider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        //2번키를 누르면
        //사라질때 시간체크 해제
        //나타날때 시간체크 설정


        
        if(startDisappear == true)
        {
            //시간을 흐르게 한다.
            currentTime += Time.deltaTime;
            //1초가 지나면
            if (currentTime > 3f)
            {
                //사라지게 하자.
                MeshRenderer mr = this.GetComponent<MeshRenderer>();
                mr.enabled = false;
                if (mr.enabled == false)
                {
                    particle.Play();
                    print("야 되냐?");
                }
                else
                {
                    particle.Stop();
                }

                BoxCollider bx = this.GetComponent<BoxCollider>();
                bx.enabled = false;
               // animator.Play("Spin");
                currentTime = 0;
                startDisappear = false;
                startBox = false;
            }
        }

        
        if (startBox == false)
        {
            //시간을 흐르게 한다.
            currentTime += Time.deltaTime;
            //1초가 지나면
            if (currentTime > 1)
            {
                //사라지게 하자.
                MeshRenderer mr = this.GetComponent<MeshRenderer>();
                mr.enabled = true;

                BoxCollider bx = this.GetComponent<BoxCollider>();
                bx.enabled = true;
                currentTime = 0;
                startBox = true;
            }
        }

    }
    public void BreakGround()
    {
        //MeshRenderer mr = this.GetComponent<MeshRenderer>();
        //mr.enabled = false;

        //BoxCollider bx = this.GetComponent<BoxCollider>();
        //bx.enabled = false;

        //StartCoroutine(Delay());
        //StartCoroutine(HideRendererAfterDelay(4.5f));
        //StartCoroutine(SpawnObject(4.5f));
        startDisappear = true;
    }

    //IEnumerator SpawnObject(float delay)
    //{
    //    yield return new WaitForSeconds(delay + 3f);
    //    this.GetComponent<MeshRenderer>().enabled = true;
    //    this.GetComponent<BoxCollider>().enabled = true;
    //}

    //IEnumerator Delay()
    //{
    //    yield return new WaitForSeconds(2.5f);
    //}


    //IEnumerator HideRendererAfterDelay(float delay)
    //{
    //    yield return new WaitForSeconds(2f);
    //    this.GetComponent<MeshRenderer>().enabled = false;
    //    boxCollider.enabled = false;
    //}
}

