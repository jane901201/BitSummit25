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
        GameManager.Instance.RemoveGhost(other.GetComponent<IGhost>());
        other.gameObject.SetActive(false);
    }
}
