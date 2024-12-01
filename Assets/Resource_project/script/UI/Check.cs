using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Check : MonoBehaviour
{
    public GameObject sure;
    public Player player;
    void Start()=>player = FindObjectOfType<Player>();
    public void open()
    {
        sure.SetActive(true);
        player.IsOpeningUI();
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void close()
    {
        sure.SetActive(false);
        player.IsOpeningUI();
        EventSystem.current.SetSelectedGameObject(null);
    }
}
