using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
 
    private List<AudioSource> audioSourcePool = new();
    private int poolSize = 5; // 你可以根据需要调整这个大小
    
    // AudioClips
    [SerializeField] private AudioClip painfulSound;
    [SerializeField] private AudioClip cooldownSound;

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource audioSource = new GameObject("AudioSourcePoolItem").AddComponent<AudioSource>();
            audioSource.gameObject.SetActive(false); // 初始时禁用所有AudioSource对象
            audioSource.transform.SetParent(transform, false); // 将AudioSource对象作为AudioManager的子对象
            audioSourcePool.Add(audioSource);
        }
    }
    
    void Awake()
    {
        // 确保只有一个Audio Manager实例存在
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 在加载新场景时保留Audio Manager
            InitializePool();
        }
        else
        {
            Destroy(gameObject); // 销毁多余的Audio Manager实例
        }
    }
    
    public AudioSource GetAudioSource()
    {
        for (int i = 0; i < audioSourcePool.Count; i++)
        {
            if (!audioSourcePool[i].isPlaying)
            {
                audioSourcePool[i].gameObject.SetActive(true); // 启用AudioSource对象
                return audioSourcePool[i];
            }
        }
 
        // 如果池中没有可用的AudioSource对象，则根据需要可以创建一个新的（这里为了简化没有实现）
        // 但在实际应用中，你可能需要增加池的大小或采取其他策略来处理这种情况
        Debug.LogError("超出Audio对象池的上限");
        return null;
    }

    public void PlayPainfulSound()
    {
        PlaySound(painfulSound, .5f);
    }

    public void PlayCooldownSound()
    {
        PlaySound(cooldownSound, 1f);
    }
    
    public void PlaySound(AudioClip clip, float volume)
    {
        AudioSource audioSource = GetAudioSource();
        if (audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
            audioSource.volume = volume;
 
            // 你可以在这里添加一个监听器来在音效播放完毕后调用ReturnAudioSource
            StartCoroutine(WaitForSoundToFinish(audioSource));
        }
    }
    
    private IEnumerator WaitForSoundToFinish(AudioSource audioSource)
    {
        // 等待直到audioSource不再播放音效
        while (audioSource.isPlaying)
        {
            yield return null; // 等待一帧
        }
 
        // 音效播放完毕，返回AudioSource到池中
        ReturnAudioSource(audioSource);
    }
    
    public void ReturnAudioSource(AudioSource audioSource)
    {
        audioSource.Stop(); // 停止播放音效
        audioSource.gameObject.SetActive(false); // 禁用AudioSource对象以节省资源
        // 注意：这里不需要从池中移除audioSource，因为它仍然在audioSourcePool列表中
    }
}
