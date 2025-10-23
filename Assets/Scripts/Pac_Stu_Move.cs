using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pac_Stu_Move : MonoBehaviour
{
    private Tweener tweener;
    private Animator animator;
    private AudioSource audioSource;
    private AudioClip moveSound;
    private float moveSpeed = 2.0f;

    public Canvas canvas;

    
    public Vector3[] pathPoints;
    private int currentPathIndex = 0;
    private Vector3 currentStartPos;
    private float soundCooldown;
    private float lastSoundTime;

public RectTransform borderTop;
public RectTransform borderBottom;
public RectTransform borderLeft;
    public RectTransform borderRight;
//    public RectTransform borderTop; // e.g., your top border
    public CanvasScaler canvasScaler;
    public Camera mainCamera;

    void Start()
    {
        tweener = GetComponent<Tweener>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();

        updatePathPoints();


        currentStartPos = pathPoints[0];
        nextOne();
    }



    void Update()
    {
        if (!tweener.isTweening())
        {
            nextOne();

        }
        // if (Time.time - lastSoundTime >= soundCooldown)
        // {
        //     audioSource.Play();
        //     lastSoundTime = Time.time;
        // }
            updatePathPoints();
        }
    public void updatePathPoints()
    {
        RectTransform rect = borderTop.GetComponent<RectTransform>();

Vector3[] corners = new Vector3[4];

        // bottom-left = corners[0]
        // top-left    = corners[1]
        // top-right   = corners[2]
        // bottom-right= corners[3]
        float uiWidth = borderTop.rect.width;
        float uiHeight = borderLeft.rect.height;

        // Get Canvas Scaler info
        Vector2 referenceResolution = canvasScaler.referenceResolution;
        float screenWidth = Screen.width;

        // Scale factor depends on your match setting (Width/Height)
        float scaleFactor;
        if (canvasScaler.screenMatchMode == CanvasScaler.ScreenMatchMode.MatchWidthOrHeight)
        {
            float logWidth = Mathf.Log(screenWidth / referenceResolution.x, 2);
            float logHeight = Mathf.Log(Screen.height / referenceResolution.y, 2);
            float match = canvasScaler.matchWidthOrHeight;
            scaleFactor = Mathf.Pow(2, Mathf.Lerp(logWidth, logHeight, match));
        }
        else
        {
            scaleFactor = screenWidth / referenceResolution.x;
        }

        // Convert UI width (pixels) to canvas-unscaled pixels
        float scaledUIWidth = uiWidth * scaleFactor;
        float scaledUIHeight = uiHeight * scaleFactor;

        Debug.Log($"UI width in pixels: {uiWidth}, scaled width: {scaledUIWidth}");

        // --- OPTIONAL: Convert to world space width ---
        // Only works if Canvas is Screen Space - Camera or World Space
        float zDist = Mathf.Abs(mainCamera.transform.position.z - borderTop.position.z);
        Vector3 left = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, zDist));
        Vector3 right = mainCamera.ScreenToWorldPoint(new Vector3(scaledUIWidth, 0, zDist));
        float worldWidth = Vector3.Distance(left, right);

        float zDistHeight = Mathf.Abs(mainCamera.transform.position.z - borderLeft.position.z);
        Vector3 bottom = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, zDistHeight));
        Vector3 top = mainCamera.ScreenToWorldPoint(new Vector3(0, scaledUIHeight, zDistHeight));
        float worldHeight = Vector3.Distance(bottom, top);

        Debug.Log($"Approx world-space width: {worldWidth}, height: {worldHeight}");

        // Define path points using the borders
        pathPoints = new Vector3[]
{
    // top-left
    new Vector3(-worldWidth/2.0f+1.0f, worldHeight/2.0f, 0.5f),
    // top-right
    new Vector3(worldWidth/2.0f-1.5f,worldHeight/2.0f, 0.5f),
    // bottom-right
    new Vector3(worldWidth/2.0f-1.5f,-worldHeight/2.0f+2.0f, 0.5f),
    // bottom-left
    new Vector3(-worldWidth/2.0f+1.0f,-worldHeight/2.0f+2.0f, 0.5f)
};
    }
    public void nextOne()
    {
        // Vector3 nextPoint = pathPoints[currentPathIndex];
        // float distance = Vector3.Distance(currentStartPos, nextPoint);
        // float duration = Mathf.Max(distance / moveSpeed, 0.01f); // Avoid zero duration

        // tweener.AddTween(transform, currentStartPos, nextPoint, duration);

        // // Directional animation
        // Vector3 dir = (nextPoint - currentStartPos).normalized;
        // if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        //     animator.Play(dir.x > 0 ? "Pac_Stu_Right" : "Pac_Stu_Left");
        // else
        //     animator.Play(dir.y > 0 ? "Pac_Stu_Up" : "Pac_Stu_Down");

        // // Prepare for next tween
        // currentStartPos = nextPoint;
        // currentPathIndex = (currentPathIndex + 1) % pathPoints.Length;
    }
}
