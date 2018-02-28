using UnityEngine;

public class Card : MonoBehaviour {

    public FractionType fraction;

    [Header("Links")]
    public AudioSource audioSource;
    public AudioClip[] soundClips;


    public void ShowDebugInformation()
    {
        Debug.Log("Fraction: " + fraction.ToString());
    }

    public void CardSelected()
    {
        GameManager.Instance.SelectCard (this);
    }

    public void SetRandomAngle()
    {
        transform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(-2f, 2f));
    }

    public void SetRandomAngle (float maxAngleRange)
    {
        transform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(-maxAngleRange, maxAngleRange));
    }

    public void SetRandomOffset(float maxOffsetRange)
    {
        RectTransform rt = GetComponent<RectTransform>();
        //rt.localPosition = new Vector3 (Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0f);
        rt.anchoredPosition = new Vector2(Random.Range(-maxOffsetRange, maxOffsetRange), Random.Range(-maxOffsetRange, maxOffsetRange));
    }

    public void  MoveToNewLocation(Transform destination, bool neatly)
    {
        GameManager.Instance.isSomeCardMoving = true;
        transform.SetParent(GameManager.Instance.containerForMovedCards);
        PlaySound();
        LeanTween.move(gameObject, destination, 1.5f).setOnComplete(() =>
        {
            transform.SetParent(destination);
            if (!neatly)
            {
                SetRandomOffset(40f);
                SetRandomAngle(5f);
            }
            else
            {
                SetRandomOffset (10f);
                SetRandomAngle(2.5f);
            }
            GameManager.Instance.isSomeCardMoving = false;
        });
    }

    public void PlaySound()
    {
        audioSource.clip = soundClips[Random.Range(0, soundClips.Length)];
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.Play();
    }
}

public enum FractionType
{
    redTeam = 1,
    blueTeam = 2,
    neutral = 3,
    death = 4
}
