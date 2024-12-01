using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Flower;

public class Counter : MonoBehaviour
{
    public Item boxItem;
    public Item braceletItem;

    FlowerSystem fs;

    InventorySystem inventorySystem;
    List<ItemData.Item> itemList;

    private void Start()
    {
        inventorySystem= InventorySystem.Instance;
        itemList = inventorySystem.items;
        fs = FlowerManager.Instance.GetFlowerSystem("default");    
    }

    public void BuyBox()
    {
        Image boxImage = boxItem.GetComponent<Image>();
        foreach (var item in itemList)
        {
            if (item.index == 18)
            {
                Debug.Log("Found");
                if (item.stackSize >= 6)
                {
                    Debug.Log("buy");
                    boxItem.Interact();
                    DisableImage(boxImage);
                    inventorySystem.RemoveItem(item, 6);
                    StartCoroutine(UnlockGAseat());
                    break;
                }
            }
        }
    }

    public void BuyBracelet()
    {
        bool hasItemA = false;
        bool hasItemB = false;
        Image braceletImage = braceletItem.GetComponent<Image>();
        foreach (var item in itemList)
        {
            if (item.index == 18 && item.stackSize >= 2)
            {
                Debug.Log("Found item A");
                hasItemA = true;
            }

            if (item.index == 19)
            {
                Debug.Log("Found item B");
                hasItemB = true;
            }

            if (hasItemA && hasItemB)
            {
                Debug.Log("Buy Bracelet");
                braceletItem.Interact();
                DisableImage(braceletImage);
                inventorySystem.RemoveItem(itemList.First(i => i.index == 18), 2);

                break;
            }
        }
    }

    private void DisableImage(Image image)
    {
        image.sprite = null;
        Color color = image.color;
        color.a = 0f;
        image.color = color;
    }

    private IEnumerator UnlockGAseat()
    {
        fs.SetupDialog("EnviromentDialogPrefab");
        fs.SetTextList(new List<string> { "走廊外頭傳來了移動桌子的聲音。[w][remove_dialog]" });
        yield return new WaitUntil(() => fs.isCompleted);
        fs.SetupDialog("PlayerDialogPrefab");
        fs.SetTextList(new List<string> { "這又是從哪裡傳來的?教室嗎?[w][remove_dialog]" });
    }
}
