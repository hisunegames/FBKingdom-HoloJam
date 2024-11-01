using FBK;
using UnityEngine;

public class SukonbuAnimationController : MonoBehaviour
{
    [SerializeField] private UIBuffBubble buffBubblePrefab;
    public Animator s_animator;
    public Animator h_animator;

    [SerializeField]
    private DetectEnemy detectEnemy;
    [SerializeField]
    private DetectPlayerMascot detectPlayerMascot;
    bool enemyInRange = false;

    bool hasBuff = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        detectEnemy.OnEnemyInRange.AddListener(SukobuEnemyState);
        detectPlayerMascot.OnPlayerInRange.AddListener(SukobuPlayerState);
    }

    public void SukobuEnemyState(bool inRange)
    {
        enemyInRange = inRange;

        if (inRange)
        {
            s_animator.Play("Fear");
            h_animator.gameObject.SetActive(true);
        }
        else
        {
            s_animator.Play("Idle");
            h_animator.gameObject.SetActive(false);
        }
    }
    public void SukobuPlayerState(bool inRange)
    {
        if (enemyInRange) return;

        if (inRange)
        {
            s_animator.Play("Banzai");

            if (!hasBuff) return;

            BuffPlayer();
        }
        else
        {
            s_animator.Play("Idle");
        }
    }

    private void BuffPlayer()
    {
        hasBuff = false;

        var content = GameManager.instance.Database.MascotMessage;
        var buffIdx = PlayerScript.instance.GetRandomBuff();
        var messageIdx = Random.Range(0, content.MessageList.Length);

        var bubble = Instantiate(buffBubblePrefab, transform.position, Quaternion.identity);
        bubble.Initialize(content.MessageList[messageIdx], content.BuffList[buffIdx]);
        int currentBuffAmount = PlayerPrefs.GetInt("buff", 0);
        PlayerPrefs.SetInt("buff", currentBuffAmount + 1);
    }
}
