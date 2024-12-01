using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flower;
public class Stage3End : MonoBehaviour
{
    FlowerSystem fs;
    private int ExtraCount = EncyclopediaUI.ExtraCounter;
    private int NarcissusCount = Narcissus.NarcissusUseCount; 

    private void Start()
    {
        fs = FlowerManager.Instance.GetFlowerSystem("default");
        fs.SetupUIStage("default", "DefaultUIStagePrefab", 8);

        StartCoroutine(PlayEnd());

    }

    private IEnumerator PlayEnd()
    {
        fs.SetupDialog("PlotDialogPrefab");
        fs.ReadTextFromResource("plot/no9");
        yield return new WaitUntil(() => fs.isCompleted);   
        StartCoroutine(TrueEnd());
    }

    private IEnumerator TrueEnd()
    {

        fs.SetupUIStage("default", "DefaultUIStagePrefab", 8);
        fs.SetupDialog("PlotDialogPrefab");
        fs.ReadTextFromResource("End/EndIntro");
        yield return new WaitUntil(()=> fs.isCompleted);
        fs.SetupButtonGroup();

        if (NarcissusCount > 2)
        {
            //憤怒選項
            fs.SetupButton("憤怒", () =>
            {
                fs.Resume();
                fs.ReadTextFromResource("End/EndAngry");
                fs.RemoveButtonGroup();
            });

        }
        if (NarcissusCount < 2 && NarcissusCount >= 1)
        {
            //憤怒選項
            fs.SetupButton("憤怒", () =>
            {
                fs.Resume();
                fs.ReadTextFromResource("End/EndAngry");
                fs.RemoveButtonGroup();
            });

            //討價還價選項
            fs.SetupButton("抗爭", () =>
            {
                fs.Resume();
                fs.ReadTextFromResource("End/EndArgue");
                fs.RemoveButtonGroup();
            });
        }
        if (NarcissusCount == 0 && ExtraCount < 3)
        {
            //憤怒選項
            fs.SetupButton("憤怒", () =>
            {
                fs.Resume();
                fs.ReadTextFromResource("End/EndAngry");
                fs.RemoveButtonGroup();
            });

            //沮喪選項
            fs.SetupButton("沮喪", () =>
            {
                fs.Resume();
                fs.ReadTextFromResource("End/EndDrepress");
                fs.RemoveButtonGroup();
            });
        }
        if (NarcissusCount == 0 && ExtraCount == 3)
        {
            //憤怒選項
            fs.SetupButton("憤怒", () =>
            {
                fs.Resume();
                fs.ReadTextFromResource("End/EndAngry");
                fs.RemoveButtonGroup();
            });

            //接受選項
            fs.SetupButton("接受", () =>
            {
                fs.Resume();
                fs.ReadTextFromResource("End/EndAccept");
                fs.RemoveButtonGroup();
            });
        }

    }
    
}
