using UnityEngine;
using Ghosts;

public class BeAttackedTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<IGhost>()?.AttackAnimation();
        int damage = other.GetComponent<IGhost>()?.GetAttackPower ?? 0;
        GameManager.Instance.TakeDamage(damage);
        SoundManager.Instance.PlayDamageTakeSound(); // �ǉ�
    }
    
    private void OnTriggerExit(Collider other)
    {
    }
}
