using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class psw_Flower : MonoBehaviour
{
    public GameObject ItemFactory;
    bool isOpen=false;
   
    private void OnTriggerEnter(Collider other)
    {
        if (isOpen) return;

        if (other.gameObject.CompareTag("Player"))
        {
            isOpen = true;
            Item item = Instantiate(ItemFactory, transform.position, Quaternion.identity).GetComponent<Item>();
            item.GetItem();
            GetComponentInChildren<Renderer>().materials[2].color = new Color32(231, 0, 0, 255);
        }
    }
}
