using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Choice : MonoBehaviour
{
    public GameObject choiceMenu = null; // 選擇選單
    public GameObject choiceButton = null; // 顯示選單的按鈕

    public void ShowChoiceMenu()
    {
        AudioManager.Instance.PlayOneShot("ClickButton"); // 播放按鈕點擊聲音
        if (choiceMenu != null)
            choiceMenu.SetActive(true); // 顯示選單
        if (choiceButton != null)
            choiceButton.SetActive(false); // 隱藏選單按鈕
    }
    public void HideChoiceMenu()
    {
        AudioManager.Instance.PlayOneShot("ClickButton"); // 播放按鈕點擊聲音
        if (choiceMenu != null)
            choiceMenu.SetActive(false); // 隱藏選單
        if (choiceButton != null)
            choiceButton.SetActive(true); // 顯示選單按鈕
    }
}
