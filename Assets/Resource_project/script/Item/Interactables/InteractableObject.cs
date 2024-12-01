using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public virtual void OnItemUsed(DraggableItem item)
    {
        Debug.Log("�ϥΤF�D��G" + item.name + " �b����G" + gameObject.name);
    }
}
