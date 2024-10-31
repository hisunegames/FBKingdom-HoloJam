using UnityEngine;
namespace DYP
{
    /// <summary>
    /// The player input driver for controller BasicMovementController2D
    /// </summary>
    public class CustomMovementDriver : MonoBehaviour
    {
        [SerializeField]
        private CustomMovementController2D m_Controller;

        // Update is called once per frame
        void Update()
        {
            //m_Controller.ClearInputBuffer(Time.deltaTime);

            var horizontal = Input.GetAxisRaw("Horizontal");
            var vertical = Input.GetAxisRaw("Vertical");

            var jump = Input.GetButtonDown("Jump");
            var dash = Input.GetButtonDown("Fire3");

            var holdingJump = Input.GetButton("Jump");
            var holdingDash = Input.GetButton("Fire3");

            var releaseJump = Input.GetButtonUp("Jump");

            if (PlayerScript.instance.isSlashing && PlayerScript.instance.m_Animator.GetBool("isGrounded"))
            {
                horizontal = 0;
                vertical = 0;
            }
            m_Controller.InputMovement(new Vector2(horizontal, vertical));

            m_Controller.PressJump(jump);

            if (PlayerScript.instance.CanDash())
            {
                m_Controller.PressDash(dash);
            }

            m_Controller.HoldJump(holdingJump);

            m_Controller.HoldDash(holdingDash);

            m_Controller.ReleaseJump(releaseJump);
        }
    }
}

