using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Choice : MonoBehaviour
{
    public GameObject choiceMenu = null; // ��ܿ��
    public GameObject choiceButton = null; // ��ܿ�檺���s

    public void ShowChoiceMenu()
    {
        AudioManager.Instance.PlayOneShot("ClickButton"); // ������s�I���n��
        if (choiceMenu != null)
            choiceMenu.SetActive(true); // ��ܿ��
        if (choiceButton != null)
            choiceButton.SetActive(false); // ���ÿ����s
    }
    public void HideChoiceMenu()
    {
        AudioManager.Instance.PlayOneShot("ClickButton"); // ������s�I���n��
        if (choiceMenu != null)
            choiceMenu.SetActive(false); // ���ÿ��
        if (choiceButton != null)
            choiceButton.SetActive(true); // ��ܿ����s
    }
}
