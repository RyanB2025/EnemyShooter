using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject winPanel;
    public GameObject losePanel;

    private void Start()
    {
        // Ensure panels are hidden when the game starts
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);

        // Ensure time is moving normally
        Time.timeScale = 1f;
    }

    public void TriggerLoseState()
    {
        losePanel.SetActive(true);
        Time.timeScale = 0f; // Pauses the game
    }

    public void TriggerWinState()
    {
        winPanel.SetActive(true);
        Time.timeScale = 0f; // Pauses the game
    }
}
