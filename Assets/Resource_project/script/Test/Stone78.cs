using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Flower;
using System;

public class Stone78 : MonoBehaviour
{
    [Header("檢查道具")]
    public ItemData itemData;
    public int[] itemIndices;
    public int correctItemIndex;
    public bool isItemCorrectlyPlaced = false;
    public Item items;

    private Image image;
    private Item tableType;
    FlowerSystem fs;

    private void Start()
    {
        image = this.GetComponent<Image>();
        items = this.GetComponent<Item>();
        GameObject table = GameObject.Find("LaboratoryTable");
        tableType = table.GetComponent<Item>();
        fs = FlowerManager.Instance.GetFlowerSystem("default");
    }

    public void Put()
    {
        if (image.sprite != null && !(FindObjectOfType<DragAndDrop>().item.index == 15 || FindObjectOfType<DragAndDrop>().item.index == 16))
        {
            PickChestItem();
            return;
        }
        else
        {
            PutItem(FindObjectOfType<DragAndDrop>().item);
        }
    }

    private void PutItem(ItemData.Item item)
    {
        
        if (image.sprite == null)
        {
            if (item.index == 15 || item.index == 16)
            {
                Debug.LogWarning($"put item {item.itemName} {item.index}");
                itemIndices[0] = item.index;
                image.sprite = item.itemIcon;
                Color color = image.color;
                color.a = 255f;
                image.color = color;
                fs.SetupDialog("PlayerDialogPrefab");
                fs.SetTextList(new List<string>{"第八顆...應該也要進得去才對...[w][remove_dialog]"});
                Debug.Log("觸發對話");
                FindObjectOfType<DragAndDrop>().StopDragItem();
                int quantityToRemove = 1;
                FindObjectOfType<InventorySystem>().RemoveItem(item, quantityToRemove);
            }
            else
            {
                PickChestItem();
            }
        }
        else if (image.sprite != null && (item.index == 15 || item.index == 16) && FindObjectOfType<InventorySystem>().isDragging)
        {
            Debug.Log("Trigger Animate");
            fs.SetupDialog("enviromentDialogPrefab");
            fs.SetTextList(new List<string> { "實驗桌的抽屜傳來異樣的聲響。[w][remove_dialog]" });
            StartCoroutine(PickUpStick());
            FindObjectOfType<DragAndDrop>().StopDragItem();
            int quantityToRemove = 1;
            FindObjectOfType<InventorySystem>().RemoveItem(item, quantityToRemove);
        }
    }

    public void PickChestItem()
    {
        if (image.sprite != null)
        {
            items.PickUpItem();
            image.sprite = null;
            Color color = image.color;
            color.a = 0f;
            image.color = color;
            isItemCorrectlyPlaced = false;
        }
        else if (FindObjectOfType<InventorySystem>().isDragging && image.sprite == null)
        {
            PutItemOnChest(FindObjectOfType<DragAndDrop>().item);
            return;
        }
        else
        {
            return;
        }

        FindObjectOfType<InventorySystem>().UpdateUI();
    }

    private void PutItemOnChest(ItemData.Item item)
    {
        Debug.LogWarning($"put item on chest {item.itemName} {item.index}");
        for (int i = 0; i < items.itemIndices.Length; i++)
        {
            items.itemIndices[i] = item.index;
            if (image.sprite == null)
            {
                image.sprite = item.itemIcon;
                Color color = image.color;
                color.a = 255f;
                image.color = color;
                if (items.itemIndices[i] == correctItemIndex)
                {
                    isItemCorrectlyPlaced = true;
                }
            }
            Debug.LogWarning(items.itemIndices[i]);
        }
        FindObjectOfType<DragAndDrop>().StopDragItem();
        int quantityToRemove = 1;
        FindObjectOfType<InventorySystem>().RemoveItem(item, quantityToRemove);
    }
    private IEnumerator PickUpStick()
    {
        yield return new WaitUntil(() => fs.isCompleted);
        tableType.PickUpItem();
        tableType.interactionType = Item.InteractionType.NONE;
        tableType.dialogueType = Item.DialogueType.Normal;
        tableType.dialogueIndex = 0;

    }
}
