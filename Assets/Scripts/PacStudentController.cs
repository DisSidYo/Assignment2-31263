using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PacStudentController : MonoBehaviour
{
    public Animator moveAnimator;
    public Tilemap tileMap;
    public ParticleSystem footstepParticles;
    public GameObject pacStudentPrefab;

    bool isDestroyed = true;


    public ParticleSystem wallHitParticles;
    int x;
    int y;
    Vector3 newPosition;

    private TweenerNormal tweener;
    private float moveDistance = 1.5f;
    private float tweenDuration = 0.5f;

    private Vector3 currentDirection = Vector3.zero;
    private Vector3 lastInput = Vector3.zero;
    private Vector3 currentInput = Vector3.zero;

    private Vector2Int gridPos;
    public AudioSource footstepAudioSource;
    public AudioSource backgroundAudioSource;

    private float lastPlayedTime = 0f;
    private float footstepDelay = 0.7f; // seconds between sounds
    public AudioClip footstep;

    public AudioClip wallHit;

    public AudioClip pelletCollect;
    int check = 0;
    int TotalPellets = 0;

    private float wallHitCooldown = 2f;
    private float lastWallHitTime = 0f;

    Vector3 startPosition;
    public ParticleSystem deathParticles;
    public string deathAnimationState = "Pac_Stu_Death";
    private bool isAlive = true;

    private float deathAnimationDuration = 3.0f; // Adjust this to match your death animation length


    private int[,] levelMap = new int[,]

{
     {1,2,2,2,2,2,2,2,2,2,2,2,2,7,7,2,2,2,2,2,2,2,2,2,2,2,2,1},
    {2,5,5,5,5,5,5,5,5,5,5,5,5,4,4,5,5,5,5,5,5,5,5,5,5,5,5,2},
    {2,5,3,4,4,3,5,3,4,4,4,3,5,4,4,5,3,4,4,4,3,5,3,4,4,3,5,2},
    {2,6,4,0,0,4,5,4,0,0,0,4,5,4,4,5,4,0,0,0,4,5,4,0,0,4,6,2},
    {2,5,3,4,4,3,5,3,4,4,4,3,5,3,3,5,3,4,4,4,3,5,3,4,4,3,5,2},
    {2,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,2},
    {2,5,3,4,4,3,5,3,3,5,3,4,4,4,4,4,4,3,5,3,3,5,3,4,4,3,5,2},
    {2,5,3,4,4,3,5,4,4,5,3,4,4,3,3,4,4,3,5,4,4,5,3,4,4,3,5,2},
    {2,5,5,5,5,5,5,4,4,5,5,5,5,4,4,5,5,5,5,4,4,5,5,5,5,5,5,2},
    {1,2,2,2,2,1,5,4,3,4,4,3,0,4,4,0,3,4,4,3,4,5,1,2,2,2,2,1},
    {0,0,0,0,0,2,5,4,3,4,4,3,0,3,3,0,3,4,4,3,4,5,2,0,0,0,0,0},
    {0,0,0,0,0,2,5,4,4,0,0,0,0,0,0,0,0,0,0,4,4,5,2,0,0,0,0,0},
    {0,0,0,0,0,2,5,4,4,0,3,4,4,8,8,4,4,3,0,4,4,5,2,0,0,0,0,0},
    {2,2,2,2,2,1,5,3,3,0,4,0,0,0,0,0,0,4,0,3,3,5,1,2,2,2,2,2},
    {0,0,0,0,0,0,5,0,0,0,4,0,0,0,0,0,0,4,0,0,0,5,0,0,0,0,0,0},
    {2,2,2,2,2,1,5,3,3,0,4,0,0,0,0,0,0,4,0,3,3,5,1,2,2,2,2,2},
    {0,0,0,0,0,2,5,4,4,0,3,4,4,8,8,4,4,3,0,4,4,5,2,0,0,0,0,0},
    {0,0,0,0,0,2,5,4,4,0,0,0,0,0,0,0,0,0,0,4,4,5,2,0,0,0,0,0},
    {0,0,0,0,0,2,5,4,3,4,4,3,0,3,3,0,3,4,4,3,4,5,2,0,0,0,0,0},
    {1,2,2,2,2,1,5,4,3,4,4,3,0,4,4,0,3,4,4,3,4,5,1,2,2,2,2,1},
    {2,5,5,5,5,5,5,4,4,5,5,5,5,4,4,5,5,5,5,4,4,5,5,5,5,5,5,2},
    {2,5,3,4,4,3,5,4,4,5,3,4,4,3,3,4,4,3,5,4,4,5,3,4,4,3,5,2},
    {2,5,3,4,4,3,5,3,3,5,3,4,4,4,4,4,4,3,5,3,3,5,3,4,4,3,5,2},
    {2,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,2},
    {2,5,3,4,4,3,5,3,4,4,4,3,5,3,3,5,3,4,4,4,3,5,3,4,4,3,5,2},
    {2,6,4,0,0,4,5,4,0,0,0,4,5,4,4,5,4,0,0,0,4,5,4,0,0,4,6,2},
    {2,5,3,4,4,3,5,3,4,4,4,3,5,4,4,5,3,4,4,4,3,5,3,4,4,3,5,2},
    {2,5,5,5,5,5,5,5,5,5,5,5,5,4,4,5,5,5,5,5,5,5,5,5,5,5,5,2},
    {1,2,2,2,2,2,2,2,2,2,2,2,2,7,7,2,2,2,2,2,2,2,2,2,2,2,2,1},
};


    private SpriteRenderer spriteRenderer;
    private Collider2D pacCollider;

    void Start()
    {
        tweener = GetComponent<TweenerNormal>();
        gridPos = new Vector2Int(1, 1);
        // wait for player input before starting
        currentInput = Vector3.zero;
        lastInput = Vector3.zero;
        moveAnimator.Play("Idle_Right"); // still show idle visually
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        pacCollider = GetComponent<Collider2D>();
        totalCalc();    
    }

    void Update()
    {
        

        if (!isAlive)
        {
            return; // Skip movement while dead
        }

        GetMovementInput();

        if (!tweener.isTweening())
        {
            CollectPellet();
            ContinueMovement();
            wallHitAudio();
        }
        FootstepAudio();
    }

    void GetMovementInput()
    {
        Vector3 input = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) input = Vector3.up;
        else if (Input.GetKey(KeyCode.S)) input = Vector3.down;
        else if (Input.GetKey(KeyCode.A)) input = Vector3.left;
        else if (Input.GetKey(KeyCode.D)) input = Vector3.right;

        if (input != Vector3.zero)
        {
            lastInput = input; // store buffered input
        }
    }

    void ContinueMovement()
    {
        // Try current input first (feels more responsive)
        if (CanMove(lastInput))
        {
            currentInput = lastInput;
            StartNewTween(currentInput);
        }
        else if (CanMove(currentInput))
        {
            StartNewTween(currentInput);
        }
        else
        {
            moveAnimator.Play("Idle_Right");
        }
        
    }

    bool CanMove(Vector3 dir)
    {
        // don't consider zero input as a valid move
        if (dir == Vector3.zero) return false;
        Vector2Int nextGrid = gridPos + new Vector2Int((int)dir.x, -(int)dir.y);

        // Wrap around horizontally at tunnel
        if (gridPos.y == 14)
        {
            if (gridPos.x == 0 && dir == Vector3.left)
                nextGrid = new Vector2Int(27, gridPos.y);
            else if (gridPos.x == 27 && dir == Vector3.right)
                nextGrid = new Vector2Int(0, gridPos.y);
        }

        if (nextGrid.y < 0 || nextGrid.y >= levelMap.GetLength(0) ||
            nextGrid.x < 0 || nextGrid.x >= levelMap.GetLength(1))
            return false;

        int tile = levelMap[nextGrid.y, nextGrid.x];
        return tile == 0 || tile == 5 || tile == 6;
    }
    void totalCalc()
    {
        for(int i=0;i<levelMap.GetLength(0);i++)
        {
            for(int j=0;j<levelMap.GetLength(1);j++)    
            {
                if(levelMap[i,j]==5 || levelMap[i,j]==6)
                {
                    TotalPellets++;
                }
            }
        }
        if(LevelManager.Instance!=null)
        {
            LevelManager.Instance.TotalPellet(TotalPellets);
        }
    }

    void StartNewTween(Vector3 dir)
    {
        currentDirection = dir;
        check = 0;

        // Check if we're entering tunnel
        if (gridPos.y == 14)
        {
            if (gridPos.x == 0 && dir == Vector3.left)
            {
                gridPos = new Vector2Int(27, gridPos.y);
                transform.position = new Vector3(transform.position.x + moveDistance * 27, transform.position.y, 0);
            }
            else if (gridPos.x == 27 && dir == Vector3.right)
            {
                gridPos = new Vector2Int(0, gridPos.y);
                transform.position = new Vector3(transform.position.x - moveDistance * 27, transform.position.y, 0);
            }
        }

        // Update position & start tween normally
        gridPos += new Vector2Int((int)dir.x, -(int)dir.y);
        Vector3 targetPos = transform.position + dir * moveDistance;
        tweener.AddTween(transform, transform.position, targetPos, tweenDuration);

        // Animation
        if (dir == Vector3.up) moveAnimator.Play("Pac_Stu_Up");
        else if (dir == Vector3.down) moveAnimator.Play("Pac_Stu_Down");
        else if (dir == Vector3.left) moveAnimator.Play("Pac_Stu_Left");
        else if (dir == Vector3.right) moveAnimator.Play("Pac_Stu_Right");
    }

    void FootstepAudio()
    {
        if (tweener.isTweening())
        {
            if (!footstepAudioSource.isPlaying)
            {
                if (Time.time - lastPlayedTime >= footstepDelay)
                {
                    footstepAudioSource.PlayOneShot(footstep);
                    lastPlayedTime = Time.time;
                    footstepParticles.Play();
                }
            }
            backgroundAudioSource.volume = 0.5f;
        }
        else
        {
            footstepAudioSource.Stop();
            backgroundAudioSource.volume = 1.0f;
            footstepParticles.Pause();
        }
    }
    void wallHitAudio()
    {
        // guard: ensure audio components assigned
        if (footstepAudioSource == null || wallHit == null) return;

        // If player has a buffered input and that move is blocked, play wall hit once (with cooldown)
        if (lastInput != Vector3.zero && !CanMove(lastInput))
        {
            if (check == 0)
            {
                // play sound (respect cooldown)
                if (Time.time - lastWallHitTime >= wallHitCooldown)
                {
                    footstepAudioSource.PlayOneShot(wallHit);
                    lastWallHitTime = Time.time;
                }

                // play particle effect at side based on movement direction
                PlayWallHitEffect();

                check = 1;
            }
        }
        // else
        // {
        //     // reset so next blocked attempt can play again
        //     check = 0;
        // }
    }

    // Play small dust/impact particle slightly offset from PacStudent in the movement direction.
    void PlayWallHitEffect()
    {
        if (wallHitParticles == null) return;

        // prefer currentDirection; fall back to lastInput if zero
        Vector3 dir = currentDirection;
        if (dir == Vector3.zero) dir = lastInput;

        // choose a small offset relative to PacStudent's transform
        float ox = 2f;
        float oy = 2f;
        Vector3 offset = Vector3.zero;

        if (dir == Vector3.left) offset = new Vector3(-ox, 0f, 0f);
        else if (dir == Vector3.right) offset = new Vector3(ox, 0f, 0f);
        else if (dir == Vector3.up) offset = new Vector3(0f, oy, 0f);
        else if (dir == Vector3.down) offset = new Vector3(0f, -oy, 0f);
        else offset = new Vector3(0f, 0f, 0f);

        wallHitParticles.transform.position = transform.position + offset;
        if(!wallHitParticles.isPlaying)
        wallHitParticles.Play();
    }

    void CollectPellet()
    {
        // Guard clause - check required components
        if (tileMap == null)
        {
            Debug.LogError("PacStudentController: tileMap not assigned!");
            return;
        }

        Vector3Int tilePosition = new Vector3Int(gridPos.x - 3, -gridPos.y + 3, 0);

        // Bounds check for levelMap array
        if (gridPos.y < 0 || gridPos.y >= levelMap.GetLength(0) ||
            gridPos.x < 0 || gridPos.x >= levelMap.GetLength(1))
        {
            return;
        }

        int currentTile = levelMap[gridPos.y, gridPos.x];
        if (currentTile == 5 || currentTile == 6)
        {
            // Destroy the tile in the tilemap
            tileMap.SetTile(tilePosition, null);

            // Update the levelMap
            levelMap[gridPos.y, gridPos.x] = 0;

            // Add score based on pellet type (with null check for LevelManager)
            if (LevelManager.Instance != null)
            {
                if (currentTile == 5) // normal pellet
                {
                    LevelManager.Instance.AddScore(10);
                    LevelManager.Instance.AddPelletEaten(1);
                }
                else if (currentTile == 6) // power pellet
                {
                    LevelManager.Instance.AddScore(50);
                    LevelManager.Instance.AddPelletEaten(1);
                    LevelManager.Instance.StartPowerMode();
                }
            }
            else
            {
                Debug.LogWarning("PacStudentController: LevelManager.Instance is null!");
            }

            // Play pellet collection sound (with null checks)
            if (footstepAudioSource != null && pelletCollect != null)
            {
                footstepAudioSource.PlayOneShot(pelletCollect);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PowerPellet"))
        {
            Destroy(other.gameObject);
            // handled via CollectPellet or tilemap; optional
        }
        if (other.CompareTag("Cherry"))
        {
            Destroy(other.gameObject);
            LevelManager.Instance.AddScore(100);
        }

        // Ghost collision handling
        if (other.CompareTag("Ghost"))
        {
            GhostStateManager g = other.GetComponent<GhostStateManager>();
            if (g == null) g = other.GetComponentInParent<GhostStateManager>();
            if (g == null) return;

            if (g.State == GhostState.Normal)
            {
                // PacStudent dies
                isAlive = false;
                
                // Disable components instead of destroying
                if (spriteRenderer != null) spriteRenderer.enabled = false;
                if (pacCollider != null) pacCollider.enabled = false;
                
                if (deathParticles != null)
                {
                    deathParticles.transform.position = transform.position;
                    deathParticles.Play();
                }
                if (moveAnimator != null && !string.IsNullOrEmpty(deathAnimationState))
                    moveAnimator.Play("Pac_Stu_Death");

                footstepAudioSource.PlayOneShot(LevelManager.Instance.pacDeathSound);

                if (LevelManager.Instance != null) 
                    LevelManager.Instance.HandlePacDeath(this);

                // Schedule automatic respawn after death animation
                Invoke(nameof(Respawn), deathAnimationDuration);
            }
            else if (g.State == GhostState.Scared || g.State == GhostState.Recovering)
            {
                // ghost dies, award points
                if (LevelManager.Instance != null) LevelManager.Instance.GhostEaten(g);
            }
        }
    }

    // Add this method if not already present
    void Respawn()
    {
        isAlive = true;
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (pacCollider != null) pacCollider.enabled = true;
        transform.position = startPosition;
        transform.localScale = new Vector3(1.7f, 1.7f, 1f);
        gridPos = new Vector2Int(1, 1);
        // clear buffered input so we wait again for player input
        currentInput = Vector3.zero;
        lastInput = Vector3.zero;
        moveAnimator.Play("Idle_Right");

        // Unfreeze ghosts
        if (LevelManager.Instance != null)
        {
            foreach (var g in LevelManager.Instance.ghosts)
                if (g != null) g.FreezeMovement(false);
        }
    }
}
