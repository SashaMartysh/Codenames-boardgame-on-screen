using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI lblVersion;

	// Use this for initialization
	void Start ()
	{
	    lblVersion.text = "version: " + Application.version;
	}

    public void StartNewGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
