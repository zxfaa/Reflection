using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    // ���~UI���w�s��
    public GameObject itemUIPrefab;
    // ���~UI�����`�I
    public Transform itemsParent;
    // �Ω�s�x�Ыت����~UI���
    private List<GameObject> itemUIInstances = new List<GameObject>();

    // ��}���ҥήɽե�
    private void OnEnable()
    {
        // �T�{ Inventory ��Ҥ��� null
        if (Inventory.Instance != null)
        {
            // �q�\���~���ܤƨƥ�
            Inventory.Instance.OnInventoryChanged += UpdateInventoryUI;
            // ��l��sUI
            UpdateInventoryUI();
        }
    }

    // ��}���T�ήɽե�
    private void OnDisable()
    {
        // �T�{ Inventory ��Ҥ��� null
        if (Inventory.Instance != null)
        {
            // �����q�\���~���ܤƨƥ�
            Inventory.Instance.OnInventoryChanged -= UpdateInventoryUI;
        }
    }

    // ��s���~��UI
    private void UpdateInventoryUI()
    {
        // �M���{����UI��
        foreach (GameObject itemUI in itemUIInstances)
        {
            Destroy(itemUI); // �P���C�Ӫ��~UI���
        }
        itemUIInstances.Clear(); // �M�ŦC��

        // ������~�椤���Ҧ����~
        List<ItemData.Item> items = Inventory.Instance.GetItems();
        // ���C�Ӫ��~�Ы�UI���
        foreach (ItemData.Item item in items)
        {
            GameObject itemUI = Instantiate(itemUIPrefab, itemsParent); // �Ыت��~UI���
            itemUI.transform.Find("ItemIcon").GetComponent<Image>().sprite = item.itemIcon; // �]�m���~�ϼ�
            itemUI.transform.Find("ItemName").GetComponent<Text>().text = item.itemName; // �]�m���~�W��
            itemUIInstances.Add(itemUI); // �NUI��ҲK�[��C��
        }
    }
}
