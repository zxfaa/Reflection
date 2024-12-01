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
            Vector3 mousePos = GetMouseWorldPosition(); // ����ƹ��@�ɮy��
            Collider2D hitCollider = Physics2D.OverlapPoint(mousePos);

            // �˴��ƹ��O�_�I�����e����
            if (hitCollider != null && hitCollider.gameObject == gameObject)
            {
                FindObjectOfType<MazeDrag>().ExamineObject();
            }
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition; // �ƹ��ù��y��
        mouseScreenPosition.z = Mathf.Abs(Camera.main.transform.position.z); // �T�O z �b�Z�����T�]�۾����`�ס^
        return Camera.main.ScreenToWorldPoint(mouseScreenPosition); // �ഫ���@�ɮy��
    }
}
