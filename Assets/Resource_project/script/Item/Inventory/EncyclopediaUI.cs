using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Collections;
using System.Linq;
using UnityEditor;

[System.Serializable]
public class Chapter
{
    public string chapterName;           // 章節名稱
    public List<ItemData.Item> items;    // 章節包含的道具
    public Button unlockButton;          // 章節對應的按鈕
    public Button plotButton;            // 劇情按鈕，需控制其鎖定狀態
    public List<string> requiredUnlockChapters; // 依賴的章節名稱
}

public class EncyclopediaUI : MonoBehaviour
{    
    public static EncyclopediaUI Instance { get; private set; }  //圖鑑的單例實例    
    public bool IsInitialized { get; private set; } = false;     //確保圖鑑只初始化一次的標誌    
    public static int ExtraCounter;        //結局計數器

    private Dictionary<Tuple<string, int>, Image> itemSlots = new Dictionary<Tuple<string, int>, Image>();      //用於保存道具的圖鑑槽

    public Image[] itemImages;      // Normal顯示
    //public Image[] extraItemImages; // Extra顯示

    [SerializeField] private ItemData itemData;             // 引用 ItemData ScriptableObject

    public List<Chapter> chapters; // 所有章節及其對應的道具和按鈕
    
    //初始化圖鑑區
    public void Initialize()
    {
        if (IsInitialized) return;              // 防止重複初始化
        IsInitialized = true;

        InitializeSlots();                      // 初始化圖鑑槽
        // 檢查是否正確初始化所有道具槽位                                        
        if (itemSlots == null || itemSlots.Count == 0)
            Debug.LogError("圖鑑槽初始化失敗。");
    }

    //靜態定義區
    public static class EncyclopediaProgress
    {       
        public static Dictionary<string, bool> unlockedChapters = new Dictionary<string, bool>();   // 靜態變數保存章節解鎖狀態       
        public static Dictionary<string, bool> collectedItems = new Dictionary<string, bool>();     // 靜態變數保存道具的拾取狀態（使用道具名稱作為鍵）
        public static Dictionary<string, bool> lockedChapters = new Dictionary<string, bool>();     // 鎖定的章節
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ClearSlots();
        }
        else
            Destroy(gameObject);            // 防止多個圖鑑實例
    }
    void ClearSlots()
    {
        itemSlots.Clear(); // 清空字典
        foreach (var image in itemImages)
        {
            image.sprite = null;
            image.color = new Color(0.5f, 0.5f, 0.5f, 1f); // 灰色
            image.gameObject.SetActive(false); // 隱藏所有插槽
        }
    }

    void Start()
    {
        if (itemData == null)
        {
            Debug.LogError("ItemData is not assigned in the Inspector");
            return;
        }
        
        InitializeSlots();      // 初始化插槽
        SubscribeToEvents();    // 訂閱事件
        UpdateUI();             // 更新 UI 狀態
        // 初始化時隱藏所有按鈕
        foreach (var chapter in chapters)
            chapter.unlockButton.gameObject.SetActive(false);
        gameObject.SetActive(false);
        // 還原跨場景資料
        RestoreCollectedItems();
        RestoreUnlockedChapters();
        RestoreRequiredUnlockChapters();

        Debug.Log("圖鑑初始化完成");
    }
    // 初始化圖鑑槽位
    void InitializeSlots()
    {
        IsInitialized = true;  // 標記為已初始化
        itemSlots.Clear();
        int currentSlotIndex = 0;
        
        int requiredSlots = chapters.Sum(chapter => chapter.items.Count);
        Debug.Log($"需要的插槽數量: {requiredSlots}, 當前插槽數量: {itemImages.Length}");
        Debug.Log("--------------------開始初始化圖鑑插槽----------------------");
        foreach (var chapter in chapters)
        {
            bool shouldSkipChapter = chapter.chapterName == "chapter 3" || chapter.chapterName == "chapter 6";
            if (shouldSkipChapter)
            {
                Debug.Log($"跳過章節: {chapter.chapterName}");
                continue; // 跳過本章節
            }
            foreach (var item in chapter.items)
            {
                // 忽略與按鈕相關的章節項
                /*if (chapter.chapterName == "chapter 3")
                    break;
                if (chapter.chapterName == "chapter 6")
                    break;*/
                if (currentSlotIndex >= itemImages.Length)
                {
                    Debug.LogWarning($"插槽不足，無法顯示道具: {item.itemName} (index: {item.index})");
                    continue;
                }

                var itemImage = itemImages[currentSlotIndex];
                // 使用 item 名稱與 index 作為鍵
                var key = new Tuple<string, int>(item.itemName, item.index);
                itemSlots[key] = itemImage;

                // 初始化時將所有道具圖標設置為灰色
                itemImage.sprite = item.itemIcon;
                itemImage.color = new Color(0.5f, 0.5f, 0.5f, 1f); // 灰色
                itemImage.gameObject.SetActive(true); // 確保方框是啟用狀態

                Debug.Log($"插槽初始化: Chapter: {chapter.chapterName}, Item: {item.itemName}, Index: {item.index}, 插槽序號: {currentSlotIndex}");
                currentSlotIndex++;
            }
        }
        Debug.Log($"圖鑑插槽初始化完成，已使用插槽數量: {currentSlotIndex}, 總插槽數量: {itemImages.Length}");
    }

    // 訂閱事件
    void SubscribeToEvents()
    {
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.OnInventoryChanged += UpdateUI;
            InventorySystem.Instance.OnItemAdded += OnItemAdded;
        }
        else
        {
            Debug.LogError("InventorySystem.Instance is null when trying to subscribe to events");
            StartCoroutine(RetrySubscribeToEvents());
        }
    }

    // 重複偵測事件
    private IEnumerator RetrySubscribeToEvents()
    {
        while (InventorySystem.Instance == null)
            yield return new WaitForSeconds(1f);            // 每秒重試一次

        InventorySystem.Instance.OnInventoryChanged += UpdateUI;
        InventorySystem.Instance.OnItemAdded += OnItemAdded;
    }

    private void OnEnable()
    {
        SubscribeToEvents();
        RestoreCollectedItems();
        RestoreUnlockedChapters();
    }

    private void OnDisable()
    {
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.OnInventoryChanged -= UpdateUI;
            InventorySystem.Instance.OnItemAdded -= OnItemAdded;
        }
    }

    //拾取道具後圖鑑的變化
    private void OnItemAdded(ItemData.Item item)
    {
        var key = new Tuple<string, int>(item.itemName, item.index);

        // 檢查鍵值是否存在
        if (itemSlots.TryGetValue(key, out Image itemImage))
        {
            itemImage.sprite = item.itemIcon;
            itemImage.color = Color.white;      // 顯示道具圖標
            // 記錄道具已拾取狀態
            EncyclopediaProgress.collectedItems[item.itemName] = true;           
        }
        else
            Debug.LogError($"找不到該道具的插槽: {item.itemName}，index: {item.index}");

        CheckAllItemsCollected();       // 檢查是否收集完所有道具
        CheckPlotButtonDependencies();
        UpdateUI();
    }

    public void UpdateUI()
    {
        foreach (var pair in itemSlots)
        {
            var key = pair.Key;
            Image itemImage = pair.Value;

            if (EncyclopediaProgress.collectedItems.ContainsKey(key.Item1))
            {
                itemImage.color = Color.white;                      // 顯示為正常顏色
            }
            else
                itemImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);  // 保持灰色
        }
        CheckAllItemsCollected();
    }
    public void RestoreItemImages()
    {
        foreach (var pair in itemSlots)
        {
            var key = pair.Key;
            Image itemImage = pair.Value;

            if (EncyclopediaProgress.collectedItems.ContainsKey(key.Item1) && EncyclopediaProgress.collectedItems[key.Item1])
            {
                ItemData.Item item = itemData.GetItemByIndex(key.Item2);
                // 修改这里的 null 检查
                itemImage.sprite = item.itemIcon;
                itemImage.color = Color.white;  // 显示正常颜色
            }
            else
                itemImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);  // 如果道具未被收集，保持灰色
        }
    }

    /*---------------------------------------水仙花跟圖鑑-------------------------------------------------*/
    private bool isNarcissusUsed = false;

    // 針對特定章節進行鎖定
    public void LockPlotButtonsAfterNarcissus(string chapterNameToLock)
    {
        isNarcissusUsed = true;  // 標記水仙花已使用
        var chapterToLock = chapters.Find(chapter => chapter.chapterName == chapterNameToLock);// 查找指定的章節

        if (chapterToLock != null)
        {
            // 鎖定特定章節的劇情按鈕和解鎖按鈕
            chapterToLock.plotButton.interactable = false;
            SetButtonColor(chapterToLock.plotButton, Color.gray);

            chapterToLock.unlockButton.interactable = false;
            SetButtonColor(chapterToLock.unlockButton, Color.gray);

            Debug.Log($"章節 {chapterNameToLock} 的按鈕已鎖定（使用水仙花後）。");
        }
        else
            Debug.LogError($"找不到名為 {chapterNameToLock} 的章節。");
    }

    /*--------------------------------------解鎖按鈕與劇情按鈕控制----------------------------------------*/
    void CheckAllItemsCollected()
    {
        foreach (var chapter in chapters)
        {
            bool allItemsCollected = true;

            foreach (var item in chapter.items)
            {
                var key = new Tuple<string, int>(item.itemName, item.index);
                bool hasItem = InventorySystem.Instance.HasItem(item.itemName, item.index);
                
                if (!EncyclopediaProgress.collectedItems.ContainsKey(key.Item1) ||
                !EncyclopediaProgress.collectedItems[key.Item1])
                {
                    allItemsCollected = false;
                    break;
                }
            }

            if (allItemsCollected)      //對已收集完畢之章節做靜態保存
            {
                // 記錄已解鎖章節狀態
                if (!EncyclopediaProgress.unlockedChapters.ContainsKey(chapter.chapterName))
                    EncyclopediaProgress.unlockedChapters[chapter.chapterName] = true;
                chapter.unlockButton.gameObject.SetActive(true);    // 顯示按鈕
                chapter.unlockButton.interactable = true;           // 確保按鈕可交互
            }
            else
            {
                chapter.unlockButton.gameObject.SetActive(false);   // 隱藏按鈕
                chapter.unlockButton.interactable = false;          // 禁用按鈕交互
            }

            if (allItemsCollected && !isNarcissusUsed)      // 如果道具已收集完全且水仙花未使用，解鎖按鈕
            {
                chapter.plotButton.interactable = true;             //外層按鈕開啟
                SetButtonColor(chapter.plotButton, Color.white);    //顏色設置

                chapter.unlockButton.gameObject.SetActive(true);    //內層按鈕開啟
                chapter.unlockButton.interactable = true;           //內層按鈕交互開啟
                SetButtonColor(chapter.unlockButton, Color.white);  //顏色設置

                //Debug.Log($"章節 {chapter.chapterName} 的按鈕已啟用。");
            }
            else if (!allItemsCollected && !isNarcissusUsed)
            {
                chapter.plotButton.interactable = true;             //外層按鈕開啟
                SetButtonColor(chapter.plotButton, Color.white);    //顏色設置

                chapter.unlockButton.gameObject.SetActive(false);   //內層按鈕開啟
                chapter.unlockButton.interactable = false;          //內層按鈕交互開啟
                SetButtonColor(chapter.unlockButton, Color.white);  //顏色設置
            }
            else  //東西沒拿完還偷吃步 鎖圖鑑
            {
                chapter.plotButton.interactable = false;            //外層按鈕關閉
                SetButtonColor(chapter.plotButton, Color.gray);     //顏色設置

                chapter.unlockButton.interactable = false;          //內層按鈕關閉
                SetButtonColor(chapter.unlockButton, Color.gray);   //顏色設置

                Debug.Log($"章節 {chapter.chapterName} 的按鈕已鎖定。");
            }
        }
        CheckPlotButtonDependencies(); // 檢查依賴的劇情按鈕
    }
    void CheckPlotButtonDependencies()
    {
        foreach (var chapter in chapters)
        {
            if (chapter.plotButton == null || chapter.requiredUnlockChapters == null || chapter.requiredUnlockChapters.Count == 0)
                continue;
            
            bool dependenciesMet = true;

             foreach (var requiredChapterName in chapter.requiredUnlockChapters)
             {
                 if (!EncyclopediaProgress.unlockedChapters.ContainsKey(requiredChapterName) ||
                     !EncyclopediaProgress.unlockedChapters[requiredChapterName])
                 {
                     dependenciesMet = false;
                     break;
                 }
             }
            if (dependenciesMet)
             {
                 chapter.plotButton.interactable = true;
                 chapter.plotButton.gameObject.SetActive(true);
             }
             else
             {
                 chapter.plotButton.interactable = false;
                 chapter.plotButton.gameObject.SetActive(false);
             }
        }
    }
    public void SetButtonColor(Button button, Color color)          // 設置按鈕顏色
    {
        var colors = button.colors;
        colors.disabledColor = color;   // 設置禁用時的顏色
        button.colors = colors;         // 應用新的顏色
    }

    /*-------------------------------------------------道具跨場景--------------------------------------------------------*/
    void RestoreCollectedItems()
    {
        foreach (var pair in itemSlots)
        {
            var key = pair.Key;
            Image itemImage = pair.Value;

            // 如果道具已經被拾取，顯示道具圖標
            if (EncyclopediaProgress.collectedItems.ContainsKey(key.Item1) && EncyclopediaProgress.collectedItems[key.Item1])
            {
                itemImage.sprite = itemData.GetItemByIndex(key.Item2).itemIcon;
                itemImage.color = Color.white;  // 顯示正常顏色
                Debug.Log($"恢復道具: {key.Item1}, 插槽序號: {key.Item2}");
            }
            else               
                itemImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);      // 如果道具未被拾取，保持灰色
        }
    }

    void RestoreUnlockedChapters()
    {
        foreach (var chapter in chapters)
        {
            if (EncyclopediaProgress.unlockedChapters.ContainsKey(chapter.chapterName) && 
                EncyclopediaProgress.unlockedChapters[chapter.chapterName])
            {
                chapter.unlockButton.gameObject.SetActive(true);  // 如果該章節已解鎖，顯示按鈕
                chapter.unlockButton.interactable = true;         // 確保按鈕可交互
            }
            else
                chapter.unlockButton.gameObject.SetActive(false);  // 否則隱藏按鈕

            // 還原鎖定狀態
            if (EncyclopediaProgress.lockedChapters.ContainsKey(chapter.chapterName) &&
                EncyclopediaProgress.lockedChapters[chapter.chapterName])
            {
                if (chapter.plotButton != null)
                {
                    chapter.plotButton.gameObject.SetActive(false);
                }

                if (chapter.unlockButton != null)
                {
                    chapter.unlockButton.gameObject.SetActive(false);
                }
            }
        }
    }
    void RestoreRequiredUnlockChapters()
    {
        foreach (var chapter in chapters)
        {
            if (chapter.requiredUnlockChapters == null || chapter.requiredUnlockChapters.Count == 0)
                continue;

            bool dependenciesMet = true;
            foreach (var requiredChapter in chapter.requiredUnlockChapters)
            {
                if (!EncyclopediaProgress.unlockedChapters.ContainsKey(requiredChapter) ||
                    !EncyclopediaProgress.unlockedChapters[requiredChapter])
                {
                    dependenciesMet = false;
                    break;
                }
            }

            if (dependenciesMet)
            {
                chapter.plotButton.interactable = true;
                chapter.plotButton.gameObject.SetActive(true);
                Debug.Log($"PlotButton for {chapter.chapterName} is now active.");
            }
            else
            {
                chapter.plotButton.interactable = false;
                chapter.plotButton.gameObject.SetActive(false);
            }
        }
    }
    public void IncrementExtraCounter(){ExtraCounter++;}    //結局計數器
}