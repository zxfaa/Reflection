using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flower;

public class BookCase : MonoBehaviour
{
    FlowerSystem fs;
    public Item item;
    public bool HasItem;

    //private bool isPlayerInRange = false; // 記錄玩家是否在範圍內
    //private bool isInteracting = false; // 紀錄是否正在互動

    public GameObject player; // Player對象
    private InteractionSystem interactionSystem;

    // Start is called before the first frame update
    void Start()
    {
        fs = FlowerManager.Instance.GetFlowerSystem("default");
        interactionSystem = player.GetComponent<InteractionSystem>();

    }

    /*void Update()
    {
        // 當玩家在範圍內且按下指定按鍵
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Mouse0) && !isInteracting )
        {
            isInteracting = true;  // 設置為正在互動
                                   // 使用 InteractionSystem 的檢測方法
            if (interactionSystem != null && interactionSystem.DetectObject() && interactionSystem.IsMouseOverObject())
            {
                BookInteration();
            }
            else
            {
                Debug.Log($"互動條件不符合，無法執行互動{interactionSystem.DetectObject()} {interactionSystem.IsMouseOverObject()}");
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
    }*/
    public void BookInteration()
    {
        fs.SetupDialog("EnviromentDialogPrefab");
        fs.SetTextList(new List<string> { "書櫃裡傳來了異樣的聲響[w][remove_dialog]" });
        StartCoroutine(WaitForDialogCompletion(() =>
        {
            fs.SetupButtonGroup();
            fs.SetupButton("查看", () => {
                fs.Resume();
                fs.SetupDialog("EnviromentDialogPrefab");
                fs.SetTextList(new List<string> { "長滿灰塵的書出現在你的眼前[w][remove_dialog]" });
                fs.RemoveButtonGroup();
                OpenBook();
            });
            fs.SetupButton("離開", () => {
                fs.Resume();
                fs.RemoveButtonGroup();
                fs.SetupDialog("PlayerDialogPrefab");
                fs.SetTextList(new List<string> { "可能是我聽錯了吧[w][remove_dialog]" });
                /*StartCoroutine(WaitForDialogCompletion(() =>
                {
                    isInteracting = false;  // 重置交互标志
                }));*/

            });
        }));

       
    }

    private void OpenBook()
    {
        StartCoroutine(WaitForDialogCompletion(() =>
        {
            fs.SetupButtonGroup();
            fs.SetupButton("打開", () => {
                if (HasItem)
                {
                    fs.Resume();
                    fs.SetupDialog("EnviromentDialogPrefab");
                    fs.SetTextList(new List<string> { "一塊作工精美的拼圖掉在你眼前[w][remove_dialog]" });
                    fs.RemoveButtonGroup();
                    StartCoroutine(WaitForDialogCompletion(() =>
                    {
                        //獲得道具
                        Debug.Log("獲得道具");
                        item.PickUpItem();
                        item.interactionType = Item.InteractionType.Others;
                        HasItem = false;
                    }));
                }
                else
                {
                    fs.SetupDialog("EnviromentDialogPrefab");
                    fs.SetTextList(new List<string> { "內部甚麼都沒有[w][remove_dialog]" });
                    fs.RemoveButtonGroup();
                }
                /*StartCoroutine(WaitForDialogCompletion(() =>
                {
                    isInteracting = false;  // 重置交互标志
                }));*/
            });
            fs.SetupButton("關閉", () => {
                fs.SetupDialog("PlayerDialogPrefab");
                fs.SetTextList(new List<string> { "可能是我聽錯了吧[w][remove_dialog]" });
                fs.RemoveButtonGroup();
                /*StartCoroutine(WaitForDialogCompletion(() =>
                {
                    isInteracting = false;  // 重置交互标志
                }));*/
            });
        }));
       
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
