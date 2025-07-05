using UnityEngine;
using Ghosts;

/// <summary>
/// 攻撃範囲に入った時に、敵の isAttackable を true に設定する。
/// </summary>
public class AttackableTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<IGhost>()?.SetIsInAttackableRange(true);
        //Debug.Log("入った");
    }
}