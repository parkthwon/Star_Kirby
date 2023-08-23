using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Psw_SpinCoin : MonoBehaviour
{
    //public Animation animation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    private void OnTriggerEnter(Collider other)
    {
        print("1111111111111111111");
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(PlayAnimationDelayed(0f));
            Destroy(gameObject, 1.5f);
        }
    }
    private IEnumerator PlayAnimationDelayed(float delay)
    {
        print("22222222222");
        Animation animation = this.gameObject.GetComponent<Animation>();
        animation.Play();
        //yield return new WaitForSeconds(delay);
        yield return new WaitForSeconds(delay);
    }
}
