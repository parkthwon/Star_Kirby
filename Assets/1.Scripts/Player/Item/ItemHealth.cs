using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHealth : Item
{
    public int health = -1;

    public override void GetItem()
    {
        base.GetItem();
        PlayerManager.Instance.PHealth.Heal(health);
        Destroy(gameObject);

        SoundManager.Instance.PlaySFX("Tomato");
    }
}
