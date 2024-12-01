using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flower;
using System.Linq;
using Unity.VisualScripting;

public class Stage3Counter : MonoBehaviour
{

    FlowerSystem fs;
    InventorySystem playerInventorySystem; //玩家的物品欄
    List<ItemData.Item> itemList; //玩家的物品欄
    public Item counterItem;

    public static bool isCheck = false;
    public static bool isBoxBuy = false;
    public static bool isBraceltBuy = false;

    // Start is called before the first frame update
    void Start()
    {
        fs = FlowerManager.Instance.GetFlowerSystem("default");
        playerInventorySystem = InventorySystem.Instance;
        itemList = playerInventorySystem.items;
    }

    public void Counter()
    {
        if(!isCheck) 
        {
            isCheck = true;
            fs.SetupDialog("EnviromentDialogPrefab");
            fs.SetTextList(new List<string> { "走廊外頭傳來了錢掉落的聲音。[w]櫃台好像有些甚麼。[w][remove_dialog]" });
        }
        else if( !isBoxBuy || !isBraceltBuy) 
        {
            fs.SetupDialog("EnviromentDialogPrefab");
            fs.SetTextList(new List<string> { "櫃台好像有些甚麼。[w][remove_dialog]" });
        }
        else
        {
            fs.SetupDialog("EnviromentDialogPrefab");
            fs.SetTextList(new List<string> { "櫃台空無一物。[w][remove_dialog]" });
            return;
        }
  
        StartCoroutine(WaitForDialogCompletion(() =>
        {
            fs.SetupButtonGroup();
            fs.SetupButton("查看", () => {
                fs.Resume();
                fs.SetupDialog("EnviromentDialogPrefab");
                fs.SetTextList(new List<string> { "映入眼簾的是玻璃櫥窗，裡面還有兩個東西。[w][remove_dialog]" });
                fs.RemoveButtonGroup();
                OpenCounter();
            });
            fs.SetupButton("離開", () => {
                fs.Resume();
                fs.RemoveButtonGroup();
            });
        }));
    }


    public void OpenCounter()
    {
        StartCoroutine(WaitForDialogCompletion(() =>
        {
            fs.SetupDialog("EnviromentDialogPrefab");
            fs.SetTextList(new List<string> { "標籤上寫著：精美的盒子【六塊】、一條手鍊【兩塊】，請付款後再拿取。[w][remove_dialog]" });
            StartCoroutine(WaitForDialogCompletion (() =>
            {
                fs.SetupButtonGroup();
                if (!isBoxBuy)
                {
                    fs.SetupButton("拿走盒子", () =>
                    {
                        fs.Resume();
                        //買盒子
                        BuyBox();
                        fs.RemoveButtonGroup();
                    });
                }
                if (!isBraceltBuy)
                {
                    fs.SetupButton("拿走手鍊", () => {
                        fs.Resume();
                        //買手鍊
                        BuyBracelet();
                        fs.RemoveButtonGroup();
                    });
                }
                fs.SetupButton("離開", () => {
                    fs.Resume();
                    fs.RemoveButtonGroup();
                });
            }));
        }));


    }

    public void BuyBox()
    {
        foreach (var item in itemList)
        {
            if (item.index == 18)
            {
                Debug.Log("Found");
                if (item.stackSize >= 6)
                {
                    Debug.Log("buy");
                    playerInventorySystem.RemoveItem(item, 6);
                    counterItem.PickUpByIndex(19);
                    isBoxBuy = true;
                    StartCoroutine(UnlockGAseat());
                    break;
                }
            }
            else
            {
                fs.SetupDialog("EnviromentDialogPrefab");
                fs.SetTextList(new List<string> { "妳用力打開櫥窗，卻只見紋絲不動。[w][remove_dialog]" });
                StartCoroutine(WaitForDialogCompletion(() =>
                {
                    fs.SetupDialog("PlayerDialogPrefab");
                    fs.SetTextList(new List<string> { "或許我...應該要付錢?[w][remove_dialog]" });
                }));
                break;

            }

        }
    }

    public void BuyBracelet()
    {
        if (isBoxBuy)
        {
            foreach (var item in itemList)
            {
                if (item.index == 18)
                {
                    Debug.Log("Found");
                    if (item.stackSize >= 2)
                    {
                        Debug.Log("buy");
                        playerInventorySystem.RemoveItem(item, 2);
                        counterItem.PickUpByIndex(20);
                        isBraceltBuy = true;
                        break;
                    } 
                }
                else
                {
                    fs.SetupDialog("EnviromentDialogPrefab");
                    fs.SetTextList(new List<string> { "妳用力打開櫥窗，卻只見紋絲不動。[w][remove_dialog]" });
                    StartCoroutine(WaitForDialogCompletion(() =>
                    {
                        fs.SetupDialog("PlayerDialogPrefab");
                        fs.SetTextList(new List<string> { "或許我...應該要付錢?[w][remove_dialog]" });
                    }));
                    break;

                }

            }
        }
        else
        {
            fs.SetupDialog("PlayerDialogPrefab");
            fs.SetTextList(new List<string> { "感覺沒甚麼用，先看盒子好了。[w][remove_dialog]" });
        }
        
    }

    private IEnumerator UnlockGAseat()
    {
        fs.SetupDialog("EnviromentDialogPrefab");
        fs.SetTextList(new List<string> { "走廊外頭傳來了移動桌子的聲音。[w][remove_dialog]" });
        yield return new WaitUntil(() => fs.isCompleted);
        fs.SetupDialog("PlayerDialogPrefab");
        fs.SetTextList(new List<string> { "這又是從哪裡傳來的?教室嗎?[w][remove_dialog]" });
    }
    private IEnumerator WaitForDialogCompletion(System.Action onCompleted)
    {
        // 等待對話完成
        while (!fs.isCompleted)
        {
            yield return null; // 每幀檢查一次
        }

        // 對話完成後執行回調
        onCompleted?.Invoke();
    }
}
