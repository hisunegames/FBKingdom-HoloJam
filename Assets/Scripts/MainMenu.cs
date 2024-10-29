using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OpenStage(int stageId)
    {
        SceneManager.LoadScene(stageId);
    }
}
