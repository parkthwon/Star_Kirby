using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class psw_DeBox : MonoBehaviour
{
    public GameObject particle;
    public GameObject item;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Destroy(this.gameObject);
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Suction")) return;
        GameObject pa = Instantiate(particle);
        pa.transform.position = this.transform.position;

        if (item != null) item.SetActive(true);
        Destroy(pa, 1);
        Destroy(this.gameObject);
        SoundManager.Instance.PlaySFX("BoxBreak");
    }
}
