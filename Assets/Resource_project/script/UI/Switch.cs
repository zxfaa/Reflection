using UnityEngine;

public class Switch : MonoBehaviour
{
    public GameObject NormalMenu;   // Normal ��Ų���
    public GameObject ExtraMenu;    // Extra ��Ų���

    // ���� normal �M extra ���A
    public void SwitchMenu()
    {
        if (NormalMenu.activeSelf)
        {
            // �p�G normal �{�b�O��ܪ��A���� normal�A��� extra
            NormalMenu.SetActive(false);
            ExtraMenu.SetActive(true);
        }
        else if (ExtraMenu.activeSelf)
        {
            // �p�G extra �{�b�O��ܪ��A���� extra�A��� normal
            ExtraMenu.SetActive(false);
            NormalMenu.SetActive(true);
        }
    }
}
