using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    public Animator moveAnimator;
    public ParticleSystem footstepParticles;

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
    int check = 0;

    private float wallHitCooldown = 0.5f;
    private float lastWallHitTime = 0f;

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


    void Start()
    {
        tweener = GetComponent<TweenerNormal>();
        gridPos = new Vector2Int(1, 1);
        currentInput = Vector3.right;
        lastInput = Vector3.right;
        moveAnimator.Play("Idle_Right");
    }

    void Update()
    {
        GetMovementInput();


        if (!tweener.isTweening())
        {
            ContinueMovement();
        }
        FootstepAudio();
        wallHitAudio();
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
            if (check==0  ){
                footstepAudioSource.PlayOneShot(wallHit);
                lastWallHitTime = Time.time;
                check = 1;
            }
        }
    }
}
