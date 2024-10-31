using TMPro;
using UnityEngine;

public class UIGameOver : MonoBehaviour
{
    [SerializeField] private TMP_Text progress;
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject progressTips;

    private void Start()
    {
        content.SetActive(false);

        PlayerScript.instance.EventGameOver += ShowGameOver;
    }

    private void ShowGameOver()
    {
        content.SetActive(true);

        var buff = PlayerStatsHandler.GetBuffCount();
        progress.text = buff.ToString();
        progressTips.SetActive(buff > 0);
    }

    public void ResetProgress()
    {
        PlayerStatsHandler.ResetPlayerBuff();
    }
}
