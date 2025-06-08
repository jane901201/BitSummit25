using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Sound Effects")]
    public AudioClip damageTakeClip;
    public AudioClip damageMakeClip;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }


�@�@//�_���[�W���󂯂�SE
    public void PlayDamageTakeSound()
    {
        if (damageTakeClip != null)
            audioSource.PlayOneShot(damageTakeClip);
    }

    //�_���[�W��^����SE
    public void PlayDamageMakeSound()
    {
        if (damageMakeClip != null)
            audioSource.PlayOneShot(damageMakeClip);
    }
}
