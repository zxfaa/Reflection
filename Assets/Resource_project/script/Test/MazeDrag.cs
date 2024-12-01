using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Flower;
using Cinemachine;

public class MazeDrag : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public Transform player;
    public Transform maze;
    public Collider2D changeConfiner;
    public Text hintText;

    InteractionSystem imteVariable;
    FlowerSystem fs;
    Collider2D confiner;
    // Start is called before the first frame update
    void Start()
    {
        imteVariable = FindObjectOfType<InteractionSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExamineObject()
    {
        Debug.Log("CLICK");
        imteVariable.isExamine = !imteVariable.isExamine;
        
        if (imteVariable.isExamine)
        {
            confiner = virtualCamera.GetComponent<CinemachineConfiner>().m_BoundingShape2D;
            virtualCamera.GetComponent<CinemachineConfiner>().m_BoundingShape2D = changeConfiner;
            virtualCamera.Follow = maze;
            virtualCamera.LookAt = maze;
            hintText.gameObject.SetActive(false);
        }
        else
        {
            virtualCamera.GetComponent<CinemachineConfiner>().m_BoundingShape2D = confiner;
            virtualCamera.Follow = player;
            virtualCamera.LookAt = player; 
            hintText.gameObject.SetActive(true);
        }

        imteVariable.menuBar.SetActive(!imteVariable.isExamine);
        imteVariable.narcissus.SetActive(!imteVariable.isExamine);
    }
}
