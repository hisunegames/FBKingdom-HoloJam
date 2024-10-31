using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OpenStage(int stageId)
    {
        if (stageId == 1)
        {
            PlayerStatsHandler.ResetPlayerBuff();
        }

        SceneManager.LoadScene(stageId);
    }
}
