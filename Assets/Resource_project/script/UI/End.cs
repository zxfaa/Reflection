using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End : MonoBehaviour
{
    public void ExitGame()
    {
        Debug.Log("ExitGame called. Quitting application.");

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // 停止播放模式 (在編輯器內使用)
        #else
            Application.Quit(); // 在構建的遊戲中退出應用
        #endif
    }
}
