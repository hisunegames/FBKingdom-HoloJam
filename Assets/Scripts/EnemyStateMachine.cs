using UnityEngine;

public class EnemyStateMachine : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if (animatorStateInfo.IsName("Death"))
            Destroy(animator.gameObject);
    }
}
