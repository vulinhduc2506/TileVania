using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameSession : MonoBehaviour
{
    [SerializeField] int playerLives = 3;
    [SerializeField] int score = 0;
    [SerializeField] TextMeshProUGUI livesText;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] GameObject gameOverPanel;

    [Header("Win Level UI")]
    [SerializeField] GameObject levelCompletePanel; 
    [SerializeField] TextMeshProUGUI titleText; 
    [SerializeField] TextMeshProUGUI nextLevelButtonText;

    [Header("Final Score UI")]
    [SerializeField] TextMeshProUGUI gameOverScoreText;
    [SerializeField] TextMeshProUGUI winScoreText;
    void Awake()
    {
        int numGameSession = FindObjectsOfType<GameSession>().Length;
        if (numGameSession > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        livesText.text = playerLives.ToString();
        scoreText.text = score.ToString();
    }

    public int GetScore()
    {
        return score;
    }

    public void ProccessPlayerDeath()
    {
        if (playerLives > 1)
        {
            TakeLife();
        }
        else
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                gameOverScoreText.text = "SCORE: " + score.ToString();
            }
            // Đóng băng thời gian để quái vật ngừng di chuyển
            Time.timeScale = 0f;
        }
    }

    public void AddToScore(int pointEachCoin)
    {
        score += pointEachCoin;
        scoreText.text = score.ToString();
    }

    public void ResetGameSession()
    {
        FindObjectOfType<ScenePersist>().ResetSencePersist();
        SceneManager.LoadScene(1);
        score = 0;
        Destroy(gameObject);
    }

    private void TakeLife()
    {
        playerLives--;
        livesText.text = playerLives.ToString();
        int currentSenceIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSenceIndex);
    }

    public void RetryGame()
    {
        Time.timeScale = 1f; // Mở khóa thời gian
        ResetGameSession();
        
        // Ẩn bảng Game Over đi để chơi tiếp
        gameOverPanel.SetActive(false); 
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        FindObjectOfType<ScenePersist>().ResetSencePersist();
        SceneManager.LoadScene("Menu");
        Destroy(gameObject); // Về menu thì tự hủy cỗ máy GameSession đi
    }

    // CHUYỂN HÀM NÀY TỪ LEVEL EXIT SANG ĐÂY
    public void ProcessLevelComplete()
    {
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
            
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int totalScenes = SceneManager.sceneCountInBuildSettings;
            winScoreText.text = "SCORE: " + score.ToString();

            if (currentSceneIndex + 1 == totalScenes)
            {
                titleText.text = "GAME\nCLEARED!"; 
                nextLevelButtonText.text = "PLAY AGAIN"; 
            }
            else
            {
                titleText.text = "LEVEL\nCOMPLETE";
                nextLevelButtonText.text = "NEXT LEVEL";
            }
        }
        Time.timeScale = 0f; 
    }

    // NÚT NEXT LEVEL SẼ GỌI HÀM NÀY NẰM NGAY TẠI GAMESESSION
    public void LoadNextLevelByButton()
    {
        Time.timeScale = 1f; 
        
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            ResetGameSession(); // Về màn 0 và tự hủy
        }
        else
        {
            // VÌ HÀM NÀY NẰM CÙNG NHÀ VỚI PANEL NÊN LỆNH TẮT CHẮC CHẮN ĂN 100%
            if(levelCompletePanel != null) { levelCompletePanel.SetActive(false); }
            
            FindObjectOfType<ScenePersist>().ResetSencePersist();
            SceneManager.LoadScene(nextSceneIndex);
        }
    }
}
