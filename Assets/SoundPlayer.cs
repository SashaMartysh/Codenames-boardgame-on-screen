using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [Header("Options")]
    public float pitchVariation = 0.05f;

    [Header("Link")]
    public AudioSource audioSource;

    [Header("Clips")]
    public AudioClip[] defeatSound;
    public AudioClip[] applauseSounds;
    public AudioClip[] booSounds;
    public AudioClip[] timeOver;

    public void PlayApplauseSound()
    {
        SetupPitch();
        audioSource.clip = applauseSounds[Random.Range(0, applauseSounds.Length)];
        audioSource.Play();
    }

    public void PlayBooSound()
    {
        SetupPitch();
        audioSource.clip = booSounds[Random.Range(0, booSounds.Length)];
        audioSource.Play();
    }

    public void PlayDefeatSound()
    {
        SetupPitch();
        audioSource.clip = defeatSound[Random.Range(0, defeatSound.Length)];
        audioSource.Play();
    }

    private void SetupPitch()
    {
        audioSource.pitch = Random.Range(1.0f-pitchVariation, 1.0f+pitchVariation);
    }

    public void PlayTimeOverSound()
    {
        SetupPitch();
        audioSource.clip = timeOver[Random.Range(0, timeOver.Length)];
        audioSource.Play();
    }
}
