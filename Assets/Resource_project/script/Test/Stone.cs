using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Stone : MonoBehaviour
{
    [Header("獲得道具列表")]
    public int correctItemIndex;
    public bool isItemCorrectlyPlaced = false;
    public Item items;
    public SpriteRenderer[] stoneSprite;
    
    private Image image;
    private Chest chestManager;

    private void Start()
    {
        image = this.GetComponent<Image>();
        items = this.GetComponent<Item>();
    }

    public void SetChestManager(Chest manager)
    {
        chestManager = manager;
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
            PutItem(FindObjectOfType<DragAndDrop>().item);
            return;
        }
        else
        {
            return;
        }
        
        FindObjectOfType<InventorySystem>().UpdateUI();
    }

    public void PickLibraItem()
    {
        /*foreach (int index in items.itemIndices)
        {
            if (index >= 0 && index < items.itemData.items.Count)
            {
                items.PickUpItem();
            }
            else
            {
                return;
            }
        }*/
        items.PickUpItem();

        Transform[] children = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child != transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        FindObjectOfType<InventorySystem>().UpdateUI();
        gameObject.SetActive(false);
        FindObjectOfType<LibraConrtoller>().stoneGram = 0;
        FindObjectOfType<LibraConrtoller>().LibraAnimate();
    }

    public void LibraStone()
    {
        if (image.sprite != null)
        {
            items.PickUpItem();
            image.sprite = null;
            for (int i = 0; i < stoneSprite.Length; i++)
                stoneSprite[i].gameObject.SetActive(false);
            isItemCorrectlyPlaced = false;
            FindObjectOfType<LibraConrtoller>().stoneGram = 0;
            FindObjectOfType<LibraConrtoller>().LibraAnimate();
        }
        else if (FindObjectOfType<InventorySystem>().isDragging)
        {
            PutItemOnLibra(FindObjectOfType<DragAndDrop>().item);
            FindObjectOfType<LibraConrtoller>().SetStoneGram();
            return;
        }
        else
        {
            return;
        }

        FindObjectOfType<InventorySystem>().UpdateUI();
    }

    private void PutItem(ItemData.Item item)
    {
        Debug.LogWarning($"put item {item.itemName} {item.index}");
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
                    chestManager.CheckAllSlots();
                }
            }
            Debug.LogWarning(items.itemIndices[i]);
        }
        FindObjectOfType<DragAndDrop>().StopDragItem();
        int quantityToRemove = 1;
        FindObjectOfType<InventorySystem>().RemoveItem(item, quantityToRemove);
    }

    private void PutItemOnLibra(ItemData.Item item)
    {
        Debug.LogWarning($"put item {item.itemName} {item.index}");
        for (int i = 0; i < items.itemIndices.Length; i++)
        {
            items.itemIndices[i] = item.index;
            for (int j = 0; j < stoneSprite.Length; j++)
            {
                if (image.sprite == null && item.itemName == stoneSprite[j].name)
                {
                    stoneSprite[j].gameObject.SetActive(true);
                    image.sprite = item.itemIcon;
                }
            }
            Debug.LogWarning(items.itemIndices[i]);
        }
        FindObjectOfType<DragAndDrop>().StopDragItem();
        int quantityToRemove = 1;
        FindObjectOfType<InventorySystem>().RemoveItem(item, quantityToRemove);
    }
}
