using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End : MonoBehaviour
{
    public void ExitGame()
    {
        Debug.Log("ExitGame called. Quitting application.");

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // �����Ҧ� (�b�s�边���ϥ�)
        #else
            Application.Quit(); // �b�c�ت��C�����h�X����
        #endif
    }
}
