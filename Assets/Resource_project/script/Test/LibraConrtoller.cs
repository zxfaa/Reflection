using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LibraConrtoller : MonoBehaviour
{
    public Animator animator;
    public Stone stone;
    public int stoneGram;
    public int gram;
    public TMP_Text[] textMeshPro;

    private bool operation;  //true for plus, false for minus
    private bool isRight;
    private bool isLeft;
    private bool isDefault;
    private int oneCount = 0;
    private int fiveCount = 0;
    private int tenCount = 0;
    private bool savedIsRight;
    private bool savedIsLeft;
    private bool savedIsDefault;

    void OnEnable()
    {
        // Restore animation state when re-enabled
        isRight = savedIsRight;
        isLeft = savedIsLeft;
        isDefault = savedIsDefault;
        RestoreAnimationState();
    }

    void OnDisable()
    {
        // Save current animation state when disabled
        savedIsRight = isRight;
        savedIsLeft = isLeft;
        savedIsDefault = isDefault;
    }

    public void Operation(bool isPlus) //true for plus, false for minus
    {
        operation = isPlus;
    }  

    public void one()
    {
        if (operation)
        {
            if (oneCount == 5)
                return;
            oneCount++;
            gram += 1;
        }
        else
        {
            if (gram > 0 && oneCount != 0)
            {
                oneCount--;
                gram -= 1;
            }
        }
        UpdateWeightOne(oneCount);
        LibraAnimate();
    }

    public void five() 
    {
        if (operation)
        {
            if (fiveCount == 3)
                return;
            fiveCount++;
            gram += 5;
        }
        else
        {
            if (gram > 4 && fiveCount != 0)
            {
                fiveCount--;
                gram -= 5;
            }  
        }
        UpdateWeightFive(fiveCount);
        LibraAnimate();
    }

    public void ten()
    {
        if (operation)
        {
            if (tenCount == 1)
                return;
            tenCount++;
            gram += 10;
        }
        else
        {
            if (gram > 9 && tenCount != 0)
            {
                tenCount--;
                gram -= 10;
            }
        }
        UpdateWeightTen(tenCount);
        LibraAnimate();
    }

    public void Clear()
    {
        oneCount = 0;
        fiveCount = 0;
        tenCount = 0;
        gram = 0;

        UpdateWeightOne(oneCount);
        UpdateWeightFive(fiveCount);
        UpdateWeightTen(tenCount);

        LibraAnimate();
    }

    public void LibraAnimate()
    {
        isRight = (gram < stoneGram);
        isLeft = (gram > stoneGram);
        isDefault = (gram == stoneGram);

        animator.SetBool("Right", isRight);
        animator.SetBool("Left", isLeft);
        animator.SetBool("Default", isDefault);
    }

    public void SetStoneGram()
    {
        int[] stoneindex = stone.items.itemIndices;
        if (stoneindex[0] == 14)
            stoneGram = 9;
        else if (stoneindex[0] == 11)
            stoneGram = 12;
        else if (stoneindex[0] == 13)
            stoneGram = 14;
        else if (stoneindex[0] == 12)
            stoneGram = 12;
        else if (stoneindex[0] == 9)
            stoneGram = 14;
        else if (stoneindex[0] == 10)
            stoneGram = 15;
        else if (stoneindex[0] == 15 || stoneindex[0] == 16)
            stoneGram = 30;
        else
            stoneGram = 0;
        LibraAnimate();
    }

    public void RestoreAnimationState()
    {
        animator.SetBool("Right", isRight);
        animator.SetBool("Left", isLeft);
        animator.SetBool("Default", isDefault);
        LibraAnimate();
    }

    private void UpdateWeightOne(int value)
    {
        textMeshPro[0].text = value.ToString();
    }

    private void UpdateWeightFive(int value)
    {
        textMeshPro[1].text = value.ToString();
    }

    private void UpdateWeightTen(int value)
    {
        textMeshPro[2].text = value.ToString();
    }
}
