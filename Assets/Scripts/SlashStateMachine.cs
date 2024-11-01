using UnityEngine;
using System.Collections;

public class SlashStateMachine : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("Action-Slash-Ground-Start"))
        {
            PlayerScript.instance.isSlashing = true;

            animator.SetInteger("ComboState", 1);

        }

        else if (stateInfo.IsName("Action-Slash-Ground-1"))
        {
            PlayerScript.instance.PlaySoundSlash();
            PlayerScript.instance.transform.GetChild(0).gameObject.SetActive(true);
            if (animator.GetInteger("ComboState") == 3)
            {
                // Define the step size to move the player by one step
                Vector3 moveStep = new Vector3(.05f * PlayerScript.instance.m_Controller.FacingDirection, 0, 0); // Adjust the x value as needed for the step size

                // Apply the translation to the Transform
                animator.transform.Translate(moveStep);


                // Start a coroutine to stop the movement after 0.5 seconds
                // PlayerScript.instance.StartCoroutine(StopMovementAfterDelay(animator.transform, moveStep, 0.5f));
            }
        }

        else if (stateInfo.IsName("Action-Slash-Ground-2"))
        {
            PlayerScript.instance.PlaySoundSlash();
            PlayerScript.instance.transform.GetChild(1).gameObject.SetActive(true);

        }
        else if (stateInfo.IsName("Action-Slash-Aerial"))
        {
            PlayerScript.instance.PlaySoundSlash();
            PlayerScript.instance.transform.GetChild(2).gameObject.SetActive(true);

            animator.ResetTrigger("Slash");

        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("Action-Slash-Ground-Start"))
        {

        }

        else if (stateInfo.IsName("Action-Slash-Ground-1"))
        {

            if (animator.GetInteger("ComboState") == 1)
            {

                if (PlayerScript.instance.isSlashing)
                    animator.SetInteger("ComboState", 2);
                else if (!PlayerScript.instance.isSlashing)
                {
                    animator.SetInteger("ComboState", 0);
                }
            }
            else if (animator.GetInteger("ComboState") == 3)
                if (PlayerScript.instance.isSlashing)
                    animator.SetInteger("ComboState", 2);
                else if (!PlayerScript.instance.isSlashing)
                    animator.SetInteger("ComboState", 0);
        }

        else if (stateInfo.IsName("Action-Slash-Ground-2"))
        {
            if (animator.GetInteger("ComboState") == 2)
            {
                if (PlayerScript.instance.isSlashing)
                    animator.SetInteger("ComboState", 3);
                else if (!PlayerScript.instance.isSlashing)
                    animator.SetInteger("ComboState", 0);
            }
            else if (animator.GetInteger("ComboState") == 4)
                if (PlayerScript.instance.isSlashing)
                    animator.SetInteger("ComboState", 1);
                else if (!PlayerScript.instance.isSlashing)
                    animator.SetInteger("ComboState", 0);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("NoneState"))
        {
            animator.ResetTrigger("Slash");
            animator.SetInteger("ComboState", 0);
            PlayerScript.instance.transform.GetChild(0).gameObject.SetActive(false);
            PlayerScript.instance.transform.GetChild(1).gameObject.SetActive(false);
        }

        else if (stateInfo.IsName("Action-Slash-Ground-Start"))
        {
            animator.ResetTrigger("Slash");
            // PlayerScript.instance.transform.GetChild(0).gameObject.SetActive(false);
        }

        else if (stateInfo.IsName("Action-Slash-Ground-1"))
        {
            PlayerScript.instance.transform.GetChild(0).gameObject.SetActive(false);

        }

        else if (stateInfo.IsName("Action-Slash-Ground-2"))
        {
            PlayerScript.instance.transform.GetChild(1).gameObject.SetActive(false);


        }
        else if (stateInfo.IsName("Action-Slash-Aerial"))
        {
            PlayerScript.instance.transform.GetChild(2).gameObject.SetActive(false);

            // PlayerScript.instance.isSlashing = false;
            animator.ResetTrigger("Slash");
        }
    }

    // Coroutine to stop the movement after a delay
    // private IEnumerator StopMovementAfterDelay(Transform transform, Vector3 moveStep, float delay)
    // {
    //     yield return new WaitForSeconds(delay);
    //     transform.Translate(-moveStep); // Move back to the original position
    // }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
