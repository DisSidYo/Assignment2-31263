using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    
    public int score = 0;
    public TextMeshProUGUI scoreText;

    [Header("Lives")]
    public int lives = 3;
    public TextMeshProUGUI livesText;

    public TextMeshProUGUI ghostTimerText; // UI element to show remaining time
    public AudioSource musicSource;
    public AudioClip normalMusic;
    public AudioClip scaredMusic;

    public AudioClip deathSound;
    public float powerDuration = 10f;
    public float recoveringThreshold = 3f; // last N seconds -> recovering state

    public List<GhostStateManager> ghosts = new List<GhostStateManager>();

    // runtime state (no coroutines)
    private bool powerActive = false;
    private float powerTimer = 0f;
    private bool recoveringEntered = false;

    // ghost-eaten respawn bookkeeping
    class DeadGhostRecord { public GhostStateManager ghost; public float timer; }
    private List<DeadGhostRecord> deadGhosts = new List<DeadGhostRecord>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateScoreUI();
        if (ghostTimerText != null) ghostTimerText.gameObject.SetActive(false);

        UpdateLivesUI();

        if (musicSource != null && normalMusic != null)
        {
            musicSource.clip = normalMusic;
            musicSource.Play();
        }
    }

    void Update()
    {
        if (powerActive)
        {
            powerTimer -= Time.deltaTime;

            if (ghostTimerText != null)
                ghostTimerText.text = Mathf.CeilToInt(powerTimer).ToString();

            if (!recoveringEntered && powerTimer <= recoveringThreshold)
            {
                recoveringEntered = true;
                foreach (var g in ghosts)
                    if (g != null && g.State != GhostState.Dead) g.SetRecovering();
            }

            if (powerTimer <= 0f)
            {
                EndPowerMode();
            }
        }

        // process dead ghost respawn timers
        if (deadGhosts.Count > 0)
        {
            for (int i = deadGhosts.Count - 1; i >= 0; --i)
            {
                deadGhosts[i].timer -= Time.deltaTime;
                if (deadGhosts[i].timer <= 0f)
                {
                    var g = deadGhosts[i].ghost;
                    deadGhosts.RemoveAt(i);
                    // respawn logic: if powerActive still true -> Scared/Recovering else Normal
                    if (g != null)
                    {
                        g.ResetToInitial();
                        if (powerActive)
                        {
                            if (powerTimer <= recoveringThreshold) g.SetRecovering();
                            else g.SetScared();
                        }
                        else
                        {
                            g.SetNormal();
                        }
                    }
                }
            }
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = score.ToString();
    }

    void UpdateLivesUI()
    {
        if (livesText != null) livesText.text = lives.ToString();
    }

    // Public API to start power mode (call from PacStudentController on power pellet)
    public void StartPowerMode()
    {
        powerActive = true;
        powerTimer = powerDuration;
        recoveringEntered = false;

        if (musicSource != null && scaredMusic != null)
        {
            musicSource.clip = scaredMusic;
            musicSource.Play();
        }

        foreach (var g in ghosts)
            if (g != null && g.State != GhostState.Dead) g.SetScared();

        if (ghostTimerText != null) ghostTimerText.gameObject.SetActive(true);
    }

    // Called when a ghost is eaten while in Scared/Recovering
    public void GhostEaten(GhostStateManager ghost)
    {
        if (ghost == null) return;
        // score for eating ghost
        AddScore(300);
        // set ghost dead and start respawn timer
        ghost.SetDead();
        deadGhosts.Add(new DeadGhostRecord { ghost = ghost, timer = 3f });
        musicSource.clip = deathSound;
        musicSource.Play();
        // music handled by power mode state; optional extra sound could be played here
    }

    // Called when PacStudent is killed by a Normal ghost
    public void HandlePacDeath(PacStudentController pac)
    {
        // decrement lives and update UI
        lives = Mathf.Max(0, lives - 1);
        UpdateLivesUI();

        // freeze ghosts so they don't move during death sequence
        foreach (var g in ghosts)
            if (g != null) g.FreezeMovement(true);

        // reset ghosts to their initial pos and Normal state
        foreach (var g in ghosts)
        {
            if (g != null)
            {
                g.ResetToInitial();
                g.SetNormal();
            }
        }
        // ensure power mode ended on player death
        powerActive = false;
        powerTimer = 0f;
        recoveringEntered = false;
        if (musicSource != null && normalMusic != null)
        {
            musicSource.clip = normalMusic;
            musicSource.Play();
        }
        if (ghostTimerText != null) ghostTimerText.gameObject.SetActive(false);
    }

    // Called internally when the timer ends
    void EndPowerMode()
    {
        powerActive = false;
        powerTimer = 0f;
        recoveringEntered = false;

        if (musicSource != null && normalMusic != null)
        {
            musicSource.clip = normalMusic;
            musicSource.Play();
        }

        foreach (var g in ghosts)
            if (g != null && g.State != GhostState.Dead) g.SetNormal();

        if (ghostTimerText != null) ghostTimerText.gameObject.SetActive(false);
    }
}
