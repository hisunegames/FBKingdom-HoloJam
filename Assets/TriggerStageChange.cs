using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerStageChange : MonoBehaviour
{
    public static TriggerStageChange instance;
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
        {
            PlayerScript.instance.m_Controller.InputMovement(new(1, 0));
            CameraManager.instance.DisableCameraFollow();
            if (Mathf.Abs(PlayerScript.instance.gameObject.transform.position.x - offscreenPos.transform.position.x) < .5f)
            {

                SceneManager.LoadScene(nextStageId);
            }
        }
    }
}
