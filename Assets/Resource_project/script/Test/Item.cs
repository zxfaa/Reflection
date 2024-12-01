using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Flower;
using System.Linq;

[RequireComponent(typeof(BoxCollider2D))]
public class Item : MonoBehaviour
{
    public enum InteractionType { NONE, PickUp, Examine, Others}
    public enum ItemType { NONE, Require, CheckBag }
    public enum DialogueType {NONE, Normal}
    [Header("屬性")]
    public InteractionType interactionType;
    public ItemType itemType;
    public DialogueType dialogueType;
    public GameObject examinePrefab;
    public RectTransform prefabContainer;
    public Transform prefabTransform;
    [Header("獲得道具列表")]
    public ItemData itemData;
    public int[] itemIndices;
    [Header("道具需求")]
    public string requireItemName;
    public int requireItemIndex;
    [Header("設置物件啟用互動功能")]
    // 是否為關鍵物件
    public bool isKeyObject = false; 
    // 物件唯一標識符
    public string objectId;
    public bool canExamine = true;
    [Header("觸發效果")]
    public UnityEvent coustomEvent;
    [Header("其他")]
    // 對話索引，用於觸發特定的對話
    public int dialogueIndex;
    public GameObject instantiatedExaminePrefab;

    FlowerSystem fs;

    private void Reset()
    {
        //GetComponent<BoxCollider2D>().isTrigger = true;
        gameObject.layer = 8;
    }

    void Start() 
    {
        InteractionManager.Instance.RegisterObject(this);
        fs = FlowerManager.Instance.GetFlowerSystem("default");
    }

    // 獲取或創建ExaminePrefab實例
    public GameObject GetOrCreateExaminePrefab()
    {
        if (instantiatedExaminePrefab == null)
        {
            if (prefabContainer != null)
                instantiatedExaminePrefab = Instantiate(examinePrefab, prefabContainer);
            else if (prefabTransform != null)
                instantiatedExaminePrefab = Instantiate(examinePrefab, prefabTransform);
        }
        return instantiatedExaminePrefab;
    }

    public void CheckAndDisableInteraction()
    {
        foreach (int index in itemIndices)
        {
            if (index >= 0 && index < itemData.items.Count)
            {
                ItemData.Item currentItem = itemData.items[index];

                if (currentItem.IsInteration)
                {
                    interactionType = InteractionType.NONE;
                    dialogueIndex = 0;
                    Debug.Log($"Item '{currentItem.itemName}' has already been interacted with. Disabling interaction.");
                }
            }
            else
            {
                Debug.LogError("Item index out of range in CheckAndDisableInteraction");
            }
        }
    }

    public void Interact()
    {
        if (!InteractionManager.Instance.CanInteract(objectId))
        {
            Debug.Log("不能與此物件互動：" + gameObject.name);
            return;
        }


        switch (interactionType)
        {
            case InteractionType.PickUp:
                AudioManager.Instance.PlayOneShot("PickUpThing");
                PickUpItem();
                break;
            case InteractionType.Examine:
                if (canExamine)
                {
                    if (dialogueType != DialogueType.NONE)
                    {
                        TriggerDialogue(dialogueIndex);
                        dialogueType = DialogueType.NONE;
                    }
                    FindObjectOfType<InteractionSystem>().ExamineObject();
                    if (FindObjectOfType<Chest>() != null)
                        FindObjectOfType<Chest>().OpenExamine();
                } 
                break;
            case InteractionType.Others:
                OtherInteraction();
                break;
            case InteractionType.NONE:
                break;
        }

        switch (itemType)
        {
            case ItemType.Require:
                if (FindObjectOfType<InventorySystem>().isDragging)
                    RequireItem(FindObjectOfType<DragAndDrop>().item.itemName);
                else if (dialogueType != DialogueType.NONE)
                    TriggerDialogue(dialogueIndex);
                break;
            case ItemType.CheckBag:
                CheckBagItems();
                break;
        }

        switch (dialogueType)
        {
            case DialogueType.Normal:
                TriggerDialogue(dialogueIndex);
                break;
        }



        InteractionManager.Instance.ObjectInteracted(objectId);
    }

   

    private void UpdateItemInteraction(int index)
    {
        //獲取並更新ItemData
        ItemData.Item currentItem = itemData.items[index];
        currentItem.IsInteration = true;
        itemData.items[index] = currentItem;

        Debug.Log($"Item '{currentItem.itemName}' interaction updated.");
    }
    #region 獲取道具
    public void PickUpItem()
    {
        
        // 如果有對話框，等待其結束；否則直接撿取道具
        if (dialogueType != DialogueType.NONE || !fs.isCompleted)
        {
            StartCoroutine(WaitForDialogCloseAndPickUp());
        }
        else
        {
            // 沒有對話框，直接撿取道具
            DirectPickUp();
        }
    }

    public void PickUpByIndex(int index)
    {
        // 確保索引合法，避免溢出錯誤
        if (index >= 0 && index < itemData.items.Count)
        {
            // 獲取對應索引的道具
            ItemData.Item itemToPick = itemData.items[index];

            // 將道具加入到玩家的 InventorySystem
            InventorySystem.Instance.PickUp(itemToPick);

            // 更新道具的 IsInteration 狀態，表示已被撿取過
            itemToPick.IsInteration = true;
            itemData.items[index] = itemToPick;

            Debug.Log($"Picked up item: {itemToPick.itemName} (Index: {index})");

            // 如果該道具屬於 Extra 類別，可以進行額外處理（如更新圖鑑計數器）
            int[] extraIndices = { 26, 20, 2 }; // 假設 Extra 類道具索引
            if (extraIndices.Contains(index))
            {
                EncyclopediaUI.Instance.IncrementExtraCounter();
                Debug.Log($"Extra item picked. ExtraCounter: {EncyclopediaUI.ExtraCounter}");
            }

            // 更新 UI 或顯示撿取效果
            ImageDisplayManager.Instance.QueueImagesWithFade(itemData, new[] { index });
        }
        else
        {
            Debug.LogError($"Invalid item index: {index}. Cannot pick up.");
        }
    }


    private void DirectPickUp()
    {
        // 定義 Extra 區域的道具索引
        int[] extraIndices = { 26, 20, 2 };

        // 直接進行道具拾取
        foreach (int index in itemIndices)
        {
            if (index >= 0 && index < itemData.items.Count)
            {
                // 處理完後更新IsInteration
                UpdateItemInteraction(index);
                // 優先處理EncyclopediaUI
                FindObjectOfType<InventorySystem>().PickUp(itemData.items[index]);

                // 檢查是否為 Extra 區域的道具
                if (extraIndices.Contains(index))
                {
                    // 增加 ExtraCounter 計數
                    EncyclopediaUI.Instance.IncrementExtraCounter();
                    Debug.Log($"Extra item with index {index} picked up. ExtraCounter increased to {EncyclopediaUI.ExtraCounter}");
                }

            }
            else
            {
                Debug.LogError("Item index out of range in DirectPickUp");
            }
        }

        // 更新圖鑑UI
        if (EncyclopediaUI.Instance != null)
        {
            //EncyclopediaUI.Instance.UpdateUI();
        }
        else
        {
            Debug.LogError("EncyclopediaUI.Instance is null");
        }

        interactionType = InteractionType.NONE;

        // 撿取道具的顯示動畫
        ImageDisplayManager.Instance.QueueImagesWithFade(itemData, itemIndices);
        dialogueIndex = 0;
    }

    private IEnumerator WaitForDialogCloseAndPickUp()
    {
        // 等待對話框開啟
        yield return new WaitUntil(() => !fs.isCompleted);

        // 等待對話框關閉
        yield return new WaitUntil(() => fs.isCompleted);

        // 撿取道具
        DirectPickUp();
    }

    #endregion

    public void RequireItem(string itemName)
    {
        if (requireItemName == itemName)
        {
            //觸發設置的效果
            Debug.Log("Use Item");
            coustomEvent.Invoke();
            FindObjectOfType<DragAndDrop>().StopDragItem();
            FindObjectOfType<InventorySystem>().CheckRightClick();
            itemType = ItemType.NONE;
        }
        else
        {
            TriggerDialogue(dialogueIndex: 14);
            FindObjectOfType<DragAndDrop>().StopDragItem();
            FindObjectOfType<InventorySystem>().CheckRightClick();
            Debug.Log("Worng Item");
        }
    }

    void OtherInteraction()
    {
        if (GetComponent<TeleportTrigger>() != null && GetComponent<OpenDoor>().isDoorUnlocked)
        {
            TeleportTrigger tp = GetComponent<TeleportTrigger>();
            tp.Teleport();
        }
        else if (GetComponent<TransformerBox>() != null)
        {
            TransformerBox transformerBox = GetComponent<TransformerBox>();
            transformerBox.TurnOnLight();
        }
        else if (GetComponent<MazeDrag>() != null)
        {
            MazeDrag md = GetComponent<MazeDrag>();
            md.ExamineObject();
        }
        else if (gameObject.GetComponent<BookCase>() != null)
        {
            coustomEvent.Invoke();
        }
        else if (gameObject.GetComponent<Stage3Counter>() != null)
        {
            coustomEvent.Invoke();
        }
    }

    public void TriggerDialogue(int dialogueIndex)
    {
        if (fs.isCompleted)
        {
            // 從 DialogueManager 中獲取對話資料
            DialogueData dialogueData = DialogueManager.Instance.dialogueData;
            List<string> dialogueLines = dialogueData.GetDialogueByIndex(dialogueIndex);

            if (dialogueLines != null)
            {
                // 根據 SaidByPlayer 屬性選擇不同的對話框預製體
                string dialogPrefabName = dialogueData.dialogues[dialogueIndex].SaidByPlayer
                                          ? "PlayerDialogPrefab"
                                          : "EnviromentDialogPrefab";

                // 設置對話框，並將對話文本列表傳入
                fs.SetupDialog(dialogPrefabName, true, true);
                fs.SetTextList(dialogueLines);
            }
            else
            {
                Debug.LogError("Dialogue lines not found for index: " + dialogueIndex);
            }
        }
    }

    void CheckBagItems()
    {
        foreach (var item in InventorySystem.Instance.items)
        {
            if (item.index == requireItemIndex)
            {
                canExamine = true;
            }
        }
        if (!canExamine)
        {
            TriggerDialogue(dialogueIndex: 22);
        }
    }
}
