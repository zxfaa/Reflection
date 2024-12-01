using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flower;

public class Test : MonoBehaviour
{
    FlowerSystem fs;
    public string index;
    // Start is called before the first frame update
    void Start()
    {
        fs = FlowerManager.Instance.GetFlowerSystem("default");
        fs.SetupUIStage();
        fs.SetupDialog("PlotDialogPrefab");
        //fs.ReadTextFromResource("Test");
        watch(index);
    }
    public void watch(string index)
    {
        fs.ReadTextFromResource(index);
    }

   
}
