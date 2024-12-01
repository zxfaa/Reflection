using UnityEngine;

[CreateAssetMenu(fileName = "NewBoardEffect", menuName = "ItemEffects/BoardEffect")]
public class BoardEffect : ItemEffectBase
{
    public override void UseEffect(GameObject target)
    {
        // ����ʵe���޿�
        Animator animator = target.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("PlayAnimation"); // ���]�ʵe�ϥΪ��O�@�ӦW��"PlayAnimation"��Trigger
        }
        else
        {
            Debug.LogWarning("Target does not have an Animator component.");
        }

        // ����D�㪺�޿�
        InteractionHandler interactionHandler = target.GetComponent<InteractionHandler>();
        if (interactionHandler != null)
        {
            interactionHandler.PickUp();
        }
        else
        {
            Debug.LogWarning("Target does not have a InteractionHandler component.");
        }
        /*Inventory.Instance.AddItem(this.item); // ���]���@��Inventory�޲z��
        Debug.Log("Item used: " + this.item.itemName);*/
    }
}