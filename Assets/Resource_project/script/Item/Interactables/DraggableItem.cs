using UnityEngine;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour
{
    public PlayerMovement playerMovement; // �ޥ�PlayerMovement�}��
    public ItemData.Item item; // �O�s�D��ƾ�

    private bool isDragging = false;
    private Vector3 initialPosition;
    private Image draggingItemImage;
    private RectTransform wheelBackground;

    void Start()
    {
        draggingItemImage = GetComponent<Image>();
        wheelBackground = GetComponentInParent<RectTransform>();
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        if (isDragging)
        {
            DragItemWithMouse();
        }
    }

    public void StartDragging()
    {
        isDragging = true;

        // �T��PlayerMovement�ե�
        if (playerMovement != null)
        {
            playerMovement.LockPlayer();
        }

        transform.SetAsLastSibling(); // �N�즲���D�㲾��̫e��
    }

    void DragItemWithMouse()
    {
        Vector3 mousePos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(wheelBackground, Input.mousePosition, Camera.main, out mousePos);
        transform.position = mousePos;

        if (Input.GetMouseButtonDown(1)) // �k���U�D��
        {
            StopDragging();
        }
    }

    public void StopDragging()
    {
        // ���ըϥιD��
        if (ItemUsageManager.Instance != null)
        {
            ItemUsageManager.Instance.UseItem(item, Input.mousePosition);
        }
        else
        {
            Debug.LogError("ItemUsageManager instance is not found.");
        }

        isDragging = false;
        transform.localPosition = initialPosition; // ���m��m���l��m

        // �ҥ�PlayerMovement�ե�
        if (playerMovement != null)
        {
            playerMovement.ReleasePlayer();
        }

        
    }
}
