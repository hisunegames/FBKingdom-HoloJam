using DYP;
using FBK;
using UnityEngine;

public class FubukiAnimationController : MonoBehaviour
{

    private SpriteRenderer m_SpriteRenderer;
    private CustomMovementController2D m_Controller;
    private Vector3 m_StartScale;

    [SerializeField]
    private Animator m_Animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        m_StartScale = gameObject.transform.localScale;
        m_Controller = GetComponent<CustomMovementController2D>();

        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        m_Controller.OnFacingFlip += onFacingFlip;
        m_Controller.OnDash += onDash;
        m_Controller.OnDashStay += onDashStay;
        m_Controller.OnDashEnd += onDashEnd;
        m_Controller.OnWallJump += onWallJump;

        m_Controller.OnWallSliding += onWallSliding;
        m_Controller.OnWallSlidingEnd += onWallSlidingEnd;
        m_Controller.OnJump += onJump;
        m_Controller.OnAction += OnAction;
    }
    // Update is called once per frame
    private void Update()
    {
        if (Mathf.Abs(m_Controller.InputVelocity.x) > 0.0f)
        {
            m_Animator.SetFloat("SpeedX", m_Controller.InputVelocity.x);
            // m_Animator.SetInteger("ActionState", 1);
            m_Animator.SetBool("isRunning", true);
        }
        else
        {
            m_Animator.SetBool("isRunning", false);
            // m_Animator.SetInteger("ActionState", 0);
        }

        m_Animator.SetFloat("SpeedY", m_Controller.InputVelocity.y);

        m_Animator.SetBool("isGrounded", m_Controller.IsOnGround());

        // m_Animator.SetBool("IsOnWall", m_Controller.IsState(MotorState.WallSliding));
    }

    private void onFacingFlip(int facing)
    {
        gameObject.transform.localScale = new Vector3(facing * m_StartScale.x, m_StartScale.y, m_StartScale.z);
    }

    private void onJump()
    {
        m_Animator.SetTrigger("Jump");
        BurstParticles();
    }

    public void BurstParticles()
    {
        ParticleSystem particleSystem = GetComponentInChildren<ParticleSystem>();
        if (particleSystem != null)
        {
            particleSystem.Emit(100); // Emit 30 particles
        }
    }

    private void onWallJump(Vector2 vec)
    {
        // TODO
    }

    private void onWallSliding(int wallDir)
    {
        //m_SpriteRenderer.flipX = true;
    }

    private void onWallSlidingEnd()
    {
        //m_SpriteRenderer.flipX = false;
    }

    private void onDash(int dashDir)
    {
        // m_Animator.SetBool("IsDashing", true);
        // m_Animator.Play("StartDash");
    }

    private void onDashStay(float progress)
    {
        // m_Animator.SetFloat("DashProgress", progress);
    }

    private void onDashEnd()
    {
        // m_Animator.SetBool("IsDashing", false);
    }

    private void OnAction(int actionCode)
    {
        // Handle the action based on the actionCode
    }
}
