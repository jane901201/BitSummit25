using UnityEngine;

public class WaveTrigger : MonoBehaviour
{
    [Tooltip("このオブジェクトが範囲に入ったらWaveを発動")]
    public GameObject targetObject;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.gameObject == targetObject)
        {
            Debug.Log("wave変更");
            triggered = true;
            FindObjectOfType<WaveController>()?.TriggerWave();
        }
    }
}


