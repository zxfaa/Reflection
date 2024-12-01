using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Flower;

public class Chest : MonoBehaviour
{
    public GameObject otherStone;
    public Stone[] slots;
    public int openTimes = 0;
    public bool isCorrect = false;
    FlowerSystem fs;
    [SerializeField] private bool isFirstEnd = false;

    private Item classroomType;


    private void Start()
    {
        fs = FlowerManager.Instance.GetFlowerSystem("default");
        slots = GetComponentsInChildren<Stone>();
        otherStone.SetActive(false);
        GameObject classtable = GameObject.Find("GA Seat");
        classroomType = classtable.GetComponent<Item>();

        foreach (var slot in slots)
        {
            slot.SetChestManager(this);
        }
    }

    public void CheckAllSlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (i >= 6)
            {
                continue;
            }

            if (!slots[i].isItemCorrectlyPlaced)
            {
                Debug.LogWarning("Check");
                return;
            }
        }
        Debug.LogWarning("all correct");
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].enabled = false;
            
            slots[i].GetComponentInParent<Button>().enabled = false;
        }
        TriggerFunction();
    }

    private void TriggerFunction()
    {
        isFirstEnd = true;
        fs.SetupDialog("playerDialogPrefab");
        fs.SetTextList(new List<string> { "還少一顆...?[w]去找找好了，說不定其他地方找的到[w][remove_dialog]" });
        classroomType.interactionType = Item.InteractionType.PickUp;
        classroomType.dialogueIndex = 1;
    }

    public void OpenExamine()
    {
        InteractionSystem interactionSystem = FindObjectOfType<InteractionSystem>();
        if (interactionSystem.isExamine && isFirstEnd)
            openTimes = openTimes + 1;

        if (openTimes == 1)
            HandlePrefabReopened();
    }

    private void HandlePrefabReopened()
    {
        otherStone.SetActive(true);
        FindObjectOfType<LibraConrtoller>().stoneGram = 60;
        FindObjectOfType<LibraConrtoller>().LibraAnimate();
    }
}
