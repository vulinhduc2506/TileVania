using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            // Khóa chạy nhảy
            collision.GetComponent<PlayerMovement>().enabled = false; 
            
            // "Gọi điện" báo cáo cho Sếp GameSession
            FindObjectOfType<GameSession>().ProcessLevelComplete();
        }
    }
}
