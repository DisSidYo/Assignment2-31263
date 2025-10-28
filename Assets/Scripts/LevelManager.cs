using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // ✅ Needed for TextMeshPro

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public static int score = 0;
    public static int totalPellets = 0;

    [Header("UI References")]
    public TextMeshProUGUI scoreText; // ✅ Assign in Inspector

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // ✅ Only find dynamically if not assigned in Inspector
        if (scoreText == null)
        {
            GameObject scoreObj = GameObject.FindGameObjectWithTag("Score");
            if (scoreObj != null)
                scoreText = scoreObj.GetComponent<TextMeshProUGUI>();
            else
                Debug.LogError("⚠️ LevelManager: No GameObject with tag 'Score' found in scene.");
        }

        // Count total pellets
        
        score = 0;
        UpdateScoreText();
    }

    void Update()
    {
        UpdateScoreText();
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString("D6"); // e.g. "Score: 000125"
    }
}
