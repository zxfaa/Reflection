using Flower;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevel : MonoBehaviour
{

    public LevelLoader levelLoader;
    private bool isPlayerInRange = false;  // 記錄玩家是否在範圍內
    private bool isInteracting = false;    // 記錄是否正在互動
    public GameObject player;
    private InteractionSystem interactionSystem;

    private void Start()
    {
        // 獲取 InteractionSystem 參考
        interactionSystem = player.GetComponent<InteractionSystem>();
    }
    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Mouse0) && !isInteracting)
        {
            Debug.Log("玩家在範圍內，按下了鼠標左鍵且尚未互動");
            isInteracting = true;

            if (interactionSystem != null)
            {
                Debug.Log("找到 InteractionSystem");
                if (interactionSystem.DetectObject() && interactionSystem.IsMouseOverObject() && JudeMentRoom.JudgeMentIsEnd)
                {
                    Debug.Log("條件符合，開始加載下一關");
                    levelLoader.LoadLevel(8);
                }
                else
                {
                    Debug.Log("互動條件不符合");
                    isInteracting = false;
                }
            }
            else
            {
                Debug.Log("未找到 InteractionSystem");
                isInteracting = false;
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

}
