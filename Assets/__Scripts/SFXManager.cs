using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;      // So we can call it from anywhere
    public AudioSource audioSource;         // The source used to play sounds
    public AudioClip hitClip;               // Play when hero is hit
    public AudioClip startClip;             // Play when game starts

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        // Make sure we have an audio source
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void PlayHit()
    {
        if (hitClip != null)
            audioSource.PlayOneShot(hitClip);
    }

    public void PlayStart()
    {
        if (startClip != null)
            audioSource.PlayOneShot(startClip);
    }
}
