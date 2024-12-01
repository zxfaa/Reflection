using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class UIDragHandler : MonoBehaviour
{
    public GameObject gameobject;

    private Vector3 mouseWorldPosition;
    private Vector3 offset;
    private bool isDragging = false;
    private Rigidbody2D rb;
    private Vector3 initialPosition; // 初始位置

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = false; // 確保物件可以被手動控制，而不受物理力影響
        initialPosition = transform.position;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && FindObjectOfType<InteractionSystem>().isExamine)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 將滑鼠位置轉為 Ray
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                Debug.Log("DRAG");
                isDragging = true;
                mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPosition.z = 0; // 確保 z 軸為 0（2D 場景）

                offset = transform.position - mouseWorldPosition; // 計算偏移量
                gameobject.SetActive(true); // 顯示拖曳的目標物件
            }
        }

        // 偵測滑鼠放開
        if (Input.GetMouseButton(1) && isDragging && FindObjectOfType<InteractionSystem>().isExamine)
        {
            Debug.Log("DROP");
            isDragging = false;
            gameobject.SetActive(false); // 隱藏拖曳的目標物件
            rb.MovePosition(initialPosition); // 放回初始位置
        }
    }

    void FixedUpdate() // 使用物理更新進行平滑移動
    {
        if (isDragging)
        {
            mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0; // 確保 z 軸為 0（2D 場景）

            Vector2 targetPosition = (Vector2)(mouseWorldPosition + offset); // 計算目標位置
            rb.MovePosition(Vector2.Lerp(rb.position, targetPosition, 0.5f)); // 平滑移動
        }
    }
}
