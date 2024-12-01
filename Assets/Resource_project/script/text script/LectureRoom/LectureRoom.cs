using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Flower;
using UnityEngine.UI;
using Unity.VisualScripting;

[System.Serializable]
public class Question
{
    public GameObject questionObject;
    public static int idCounter;
    public int Index;
    public TMP_InputField inputField;
    public string correctAnswer;
    
    public Question()
    {
        Index = idCounter++;
    }
}

public class LectureRoom : MonoBehaviour
{
    public List<Question> questions;
    private int currentProgress = 0;
    private int errorCount = 0;
    
    //是否結束
    public static bool MicIsEnd;

    // 用於控制透明度
    public CanvasGroup mask; 
    public CanvasGroup before;
    public CanvasGroup after;

    //InterationSystem 用來關閉此系統
    InteractionSystem interactionSystem;
    FlowerSystem fs;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)){
            fs.Next();
        }
    }
    private void OnEnable()
    {
        HideAllQuestions();
        fs = FlowerManager.Instance.GetFlowerSystem("default");

        if (interactionSystem == null)
        {
            Debug.Log("InteractionSystem can't find");
        }
        StartLectureRoom();
        StartCoroutine(FadeCanvasGroup("mask",0.5f,1)); // 開始淡入背景

    }

    public void StartLectureRoom()
    {

        interactionSystem = FindObjectOfType<InteractionSystem>();
        if (interactionSystem == null)
        {
            Debug.Log("InteractionSystem can't find");
        }
        ClearAllInputFields();
        StartCoroutine(HandleProgress());
    }

    public IEnumerator FadeCanvasGroup(string canvasGroupName, float duration, float targetAlpha)
    {
        CanvasGroup canvasGroup = null;
        switch (canvasGroupName)
        {
            case "mask":
                canvasGroup = mask;
                break;
            case "before":
                canvasGroup = before;
                break;
            case "after":
                canvasGroup = after;
                break;
            default:
                Debug.LogError("找不到指定名称的 CanvasGroup: " + canvasGroupName);
                break;
        }
        //獲取當前透明度
        float startAlpha = canvasGroup.alpha;

        //計算透明度變化速度
        float elapsedTime = 0f;

        //開始淡入or淡出
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            //線性插值
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            yield return null;
        }

        //確保最後的透明度
        canvasGroup.alpha = targetAlpha;
    }



    private IEnumerator HandleProgress()
    {
        while (true)
        {
            switch (currentProgress)
                {

                    case 0:
                        before.alpha = 0;
                        after.alpha = 0;
                        fs.SetupDialog("PlotDialogPrefab");
                        fs.ReadTextFromResource("LectureRoom/LectureRoom");
                        yield return new WaitUntil(() => fs.isCompleted);
                        fs.SetupDialog("PlotDialogPrefab");
                        fs.SetTextList(new List<string> { "首先是字謎題，請選手將推理出的英文單字，填置答案框中喔![w]第一題[w] [remove_dialog]" });
                        yield return new WaitUntil(() => fs.isCompleted);
                        StartCoroutine(FadeCanvasGroup("before", 0.2f, 0.8f));
                        ShowQuestion(currentProgress);
                        break;

                    case 1:
                        fs.SetupDialog("PlotDialogPrefab");
                        fs.SetTextList(new List<string> { "再來是填空題，請將缺空中的中文字轉成單字後回答![w]第二題[w][remove_dialog]" });
                        yield return new WaitUntil(() => fs.isCompleted);
                        StartCoroutine(FadeCanvasGroup("before", 0.2f, 0.5f));
                        ShowQuestion(currentProgress);
                        break;

                    case 2:
                        fs.SetupDialog("PlotDialogPrefab");
                        fs.SetTextList(new List<string> { "是跟第一題很像的題型，請仔細思考吧![w]第三題[w][remove_dialog]" });
                        yield return new WaitUntil(() => fs.isCompleted);
                        StartCoroutine(FadeCanvasGroup("before", 0.2f, 0.4f));
                        ShowQuestion(currentProgress);
                        break;
                    case 3:
                        fs.SetupDialog("PlotDialogPrefab");
                        fs.SetTextList(new List<string> { "這題要看完短詩後作答，若有正確答案就嘗試吧![w]第四題[w][remove_dialog]" });
                        yield return new WaitUntil(() => fs.isCompleted);
                        StartCoroutine(FadeCanvasGroup("before", 0.2f, 0.3f));
                        ShowQuestion(currentProgress);
                        break;
                    case 4:
                        fs.SetupDialog("PlotDialogPrefab");
                        fs.SetTextList(new List<string> { "與第四題很像的題型，請仔細思考吧[w]第五題[w][remove_dialog]" });
                        yield return new WaitUntil(() => fs.isCompleted);
                        StartCoroutine(FadeCanvasGroup("before", 0.2f, 0.2f));
                        ShowQuestion(currentProgress);
                        break;
                    case 5:
                        fs.SetupDialog("PlotDialogPrefab");
                        fs.SetTextList(new List<string> { "已經一半囉，繼續回答吧![w]第六題[w][remove_dialog]" });
                        yield return new WaitUntil(() => fs.isCompleted);
                        StartCoroutine(FadeCanvasGroup("before", 0.2f, 0.1f));
                        ShowQuestion(currentProgress);
                        break;
                    case 6:
                        fs.SetupDialog("PlotDialogPrefab");
                        fs.SetTextList(new List<string> { "[char_name,主持人]請聽我這邊的報告題目來回答吧![w]若你早已察覺了。[w]為何還在此地[w][hide_char]此地?[w]第七題[w][remove_dialog]" });
                        yield return new WaitUntil(() => fs.isCompleted);
                        StartCoroutine(FadeCanvasGroup("before", 0.2f, 0f));
                        ShowQuestion(currentProgress);
                        break;
                    case 7:
                        Debug.Log("All questions completed or error limit reached!");
                        Debug.Log("Errorcunt" + errorCount);
                        if (errorCount > 0)
                        {
                            StartCoroutine(ShowEndMessage());
                            break;
                        }
                        else
                        {
                            StartCoroutine(ShowWinMessage());
                            break;
                        }
                }

            //等待輸入框輸入完成以及對話框結束
            if (currentProgress == 7 || errorCount >= 3)
            {
                break;
            }
            if (currentProgress < questions.Count)
            {
                // 等待輸入框輸入完成以及對話框結束
                yield return new WaitUntil(() => !questions[currentProgress].questionObject.activeSelf && fs.isCompleted);
                currentProgress++;
            }
        }
    }


    public void ShowQuestion(int progress)
    {
        if (progress < 0 || progress >= questions.Count)
        {
            Debug.LogError("Progress value is out of range.");
            return;
        }
        
        // 清除過去添加的監聽器
        questions[progress].inputField.onEndEdit.RemoveListener(OnInputFieldSubmit);
        // 設置當前問題輸入框被啟用
        questions[progress].questionObject.SetActive(true);

        Debug.Log("Current Progress: " + progress);

        // 設置輸入框監聽器
        questions[progress].inputField.onEndEdit.AddListener(OnInputFieldSubmit);

        // 啟用輸入框
        questions[progress].inputField.Select();
        questions[progress].inputField.ActivateInputField();
    }

    private void HideCurrentQuestion()
    {
        questions[currentProgress].questionObject.SetActive(false);
        questions[currentProgress].inputField.onSubmit.RemoveListener(OnInputFieldSubmit);
    }

    private void HideAllQuestions()
    {
        foreach (var question in questions)
        {
            question.questionObject.SetActive(false);
        }
    }

    private void OnInputFieldSubmit(string input)
    {
        // 只在Enter時觸發
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("User submitted input: " + input);

            Question currentQuestion = questions[currentProgress];

            if (input == currentQuestion.correctAnswer)
            {
                Debug.Log("Correct answer!");

                HideCurrentQuestion();
            }
            else
            {
                Debug.Log("Incorrect answer!");
                errorCount++;
                HideCurrentQuestion();

                //錯誤三次
                if (errorCount < 3)
                {
                    //顯示錯誤訊息
                    StartCoroutine(ShowErrorMessage());
                    Debug.Log("Ready for next question.");
                }
                else if (errorCount == 3)
                {
                    StartCoroutine(ShowEndMessage());
                  
                }
            }
        }
    }


    private IEnumerator ShowErrorMessage()
    {
        fs.SetupDialog("PlotDialogPrefab");
        fs.SetTextList(new List<string> { "[async_audio,error,audio/error,false]哦....聽起來有人答錯了哦....[w]但是不要這麼早放棄，我們還有接下來的題目![w][remove_dialog]" });


        yield return new WaitUntil(() => fs.isCompleted);
    }


    private IEnumerator ShowEndMessage()
    {
        fs.SetupDialog("PlotDialogPrefab");
        fs.SetTextList(new List<string> { "[async_audio,fail,audio/fail,false]聽起來有人已經出局囉~下次，還有機會的，如果還有下次的話![w]下次...?[w]抬起頭，卻發現空無一人[w]" });
        yield return new WaitUntil(() => fs.isCompleted);

        //關閉當前畫面
        interactionSystem.ExamineObject();
    }
    //完成後顯示
    public IEnumerator ShowWinMessage()
    {
        fs.SetupDialog("PlotDialogPrefab");
        fs.SetTextList(new List<string> { "....下一題呢?[w]我抬起頭，卻發現空無一人。[play_end]" });
        yield return new WaitUntil(() => fs.isCompleted);
        yield return new WaitForSeconds(1.5f);
        CloseInteration();
        //拿到道具跟加到圖鑑
        Item item = GameObject.Find("Mic").GetComponent<Item>();
        item.PickUpItem();
    }
    //清除所有問題的輸入框內容
    public void ClearAllInputFields()
    {
        foreach (var question in questions)
        {
            if (question.inputField != null)
            {
                question.inputField.text = ""; // 清除輸入框內容
            }
        }

        // 重製進度
        currentProgress = 0;
        errorCount = 0;
    }

    //將互動關閉
    public void CloseInteration()
    {
        MicIsEnd = true;
        GameObject Mic = GameObject.Find("Mic");

        if (Mic != null ) 
        {
          
            Item item = Mic.GetComponent<Item>();
            if (item != null)
            {
                item.interactionType = Item.InteractionType.NONE;
            }

            // 確保 InteractionSystem 存在
            if (interactionSystem != null)
            {
                // 調用 ExamineObject 方法
                interactionSystem.ExamineObject();
            }
            else
            {
                Debug.LogError("Player does not have InteractionSystem component");
            }
        }
    }
}
