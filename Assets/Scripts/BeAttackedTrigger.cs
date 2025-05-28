using System;
using UnityEngine;

public class BeAttackedTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        int damage = other.GetComponent<IGhost>()?.GetAttackPower() ?? 0;
        GameManager.Instance.TakeDamage(damage);
        Debug.Log("Is be attacked");
    }
    
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Destroy");
        Destroy(other.gameObject);
    }
}
