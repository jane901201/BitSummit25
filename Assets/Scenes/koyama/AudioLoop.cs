using UnityEngine;

public class AudioLoop : MonoBehaviour
{
    public AudioSource audioSource_intro;   //�C���g����AudioSource
    public AudioSource audioSource_loop;    //���[�v�p��AudioSource

    //�C���g�����Đ����ꂽ���ǂ������L�^����t���O
    private bool hasPlayedOnce = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource_intro.Play(); // �ŏ��ɃC���g�����Đ�

        //���[�v�p��AudioSource�𖳌��ɂ���
        audioSource_loop.enabled = false; // ���[�v�p��AudioSource�𖳌���
    }

    // Update is called once per frame
    void Update()
    {
        // �Đ����~�܂���(���Ō�܂ōĐ�)���A�܂����[�v�������Ă��Ȃ��ꍇ
        if (!audioSource_intro.isPlaying && !hasPlayedOnce)
        {
            hasPlayedOnce = true; // 1��ڂ̍Đ����I��������Ƃ��L�^

            // ���[�v��AudioSource��L���ɂ���
            audioSource_loop.enabled = true; // ���[�v�p��AudioSource��L����
            audioSource_loop.loop = true; // ���[�v��L���ɂ���
            audioSource_loop.Play(); // �Đ����J�n
        }

    }
}