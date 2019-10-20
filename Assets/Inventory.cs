using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject bucketSlot;
    private int slotNumber;
    private GameObject[] slot;

    void Start()
    {
        slotNumber = 1;
        slot = new GameObject[slotNumber];

        for (int i = 0; i < slotNumber; i++)
        {
            slot[i] = bucketSlot.transform.GetChild(i).gameObject;
            if(slot[i].GetComponent<ItemSlot>().item == null)
            {
                slot[i].GetComponent<ItemSlot>().empty = true;
            }
        }
    }
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "InventoryItems")
        {
            GameObject itemPickedUp = other.gameObject;
            InventoryItems item = itemPickedUp.GetComponent<InventoryItems>();

            AddItem(itemPickedUp, item.ID, item.type, item.description, item.icon);
        }
    }

    void AddItem(GameObject itemObject, int itemID, string itemType, string itemDescription, Sprite itemIcon)
    {
        for (int i = 0; i < slotNumber; i++)
        {
            if (slot[i].GetComponent<ItemSlot>().empty)
            {
                //add item to slot
                itemObject.GetComponent<InventoryItems>().pickedUp = true;

                slot[i].GetComponent<ItemSlot>().item = itemObject;
                slot[i].GetComponent<ItemSlot>().icon = itemIcon;
                slot[i].GetComponent<ItemSlot>().type = itemType;
                slot[i].GetComponent<ItemSlot>().ID = itemID;
                slot[i].GetComponent<ItemSlot>().description = itemDescription;

                itemObject.transform.parent = slot[i].transform;

                slot[i].GetComponent<ItemSlot>().UpdateSlot();
                slot[i].GetComponent<ItemSlot>().empty = false;

                return;
            }
        }
    }
}
