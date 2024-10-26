using NUnit.Framework;
using UnityEngine;

public class HurtStateMachine : StateMachineBehaviour
{
    private int frameCounter;
    private bool isWhite;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        frameCounter = 0;
        isWhite = false;
        PlayerScript.instance.isInvincible = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        frameCounter++;
        if (frameCounter >= 8)
        {
            frameCounter = 0;
            isWhite = !isWhite;
            animator.GetComponent<SpriteRenderer>().color = isWhite ? Color.white : Color.red;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerScript.instance.isInvincible = false;
        animator.GetComponent<SpriteRenderer>().color = Color.white;
    }

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
