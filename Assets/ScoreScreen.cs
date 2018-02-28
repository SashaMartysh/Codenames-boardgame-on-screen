using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreScreen : MonoBehaviour
{
    public TextMeshProUGUI ScoreLabel;

    void OnEnable()
    {
        ScoreLabel.text = GameManager.Instance.redTeam.scores + " : " + GameManager.Instance.blueTeam.scores;
    }

    public void StartNextRound()
    {
        // TODO START ROUND WITN GM
        gameObject.SetActive(false);
        GameManager.Instance.StartNewRound();
    }

    public void QuitTomainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
