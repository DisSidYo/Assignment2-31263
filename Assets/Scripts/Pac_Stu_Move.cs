using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pac_Stu_Move : MonoBehaviour
{
    private Tweener tweener;
    private Animator animator;
    private AudioSource audioSource;
    private AudioClip moveSound;
    private float moveSpeed = 2.0f;

    public Vector3[] pathPoints;
    private int currentPathIndex = 0;
    private Vector3 currentStartPos;
    private float soundCooldown;
    private float lastSoundTime;

    void Start()
    {
        tweener = GetComponent<Tweener>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        soundCooldown = 0.2f; // Minimum time between sounds
        lastSoundTime = 0f;

        // audioSource.loop = true;
        audioSource.Play();

        pathPoints = new Vector3[]
        {
            new Vector3(0.0f, 0.0f, 0.0f),
            new Vector3(5.0f, 0.0f, 0.0f),
            new Vector3(5.0f, -4.0f, 0.0f),
            new Vector3(0.0f, -4.0f, 0.0f),
        };

        currentStartPos = pathPoints[0];
        nextOne();
    }

    void Update()
    {
        if (!tweener.isTweening())
        {
            nextOne();

        }
         if (Time.time - lastSoundTime >= soundCooldown)
            {
                audioSource.Play();
                lastSoundTime = Time.time;
            }
    }

    public void nextOne()
    {
        Vector3 nextPoint = pathPoints[currentPathIndex];
        float distance = Vector3.Distance(currentStartPos, nextPoint);
        float duration = Mathf.Max(distance / moveSpeed, 0.01f); // Avoid zero duration

        tweener.AddTween(transform, currentStartPos, nextPoint, duration);

        // Directional animation
        Vector3 dir = (nextPoint - currentStartPos).normalized;
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            animator.Play(dir.x > 0 ? "Pac_Stu_Right" : "Pac_Stu_Left");
        else
            animator.Play(dir.y > 0 ? "Pac_Stu_Up" : "Pac_Stu_Down");

        // Prepare for next tween
        currentStartPos = nextPoint;
        currentPathIndex = (currentPathIndex + 1) % pathPoints.Length;
    }
}
