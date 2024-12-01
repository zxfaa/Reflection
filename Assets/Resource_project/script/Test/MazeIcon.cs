using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeIcon : MonoBehaviour
{
    // Start is called before the first frame update
    /*void OnMouseDown()
    {
        FindObjectOfType<MazeDrag>().ExamineObject();
    }*/

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && FindObjectOfType<InteractionSystem>().isExamine)
        {
            Vector3 mousePos = GetMouseWorldPosition(); // 獲取滑鼠世界座標
            Collider2D hitCollider = Physics2D.OverlapPoint(mousePos);

            // 檢測滑鼠是否點擊到當前物件
            if (hitCollider != null && hitCollider.gameObject == gameObject)
            {
                FindObjectOfType<MazeDrag>().ExamineObject();
            }
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition; // 滑鼠螢幕座標
        mouseScreenPosition.z = Mathf.Abs(Camera.main.transform.position.z); // 確保 z 軸距離正確（相機的深度）
        return Camera.main.ScreenToWorldPoint(mouseScreenPosition); // 轉換為世界座標
    }
}
