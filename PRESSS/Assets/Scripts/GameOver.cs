using UnityEngine;
using TMPro;

public class GameOver : MonoBehaviour
{
    TextMeshProUGUI highScore;

    void Start()
    {
        highScore = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        highScore.text = "HIGH SCORE: " + GameDataManager.highscore.ToString();

    }

    public void Menu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
        GameDataManager.savedScore = 0;
        GameDataManager.savedWave = 0;
    }
}
