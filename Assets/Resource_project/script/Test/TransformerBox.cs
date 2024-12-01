using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformerBox : MonoBehaviour
{
    public static bool isKeyUsed;
    public bool isShodowGone;
    Item itemVariable;

    public void OpenBox()
    {
        itemVariable = GetComponent<Item>();
        isKeyUsed = true;
        itemVariable.TriggerDialogue(dialogueIndex: 13);
        itemVariable.dialogueType = Item.DialogueType.NONE;
    }

    public void TurnOnLight()
    {
        if (isKeyUsed)
        {
            itemVariable.TriggerDialogue(dialogueIndex: 21);
            FindObjectOfType<LeftToRightFadeController>().fade = true;
            isShodowGone = true;
        }   
    }
}
