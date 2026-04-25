using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public void PlayGame()
    {
        // Chữ "Level1" phải ĐÚNG Y HỆT tên Scene game của bạn
        SceneManager.LoadScene("Level 1"); 
    }

    public void QuitGame()
    {
        // Chữ "Level1" phải ĐÚNG Y HỆT tên Scene game của bạn
        Application.Quit(); 
    }
}
