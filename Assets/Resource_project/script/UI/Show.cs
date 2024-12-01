    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Show : MonoBehaviour
{
    public GameObject menu = null;      //���
    public GameObject mainMenu = null;  //���s
    public Player player;               // �ޥΪ��a���ʸ}��
    void Start()
    {
        // �b��������� Player �}��
        player = FindObjectOfType<Player>();
    }

    public void ActiveMainMenu()
    {
        AudioManager.Instance.PlayOneShot("ClickButton");
        if (menu != null)
            menu.SetActive(true);       //��ܿ��        
        if (mainMenu != null)           //�Y���ɫ��s�����        
            mainMenu.SetActive(false);  //�ϫ��s�����               
        if (player != null)
            player.IsOpeningUI();       // �аO UI ���b�}�ҡA�T���
        // ���� `EventSystem` �u�B�z��Ų�� UI �ƥ�
        EventSystem.current.SetSelectedGameObject(null);                
    }

    public void Back()
    {
        AudioManager.Instance.PlayOneShot("ClickButton");
        if (menu != null)
            menu.SetActive(false);
        if (mainMenu != null)
            mainMenu.SetActive(true);
        if (player != null)
            player.IsOpeningUI();       // �аO UI �����A���\����
        // ���� `EventSystem` �u�B�z��Ų�� UI �ƥ�
        EventSystem.current.SetSelectedGameObject(null);                
    }
}
