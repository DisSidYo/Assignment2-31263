using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pac_Stu_Move_1 : MonoBehaviour
{
    private Tweener tweener;
    private Animator animator;
    private AudioSource audioSource;
    private AudioClip moveSound;
    private float moveSpeed = 100.0f;

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
        RectTransform parentRect = transform.parent.GetComponent<RectTransform>();
        Vector3[] corners = new Vector3[4];
        parentRect.GetLocalCorners(corners);




        // audioSource.loop = true;
        audioSource.Play();
        float offset = 70f; // Offset from borders

        pathPoints = new Vector3[]
        {
            corners[0] + new Vector3(offset, offset, 0),   // bottom-left
            corners[1] + new Vector3(offset, -offset, 0),  // top-left
            corners[2] + new Vector3(-offset, -offset, 0), // top-right
            corners[3] + new Vector3(-offset, offset, 0)   // bottom-right
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
        //  if (Time.time - lastSoundTime >= soundCooldown)
        //     {
        //         audioSource.Play();
        //         lastSoundTime = Time.time;
        //     }
    }

    public void nextOne()
    {
        Vector3 nextPoint = pathPoints[currentPathIndex];
        float distance = Vector3.Distance(currentStartPos, nextPoint);
        float duration = Mathf.Max(distance / moveSpeed, 0.01f); // Avoid zero duration

        tweener.AddTween(GetComponent<RectTransform>(), currentStartPos, nextPoint, duration);

        // Directional animation
        Vector3 dir = (nextPoint - currentStartPos).normalized;
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            animator.Play(dir.x > 0 ? "Temp_UI" : "Pac_UI_left");
        else
            animator.Play(dir.y > 0 ? "Pac_UI_up" : "Pac_UI_down");

        // Prepare for next tween
        currentStartPos = nextPoint;
        currentPathIndex = (currentPathIndex + 1) % pathPoints.Length;
    }
}