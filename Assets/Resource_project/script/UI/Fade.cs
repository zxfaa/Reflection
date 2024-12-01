using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Fade : MonoBehaviour
{
    public GameObject targetUI;  // �ݭn�T�Ϊ� UI ����
    public GameObject nextUI;
    public Player player;

    private void Start()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();   // �۰ʧ�쪱�a�}��
            player.LockMovement(true);   // ��w����
        }
    }

    // �T�� UI ����k
    public void DisableUI()
    {
        if (targetUI != null)
            targetUI.SetActive(false);  // �N�ؼ� UI �]�m���T�Ϊ��A
        if (player != null)
            player.LockMovement(false);   // �Ѱ���w����
    }

    public void NextUI()
    {
        if(nextUI != null)
        {
            targetUI.SetActive(false);
            nextUI.SetActive(true);
        }           
    }
}
