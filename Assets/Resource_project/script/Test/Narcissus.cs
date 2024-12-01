using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Flower;
using Unity.VisualScripting;
using System.Linq;
using static EncyclopediaUI;

public class Narcissus : MonoBehaviour
{
    [Header("Item Sections")]
    public List<ItemSection> itemSections = new List<ItemSection>();

    FlowerSystem fs;
    InventorySystem inventorySystem;
    List<ItemData.Item> itemList;
    ItemData.Item item;
    int sceneIndex;
    [Header("Water Flower Usage")]
    public static int NarcissusUseCount = 0; // 計數器

    public void Start()
    {
        fs = FlowerManager.Instance.GetFlowerSystem("default");
        inventorySystem = InventorySystem.Instance;
        itemList = inventorySystem.items;
        while (itemSections.Count < 4)
        {
            itemSections.Add(new ItemSection());
        }
        sceneIndex = SceneManager.GetActiveScene().buildIndex;
    }
    public void ClickNarissus()
    {
        Debug.Log($"{fs.isCompleted}");
        Debug.Log("Click!");
        fs.SetupDialog("EnviromentDialogPrefab");
        fs.SetTextList(new List<string> { "你感受到口袋裡的水仙花傳來了異樣的感覺。[w][remove_dialog]"});
        StartCoroutine(WaitForDialogCompletion());
    }

    private IEnumerator WaitForDialogCompletion()
    {
        // 等待對話完成
        yield return new WaitUntil(() => fs.isCompleted);
        Debug.Log($"{fs.isCompleted}");
        // 對話完成後設置按鈕
        fs.SetupButtonGroup();

        fs.SetupButton("使用", () =>
        {
            fs.Resume();
            fs.RemoveButtonGroup();
            Debug.Log("使用水仙花");
            Use();
        });

        fs.SetupButton("不使用", () =>
        {
            fs.Resume();
            fs.RemoveButtonGroup();
            Debug.Log("取消使用水仙花");
        });
    }

    void Use()
    {
        Debug.Log("跳過");
        ItemData.Item newItem;
        // 增加使用水仙花的計數
        NarcissusUseCount++;
        Debug.Log($"用了{NarcissusUseCount}次");

        if (sceneIndex == 4)
        {
            if (!itemList.Any(item => item.index == 22))
            {
                LockPlotButtonForChapter("chapter 4");
                newItem = ItemData.Instance.GetItemByIndex(22);
                inventorySystem.PickUp(newItem);
                int selectedSection = 0;
                SetItemsToNone(selectedSection);
                return;
            }
            else if (!itemList.Any(item => item.index == 23))
            {
                LockPlotButtonForChapter("chapter 5");
                newItem = ItemData.Instance.GetItemByIndex(23);
                inventorySystem.PickUp(newItem);
                int selectedSection = 1;
                SetItemsToNone(selectedSection);
                return;
            }
        }
        else if (sceneIndex == 7)
        {
            if (!itemList.Any(item => item.index == 21))
            {
                LockPlotButtonForChapter("chapter 7");
                newItem = ItemData.Instance.GetItemByIndex(21);
                inventorySystem.PickUp(newItem);
                int selectedSection = 0;
                SetItemsToNone(selectedSection);
                return;
            }
            else if (!itemList.Any(item => item.index == 24) || !itemList.Any(item => item.index == 25))
            {
                LockPlotButtonForChapter("chapter 8");
                newItem = ItemData.Instance.GetItemByIndex(24);
                inventorySystem.PickUp(newItem);
                newItem = ItemData.Instance.GetItemByIndex(25);
                inventorySystem.PickUp(newItem);
                int selectedSection = 1;
                SetItemsToNone(selectedSection);
                return;
            }
        }
    }

    // 鎖定章節劇情按鈕
    void LockPlotButtonForChapter(string chapterName)
    {
        var chapter = EncyclopediaUI.Instance.chapters.Find(c => c.chapterName == chapterName);

        if (chapter != null)
        {
            if (chapter.unlockButton != null)
                chapter.unlockButton.gameObject.SetActive(false);
            if (chapter.plotButton != null)
                chapter.plotButton.gameObject.SetActive(false);
            // 記錄鎖定狀態
            EncyclopediaProgress.lockedChapters[chapterName] = true;
        }
        else
            Debug.LogError($"找不到章節：{chapterName}");
    }

    // 設置選擇區段內的所有Item屬性為 NONE
    void SetItemsToNone(int sectionIndex)
    {
        if (sectionIndex >= 0 && sectionIndex < itemSections.Count)
        {
            foreach (var item in itemSections[sectionIndex].items)
            {
                item.interactionType = Item.InteractionType.NONE;
                item.itemType = Item.ItemType.NONE;
                item.dialogueType = Item.DialogueType.NONE;
                item.GetComponent<Collider2D>().enabled = false;

                Debug.Log($"Item {item.name} in section {sectionIndex} set to NONE.");
            }
        }
        else
        {
            Debug.LogError($"Section {sectionIndex} is out of range.");
        }
    }
}

[System.Serializable]
public class ItemSection
{
    public List<Item> items = new List<Item>();
}
