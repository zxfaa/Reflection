using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flower;
using UnityEngine.UI;

public class JudeMentRoom : MonoBehaviour
{

    FlowerSystem fs;
    public static bool JudgeMentIsEnd;
    private int ExtraCount = EncyclopediaUI.ExtraCounter;
    private int NarcissusCount = Narcissus.NarcissusUseCount;

    private bool isPlayerInRange = false;  // 記錄玩家是否在範圍內
    private bool isInteracting = false;    // 記錄是否正在互動

    public Animator fadeAnimator;
    public Image fadeImage;
    public Item ToiletDoor;
    void Start()
    {
        fs = FlowerManager.Instance.GetFlowerSystem("default");
    }

    void Update()
    {
        // 當玩家在範圍內且按下指定按鍵時觸發交互
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Mouse0) && !isInteracting && Dog.IsLeave && !JudgeMentIsEnd )
        {
            isInteracting = true;  // 設置為正在互動
            Play();
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
    private void Play()
    {
        StartCoroutine(PlayAnimation());

    }

    private IEnumerator PlayAnimation()
    {
        // 鎖定角色移動
        FindObjectOfType<Player>().LockMovement(true);
        // 使 fadeImage 顯示
        fadeImage.gameObject.SetActive(true);

        // 觸發 "FadeInTrigger" 動畫
        fadeAnimator.SetTrigger("FadeInTrigger");

        // 等待 1.5 秒
        yield return new WaitForSeconds(1.5f);

        JudgeMentTrigger();
        JudgeMentIsEnd= true;

        FindObjectOfType<Player>().LockMovement(false);
        // 觸發 "FadeOutTrigger" 動畫
        fadeAnimator.SetTrigger("FadeOutTrigger");

        // 等待動畫播放結束 
        yield return new WaitForSeconds(1.5f);

        // 停止動畫後，將 fadeImage 隱藏
        fadeImage.gameObject.SetActive(false);


    }

    private void JudgeMentTrigger()
    {
        fs.SetupUIStage("default", "DefaultUIStagePrefab", 8);
        fs.SetupDialog("PlotDialogPrefab");

        if (NarcissusCount > 2)
        {
            //憤怒
            fs.ReadTextFromResource("Stage3/judge/Angry");
            
        }
        if (NarcissusCount < 2 && NarcissusCount >= 1)
        {
            //討價還價選項
            fs.ReadTextFromResource("Stage3/judge/Argue");
        }
        if (NarcissusCount == 0 && ExtraCount < 3)
        {
            //沮喪選項
            fs.ReadTextFromResource("Stage3/judge/Despress");
        }
        if (NarcissusCount == 0 && ExtraCount == 3)
        {
            //接受選項
            fs.ReadTextFromResource("Stage3/judge/Accept");
        }
        ToiletDoor.interactionType = Item.InteractionType.Others;

    }
    
}
