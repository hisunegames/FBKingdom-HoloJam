using UnityEngine;

public class SukonbuAnimationController : MonoBehaviour
{

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

            PlayerScript.instance.GetRandomBuff();
            hasBuff = false;
        }
        else
        {
            s_animator.Play("Idle");
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
