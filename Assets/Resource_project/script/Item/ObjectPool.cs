using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;
    public List<ItemData.Item> itemTypes;  // �]�t�h���������D��
    private Dictionary<string, List<GameObject>> pooledObjects;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        pooledObjects = new Dictionary<string, List<GameObject>>();
    }

    public GameObject GetPooledObject(string itemName)
    {
        if (pooledObjects.ContainsKey(itemName))
        {
            foreach (GameObject obj in pooledObjects[itemName])
            {
                if (!obj.activeInHierarchy)
                {
                    return obj;
                }
            }
        }

        // �p�G�����S���i�Ϊ�����A�Ыؤ@�ӷs��
        ItemData.Item item = itemTypes.Find(i => i.itemName == itemName);
        if (item.prefab != null)
        {
            GameObject newObj = Instantiate(item.prefab);
            newObj.SetActive(false);
            if (!pooledObjects.ContainsKey(itemName))
            {
                pooledObjects[itemName] = new List<GameObject>();
            }
            pooledObjects[itemName].Add(newObj);
            return newObj;
        }

        Debug.LogError("Item prefab not found for item: " + itemName);
        return null;
    }
}
