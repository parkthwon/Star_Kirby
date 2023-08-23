using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCoin : Item
{
    enum CoinType
    {
        Coin1,
        Coin5,
        Coin10,
    }
    [SerializeField] CoinType coinType;

    public override void GetItem()
    {
        base.GetItem();
        StartCoroutine(PlayAnimationDelayed(0f));
        SoundManager.Instance.PlaySFX("Coin");
        Destroy(gameObject, 0.6f);
        switch (coinType)
        {
            case CoinType.Coin1:
                PlayerManager.Instance.PCoin.GetCoin(1);
                break;
            case CoinType.Coin5:
                PlayerManager.Instance.PCoin.GetCoin(5);
                break;
            case CoinType.Coin10:
                PlayerManager.Instance.PCoin.GetCoin(10);
                break;
        }
    }

    private IEnumerator PlayAnimationDelayed(float delay)
    {
        Animation animation = this.gameObject.GetComponent<Animation>();
        animation.Play();
       
        //yield return new WaitForSeconds(delay);
        yield return new WaitForSeconds(delay);
    }

}
