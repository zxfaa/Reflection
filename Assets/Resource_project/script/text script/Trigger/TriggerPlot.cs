using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Flower;

public class TriggerPlot : MonoBehaviour
{

    private PlotSystem plotSystem;


    public int plotIndex;


    public static bool TriggerClassroomToCorrider; // 教室到走廊
    public static bool TriggerCorriderToClassroom; // 走廊到教室
    public static bool IsTrigger;                 // 觸發後改變

    FlowerSystem fs;
    

    private void Start()
    {

        TriggerClassroomToCorrider = false;
        TriggerCorriderToClassroom = true;
        IsTrigger = false;
        plotSystem = FindObjectOfType<PlotSystem>();
        if (plotSystem == null)
        {
            Debug.LogError("PlotSystem not found in the scene.");
        }
        fs = FlowerManager.Instance.GetFlowerSystem("default");
        

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") )
        {
            if (plotSystem != null && !IsTrigger && TriggerCorriderToClassroom && TriggerClassroomToCorrider)
            { 
                StartCoroutine(DelayedPlotTrigger());
                IsTrigger = true;
            }
            else
            {
                Debug.Log("Already Interation.");
            }
        }
    }

    private IEnumerator DelayedPlotTrigger()
    {
        plotSystem.WatchPlot(plotIndex);
        yield return new WaitUntil(() => fs.isCompleted);
        fs.SetupDialog("playerDialogPrefab");
        fs.SetTextList(new List<string> {"....太晚了...一個人都沒看到。[w] 到處晃晃好了，不知道樓梯間的鐵門被關上了沒有。[w][remove_dialog]"});
    }
}
