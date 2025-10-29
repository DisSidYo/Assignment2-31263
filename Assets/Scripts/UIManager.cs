using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private RectTransform loadingPanel;
    public static UIManager Instance { get; private set; }

    // [SerializeField] private TextMeshProUGUI timerText;

    public TextMeshProUGUI highScoreText;

    [SerializeField] private TextMeshProUGUI LoadText;
    private const string HighScoreKey = "HighScore";
    private const string BestTimeKey = "BestTime";
    GameObject innerBarObj;
    Image innerBar;
    GameObject PlayerObj;
    Transform playerTransform;
    GameObject exitButtonObj;
private Transform camTransform;
    Camera camera;
    private Tweener tweener;
    private int countdownValue = 5;
void Start()
    {
    //      int highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
    // float bestTime = PlayerPrefs.GetFloat(BestTimeKey, 0f);

    // highScoreText.text = $"High Score: {highScore}";

    // if (bestTime > 0f)
    // {
    //     int minutes = (int)(bestTime / 60);
    //     int seconds = (int)(bestTime % 60);
    //     int milliseconds = (int)((bestTime * 1000) % 1000);

    //     highScoreText.text += $"  Time: {minutes:00}:{seconds:00}:{milliseconds:000}";
    // }
        // if (loadingPanel)
        // {
        //     Debug.Log("Setting loading panel size");
        //     loadingPanel.sizeDelta = new Vector2(Screen.width, Screen.height);
        // }
        // camTransform = Camera.main.transform;
        // if (loadingPanel)
        // {
        //     loadingPanel.sizeDelta = new Vector2(Screen.width, Screen.height);
        // }
        // Invoke(nameof(HideLoadingScreen), 1f);
        UpdateHighScoreDisplay();
        
        if (loadingPanel)
        {
            loadingPanel.sizeDelta = new Vector2(Screen.width, Screen.height);
            //loadingPanel.gameObject.SetActive(false); // Hide panel at start
        }
    
        Invoke(nameof(HideLoadingScreen), 1.0f);
        
}



   private void Awake()
{
    // Destroy duplicates if any
    
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        tweener = GetComponent<Tweener>();
}


    // Update is called once per frame
    void Update()
    {


       


    }
     private void HideLoadingScreen()
    {
        if (loadingPanel && tweener)
        {
            //loadingPanel.gameObject.SetActive(false);
            LoadText.text = "";
            Vector3 startPos = Vector2.zero;
            Vector3 endPos = new Vector2(0, -Screen.height);
            tweener.AddTween(loadingPanel, startPos, endPos, 0.2f);
        }
    }
    private void ShowLoadingScreen()
    {
        if (loadingPanel && tweener)
        {
            //loadingPanel.gameObject.SetActive(true);
            // LoadText.text = "Loading...";
            //    yield return new WaitForSeconds(0.15f);

            // countdown 3..1
            // for (int i = 3; i >= 1; i--)
            // {
            //     LoadText.text = i.ToString();
            //     yield return new WaitForSeconds(1f);
            // }

            // // GO!
            // LoadText.text = "GO!";
            // yield return new WaitForSeconds(1f);
            ShowNextCountdownNumber();
           Vector3 startPos = new Vector2(0, -Screen.height);
            Vector3 endPos = Vector2.zero;
            tweener.AddTween(loadingPanel, startPos, endPos, 0.2f);
        }
    }
    private void AnimateLoadingPanel()
    {
         
    }
    private void ShowNextCountdownNumber()
{
    if (countdownValue > 1)
        {
            if (countdownValue == 2)
            {
                // Reset for next time
                LoadText.text = "GO!";
            }
            else
            {
                LoadText.text = (countdownValue - 2).ToString();
            }
        
        countdownValue--;
        
        // call again after 1 second
        Invoke(nameof(ShowNextCountdownNumber), 1f);
    }
    else
    {
        // show GO! then animate
        
        // Invoke(nameof(AnimateLoadingPanel), 1f);
    }
}
    
  
    private void AsyncLoadFirstLevel()
    {

        SceneManager.LoadSceneAsync(1);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
      public void LoadFirstLevel()
    {

        ShowLoadingScreen();
        Invoke(nameof(AsyncLoadFirstLevel), 0.1f);
        

    }
//     public void QuitGame()
//     {
// #if UNITY_EDITOR
//         // This code only runs in the Unity Editor
//         UnityEditor.EditorApplication.isPlaying = false;
// #else
//         // This code runs in builds (standalone, mobile, etc.)
//         Application.Quit();
// #endif
//     }
//         private void OnEnable()
//     {
//         SceneManager.sceneLoaded += OnSceneLoaded;
//     }

    public void GoToStartScene()
{
    // Optionally show a loading screen or fadeout later if you want
    SceneManager.LoadScene(0);
}

    void OnEnable()
{
    SceneManager.sceneLoaded += OnSceneLoaded;
}

void OnDisable()
{
    SceneManager.sceneLoaded -= OnSceneLoaded;
}

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex != 0)
        {
            camera = Camera.main;
            // Assuming you have a Button component on the same GameObject as this script
            exitButtonObj = GameObject.FindWithTag("Exit");
            if (exitButtonObj != null)
            {
                Button exitButton = exitButtonObj.GetComponent<Button>();
                if (exitButton != null)
                {
                    exitButton.onClick.AddListener(GoToStartScene);
                }

             }
        //     innerBarObj = GameObject.FindWithTag("PlayerHealthBar");
        //     if (innerBarObj != null)
        //         innerBar = innerBarObj.GetComponent<Image>();

        //     PlayerObj = GameObject.FindWithTag("Player");
        //     if (PlayerObj != null)
        //         playerTransform = PlayerObj.transform;

        //     // Set initial color and full health
        //     // if (innerBar != null)
        //     // {
        //     //     innerBar.fillAmount = 1f;
        //     //     innerBar.color = Color.green;
            }
    else if (scene.name == "StartScene")
    {
        // Find by tag OR name — whichever fits your setup
        highScoreText = GameObject.FindWithTag("HighScoreText")?.GetComponent<TextMeshProUGUI>();

        // If tag is missing, fallback to name
        if (highScoreText == null)
            highScoreText = GameObject.Find("HighScoreText")?.GetComponent<TextMeshProUGUI>();

        // Now update the display
        UpdateHighScoreDisplay();
    }



        Invoke(nameof(HideLoadingScreen), 4.0f);
        
       
    }

    // private void RotateCamera()
    // {
    //     if (Input.GetKey(KeyCode.J) && camera)
    //     {
    //         camera.transform.RotateAround(Vector3.zero, Vector3.up, 90.0f * Time.deltaTime);
    //     }
    //     if (Input.GetKey(KeyCode.L) && camera)
    //     {
    //         camera.transform.RotateAround(Vector3.zero, Vector3.up, -90.0f * Time.deltaTime);
    //         camera.transform.LookAt(Vector3.zero, Vector3.up);
    //     }
    // }
    // private void UpdateHealthBar()
    // {
    //     if (innerBarObj && camera)
    //     {
    //         Vector3 directionToCamera = innerBarObj.transform.parent.position - camera.transform.forward;
    //         innerBarObj.transform.parent.LookAt(directionToCamera, Vector3.up);
    //         // Debug.Log("Rotating Health Bar");
    //     }
    // }
    // private void LateUpdate()
    // {
    //     RotateCamera();
    //     UpdateHealthBar();
    // }
    public void ShowGameOverScreen()
    {
        if (loadingPanel == null || LoadText == null) return;

        // Make sure it covers the screen
        loadingPanel.sizeDelta = new Vector2(Screen.width, Screen.height);
        // loadingPanel.gameObject.SetActive(true);

        // Set text
        LoadText.text = "GAME OVER";

        // // Make it semi-opaque (blocking image visible)
        // Image panelImage = loadingPanel.GetComponent<Image>();
        // if (panelImage != null)
        // {
        //     Color c = panelImage.color;
        //     c.a = 0.75f; // adjust transparency (0 = invisible, 1 = fully opaque)
        //     panelImage.color = c;
        // }

        // Optional: tween in the panel smoothly
        if (tweener)
        {
            Vector3 startPos = new Vector2(0, -Screen.height);
            Vector3 endPos = Vector2.zero;
            tweener.AddTween(loadingPanel, startPos, endPos, 0.4f);
        }
    }
    public void HideGameOverScreen()
    {
        if (loadingPanel == null || LoadText == null) return;

        // Optional: tween out the panel smoothly
        if (tweener)
        {
            Vector3 startPos = Vector2.zero;
            Vector3 endPos = new Vector2(0, -Screen.height);
            tweener.AddTween(loadingPanel, startPos, endPos, 0.4f);
        }
    }
public void UpdateHighScoreDisplay()
{
    if (!highScoreText) return; // <— prevent null reference

    int highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
    float bestTime = PlayerPrefs.GetFloat(BestTimeKey, 0f);

    if (highScore == 0 && bestTime == 0f)
    {
        highScoreText.text = "High Score: --  Best Time: --:--:---";
        return;
    }

    int minutes = (int)(bestTime / 60);
    int seconds = (int)(bestTime % 60);
    int milliseconds = (int)((bestTime * 100) % 100);

    string formattedTime = $"{minutes:00}:{seconds:00}:{milliseconds:000}";
    highScoreText.text = $" Play Level 1  High Score: {highScore:D6}   Best Time: {formattedTime}";
}



    
}

