using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class RandomMusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip[] musicClips;
    [SerializeField] private float intervalMinutes = 1f;
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool shuffle = true;

    private AudioSource audioSource;
    private WaitForSeconds waitInterval;
    private int currentClipIndex = 0;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        waitInterval = new WaitForSeconds(intervalMinutes * 60f);

        if (shuffle)
        {
            ShuffleClips();
        }
    }

    private void Start()
    {
        if (playOnStart)
        {
            StartCoroutine(PlayRandomMusicRepeatedly());
        }
    }

    private IEnumerator PlayRandomMusicRepeatedly()
    {
        while (true)
        {
            PlayRandomClip();
            yield return new WaitUntil(() => !audioSource.isPlaying);
            yield return waitInterval;
        }
    }

    private void PlayRandomClip()
    {
        if (musicClips == null || musicClips.Length == 0)
        {
            Debug.LogWarning("No music clips assigned!");
            return;
        }

        // Get next clip (loops back to 0 if at end)
        currentClipIndex = (currentClipIndex + 1) % musicClips.Length;
        audioSource.clip = musicClips[currentClipIndex];
        audioSource.Play();
    }

    private void ShuffleClips()
    {
        // Fisher-Yates shuffle algorithm
        for (int i = musicClips.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            AudioClip temp = musicClips[i];
            musicClips[i] = musicClips[j];
            musicClips[j] = temp;
        }
    }

    public void PlayNextClip()
    {
        StopAllCoroutines();
        PlayRandomClip();
        StartCoroutine(PlayRandomMusicRepeatedly());
    }

    public void SetInterval(float minutes)
    {
        intervalMinutes = minutes;
        waitInterval = new WaitForSeconds(intervalMinutes * 60f);
    }
}