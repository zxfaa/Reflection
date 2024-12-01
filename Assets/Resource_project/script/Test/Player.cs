using Flower;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float speed = 3f;

    public Vector2 moveDirection = Vector2.zero;
    public Vector2 targetPosition;
    public Animator animator;

    private bool isLocked = false;

    private bool facingRight = true;
    public bool isOpeningUI;

    FlowerSystem fs;
    private StringBuilder cheatInput = new StringBuilder();

    void Start()
    {
        targetPosition = transform.position;
        animator = GetComponent<Animator>();
        fs = FlowerManager.Instance.GetFlowerSystem("default");
    }

    void Update()
    {
        HandleCheatInput();

        if (FindObjectOfType<InventorySystem>().isDragging)
            FindObjectOfType<DragAndDrop>().DraggingItem();

        if (!CanMove())
        {
            targetPosition = transform.position;
            animator.SetBool("isMoving", false);
            return;
        }

        if (Input.GetMouseButtonDown(1))
            RightClick();
    }

    void FixedUpdate()
    {
        Move();
        
    }

    void RightClick()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z;
        targetPosition = new Vector2(mousePosition.x, transform.position.y);
        moveDirection = (targetPosition - (Vector2)transform.position).normalized;
    }

    void Move()
    {
        animator.SetBool("isMoving", true);
        Vector2 currentPosition = transform.position;
        float newPosition = Mathf.MoveTowards(currentPosition.x, targetPosition.x, speed * Time.deltaTime);
        currentPosition = new Vector2(newPosition, currentPosition.y);
        transform.position = currentPosition;

        if (Vector2.Distance(currentPosition, targetPosition) < 0.1f)
        {
            animator.SetBool("isMoving", false);
            moveDirection = Vector2.zero;
        }

        // 音效播放邏輯
        if (animator.GetBool("isMoving"))
        {
            if (!AudioManager.Instance.IsLoopingSoundPlaying ()) // 確保只在音效未播放時啟動
            {
                AudioManager.Instance.PlayLoopingSound("Walking");
            }
        }
        else
        {
            AudioManager.Instance.StopLoopingSound();
        }


        if (moveDirection.x > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveDirection.x < 0 && facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    bool CanMove()
    {
        bool can = true;
        
        if (FindObjectOfType<InventorySystem>().isDragging)
            can = false;
        if (InventorySystem.Instance.isOpen)
            can = false;
        if (!fs.isCompleted)
            can = false;
        if (isOpeningUI)
            can = false;
        if (FindObjectOfType<TeleportManager>().isTeleport)
            can = false; 
        if (FindObjectOfType<InteractionSystem>().isExamine)
            can = false;
        if (isLocked)
            can =  false;

        return can;
    }

    public void LockMovement(bool lockState)
    {
        isLocked = lockState;
        // 停止角色移動動畫
        if (lockState)
        {
            targetPosition = transform.position;
            animator.SetBool("isMoving", false);
        }
    }
    public void IsOpeningUI()
    {
        isOpeningUI = !isOpeningUI;
    }

    public int GetSceneIndex ()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        return currentSceneIndex;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Collider"))
        {
            targetPosition = transform.position;
            moveDirection = Vector2.zero;
            animator.SetBool("isMoving", false);
            AudioManager.Instance.StopLoopingSound();
        }
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Collider"))
        {
            animator.SetBool("isMoving", false);
            AudioManager.Instance.StopLoopingSound();
        }
    }

    void HandleCheatInput()
    {
        foreach (char c in Input.inputString) // 監聽玩家按下的按鍵
        {
            if (c == '\b') // 處理退格鍵
            {
                if (cheatInput.Length > 0)
                    cheatInput.Remove(cheatInput.Length - 1, 1);
            }
            else if (c == '\n' || c == '\r') // 處理輸入完成（Enter鍵）
            {
                ProcessCheatCommand(cheatInput.ToString());
                cheatInput.Clear();
            }
            else
            {
                cheatInput.Append(c); // 將輸入的字符加入指令
            }
        }
    }

    void ProcessCheatCommand(string command)
    {
        if (command.ToLower().StartsWith("speed"))
        {
            string[] parts = command.Split(' ');
            if (parts.Length > 1 && float.TryParse(parts[1], out float newSpeed))
            {
                speed = newSpeed;
                Debug.Log($"玩家速度已設置為 {newSpeed}");
            }
            else
            {
                Debug.Log("無效的指令格式，使用: speed [數字]");
            }
        }
        else
        {
            Debug.Log($"未知的指令: {command}");
        }
    }
}
