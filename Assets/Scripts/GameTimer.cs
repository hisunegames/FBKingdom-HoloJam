using Febucci.UI.Actions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    private float timer;
    [SerializeField]
    private bool isTiming;
    private int activeScene;

    public const string STAGE_ONE_TIME = "STAGE_ONE_TIME";
    public const string STAGE_TWO_TIME = "STAGE_TWO_TIME";
    public const string BOSS_TIME = "BOSS_TIME";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = 0f;
        isTiming = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        activeScene = SceneManager.GetActiveScene().buildIndex;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTiming)
        {
            timer += Time.deltaTime;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        timer = 0f;
        isTiming = true;
    }

    void OnSceneUnloaded(Scene scene)
    {
        isTiming = false;
        Debug.Log("Time: " + timer);
        switch (activeScene)
        {
            case 0:
                PlayerPrefs.SetFloat(STAGE_ONE_TIME, 0);
                PlayerPrefs.SetFloat(STAGE_TWO_TIME, 0);
                PlayerPrefs.SetFloat(BOSS_TIME, 0);
                break;
            case 2:
                PlayerPrefs.SetFloat(STAGE_ONE_TIME, timer);
                break;
            case 3:
                PlayerPrefs.SetFloat(STAGE_TWO_TIME, timer);
                break;
            case 4:
                PlayerPrefs.SetFloat(BOSS_TIME, timer);
                break;
            default:
                break;
        }

        PlayerPrefs.Save();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

}
