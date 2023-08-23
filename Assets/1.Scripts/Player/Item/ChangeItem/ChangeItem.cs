using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeItem : Item
{
    [SerializeField] PlayerManager.CHANGETYPE changeType;
    [SerializeField] GameObject itemContents;
    [SerializeField] Renderer itemBorderRenderer;

    public override void GetItem()
    {
        if (changeType != PlayerManager.Instance.ChangeType)
        {
            base.GetItem();
            itemContents.SetActive(false);
            StartCoroutine(GetItemCoroutine());

            //아이템을 먹으면 플레이어 변신
            PlayerManager.Instance.ChangeStart();
            PlayerManager.Instance.Change(changeType);
        }
    }

    IEnumerator GetItemCoroutine()
    {
        Color color = itemBorderRenderer.material.color;
        while (color.a > 0)
        {
            color.a -= Time.deltaTime * 0.5f;
            itemBorderRenderer.material.color = color;
            yield return null;
        }

        Destroy(gameObject);
    }
}
