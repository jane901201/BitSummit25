using UnityEngine;

public class AudioLoop : MonoBehaviour
{
    public AudioSource audioSource_intro;   //イントロのAudioSource
    public AudioSource audioSource_loop;    //ループ用のAudioSource

    //イントロが再生されたかどうかを記録するフラグ
    private bool hasPlayedOnce = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource_intro.Play(); // 最初にイントロを再生

        //ループ用のAudioSourceを無効にする
        audioSource_loop.enabled = false; // ループ用のAudioSourceを無効化
    }

    // Update is called once per frame
    void Update()
    {
        // 再生が止まった(＝最後まで再生)かつ、まだループ処理していない場合
        if (!audioSource_intro.isPlaying && !hasPlayedOnce)
        {
            hasPlayedOnce = true; // 1回目の再生が終わったことを記録

            // ループのAudioSourceを有効にする
            audioSource_loop.enabled = true; // ループ用のAudioSourceを有効化
            audioSource_loop.loop = true; // ループを有効にする
            audioSource_loop.Play(); // 再生を開始
        }

    }
}