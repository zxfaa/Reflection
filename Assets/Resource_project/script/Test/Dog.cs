using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flower;

public class Dog : MonoBehaviour
{
    FlowerSystem fs;

    public static bool IsLeave =false;

    private bool isPlayerInRange = false; // 記錄玩家是否在範圍內
    private bool isInteracting = false; // 紀錄是否正在互動

    public GameObject player; // Player對象
    private InteractionSystem interactionSystem;

    void Start()
    {
        fs = FlowerManager.Instance.GetFlowerSystem("default");
        // 獲取 InteractionSystem 參考
        interactionSystem = player.GetComponent<InteractionSystem>();
    }


    void Update()
    {
        // 當玩家在範圍內並按下指定按鍵時，觸發交互
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Mouse0) && !isInteracting && !IsLeave)
        {
            isInteracting = true;  // 設置為正在互動

            // 使用 InteractionSystem 的檢測方法
            if (interactionSystem != null && interactionSystem.DetectObject() && interactionSystem.IsMouseOverObject())
            {
                // 執行您撰寫的互動邏輯
                Chat();
            }
            else
            {
                Debug.Log("互動條件不符合，無法執行互動");
                isInteracting = false; // 若條件不符合，將 `isInteracting` 重置為 `false`
            }
        }
    }

    // 當玩家進入觸發範圍時
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true; // 玩家進入範圍
        }
    }

    // 當玩家離開觸發範圍時
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false; // 玩家離開範圍
        }
    }

    private void Chat()
    {
        fs.SetupUIStage("default", "DefaultUIStagePrefab", 8);
        fs.SetupButtonGroup();

        fs.SetupButton("跟他說話", () => {
            fs.Resume();
            fs.SetupDialog("PlotDialogPrefab");
            fs.ReadTextFromResource("stage3/dog/talk");
            fs.RemoveButtonGroup();
            StartCoroutine(WaitForDialogCompletion(() =>
            {
                isInteracting = false;  // 重置交互标志
            }));
        });

        fs.SetupButton("沉默", () => {
            fs.Resume();
            fs.SetupDialog("PlotDialogPrefab");
            fs.ReadTextFromResource("stage3/dog/silence");
            fs.RemoveButtonGroup();
            StartCoroutine(WaitForDialogCompletion(() =>
            {
                isInteracting = false; 
            }));
        });

        fs.SetupButton("離開", () => {
            fs.Resume();
            fs.RemoveButtonGroup();
            fs.StopAndReset();
            StartCoroutine(WaitForDialogCompletion(() =>
            {
                isInteracting = false;  
            }));
        });
        // 查找 Player 下的 InventorySystem 组件
        InventorySystem inventorySystem = player.GetComponentInChildren<InventorySystem>();
        //這裡放上有沒有蛋糕
        if (inventorySystem.items.Exists(item => item.index == 25))
        {
            fs.SetupButton("拿出蛋糕", () => {
                fs.Resume();
                fs.SetupDialog("PlotDialogPrefab");
                fs.ReadTextFromResource("stage3/dog/cake");
            
                fs.RemoveButtonGroup();
                StartCoroutine(WaitForDialogCompletion(() =>
                {
                    isInteracting = false;
                    gameObject.SetActive(false);
                    IsLeave = true;
                    ItemData.Item cake = ItemData.Instance.GetItemByIndex(25);
                    InventorySystem.Instance.RemoveItem(cake,1);
                }));
                Debug.Log($"{IsLeave}");
                //把互動的trigger移除
            });
        }
        
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
