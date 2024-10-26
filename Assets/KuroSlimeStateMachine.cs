using UnityEngine;

public class KuroSlimeStateMachine : StateMachineBehaviour
{
    private EnemyAI enemyAI;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemyAI == null)
        {
            enemyAI = animator.GetComponentInParent<EnemyAI>();
        }

        if (stateInfo.IsName("Attack"))
        {
            enemyAI.isAttacking = true;
        }
        else if (stateInfo.IsName("Move"))
        {
            // enemyAI.AdjustSpeed(1);
        }
        else if (stateInfo.IsName("Move-Start"))
        {
            // enemyAI.AdjustSpeed(0.5f); // Slow down
        }
        else if (stateInfo.IsName("Idle"))
        {
            // enemyAI.AdjustSpeed(0); // Stop moving
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("Attack"))
        {
            enemyAI.isAttacking = false;
        }
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
