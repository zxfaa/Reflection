using UnityEngine;

[CreateAssetMenu(fileName = "TeleportEffect", menuName = "ItemEffects/TeleportEffect")]
public class TeleportEffect : ItemEffectBase
{
    public int teleportIndex;

    public override void UseEffect(GameObject target)
    {
        /*Debug.Log("TeleportEffect �Q����");
        TeleportManager manager = FindObjectOfType<TeleportManager>();
        if (manager != null)
        {
            manager.UseKey();
            if (manager.isKeyUsed)
            {
                Debug.Log("�_�ͤw�ϥ�");
                manager.UnlockDoor();
            }
            else
            {
                Debug.Log("�Х��ϥ��_�͸����");
            }
        }
        else
        {
            Debug.LogError("TeleportManager not found.");
        }*/
    }
}
