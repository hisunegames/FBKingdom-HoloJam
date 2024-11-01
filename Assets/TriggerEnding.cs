using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerEnding : MonoBehaviour
{
    public static TriggerEnding instance;
    [SerializeField]
    private int nextStageId;
    [SerializeField]
    private GameObject offscreenPos;
    public bool startSceneChange = false;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    void Start()
    {
        startSceneChange = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerScript.instance.OnFinishStage();
        startSceneChange = true;
    }

    private void Update()
    {
        if (startSceneChange)
            SceneManager.LoadScene(nextStageId);
    }
}
