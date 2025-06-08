using UnityEngine;
using Ghosts;

public class BeAttackedTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        int damage = other.GetComponent<IGhost>()?.GetAttackPower() ?? 0;
        GameManager.Instance.TakeDamage(damage);
    }
    
    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<IGhost>()?.AttackAnimation();
        SoundManager.Instance.PlayDamageTakeSound(); // �ǉ�
    }
}
