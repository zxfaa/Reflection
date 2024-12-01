using UnityEngine;
using UnityEngine.EventSystems;

public class Maze : MonoBehaviour
{
    public Item mazeKey;
    private Animator animator;

    void Start()
    {
        mazeKey = GetComponent<Item>();
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("拖曳物件進入目標區域");
        if (other.CompareTag("Draggable"))
        {
            Debug.Log("拖曳物件進入目標區域");
            animator.SetBool("isGet", true);
            mazeKey.PickUpItem();
            gameObject.gameObject.SetActive(false);
        }
    }
}
