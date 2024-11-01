using System.Collections;
using TMPro;
using UnityEngine;

public class DisplayScores : MonoBehaviour
{
    [SerializeField]
    private float stageOneTime;
    [SerializeField]
    private float stageTwoTime;
    [SerializeField]
    private float bossTime;
    [SerializeField]
    private float mascotsSaved;

    [SerializeField]
    private TextMeshProUGUI stageOneTimeText;
    [SerializeField]
    private TextMeshProUGUI stageTwoTimeText;
    [SerializeField]
    private TextMeshProUGUI bossTimeText;
    [SerializeField]
    private TextMeshProUGUI mascotsSavedText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(UpdateScores());
    }
    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator UpdateScores()
    {
        stageOneTime = PlayerPrefs.GetFloat("STAGE_ONE_TIME", 0);
        stageTwoTime = PlayerPrefs.GetFloat("STAGE_TWO_TIME", 0);
        bossTime = PlayerPrefs.GetFloat("BOSS_TIME", 0);
        mascotsSaved = PlayerPrefs.GetFloat("buff", 0);
        yield return new WaitForSeconds(1f);
        stageOneTimeText.text = $"{{vertexp}}Stage 1: {stageOneTime:F2}s{{/vertexp}}";
        yield return new WaitForSeconds(1f);
        stageTwoTimeText.text = $"{{vertexp}}Stage 2: {stageTwoTime:F2}s{{/vertexp}}";
        yield return new WaitForSeconds(1f);
        bossTimeText.text = $"{{vertexp}}Boss: {bossTime:F2}s{{/vertexp}}";
        yield return new WaitForSeconds(1f);
        mascotsSavedText.text = $"{{vertexp}}Mascots Saved: {mascotsSaved}{{/vertexp}}";
    }
}
