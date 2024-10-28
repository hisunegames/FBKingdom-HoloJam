using UnityEngine;

public class EnemyStateMachine : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("Death"))
        {
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if (animatorStateInfo.IsName("Death"))
            Destroy(animator.transform.parent.gameObject);
    }
}
