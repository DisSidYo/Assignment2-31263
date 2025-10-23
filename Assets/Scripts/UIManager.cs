using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private RectTransform loadingPanel;
    GameObject innerBarObj;
    Image innerBar;
    GameObject PlayerObj;
    Transform playerTransform;
    GameObject quitButtonObj;
private Transform camTransform;
    Camera camera;
    private Tweener tweener;
void Start()
{
        // camTransform = Camera.main.transform;
        // if (loadingPanel)
        // {
        //     loadingPanel.sizeDelta = new Vector2(Screen.width, Screen.height);
        // }
        // Invoke(nameof(HideLoadingScreen), 1.0f);
        
}



   private void Awake()
{
    // Destroy duplicates if any
    

        DontDestroyOnLoad(gameObject);
        tweener = GetComponent<Tweener>();
}


    // Update is called once per frame
    void Update()
    {


       


    }
    
    
    public void LoadFirstLevel()
    {
        
        Invoke(nameof(AsyncLoadFirstLevel), 1.0f);

    }
    private void AsyncLoadFirstLevel()
    {

        SceneManager.LoadSceneAsync(1);
        SceneManager.sceneLoaded += OnSceneLoaded;
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

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // if (scene.buildIndex == 0)
        // {
        //     camera = Camera.main;
        //     // Assuming you have a Button component on the same GameObject as this script
        //     quitButtonObj = GameObject.FindWithTag("QuitButton");
        //     if (quitButtonObj != null)
        //     {
        //         Button quitButton = quitButtonObj.GetComponent<Button>();
        //         if (quitButton != null)
        //         {
        //             quitButton.onClick.AddListener(QuitGame);
        //         }

        //     }
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
        //     // }




        //     Invoke(nameof(HideLoadingScreen), 1.0f);
            SceneManager.sceneLoaded -= OnSceneLoaded;
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
    
}

