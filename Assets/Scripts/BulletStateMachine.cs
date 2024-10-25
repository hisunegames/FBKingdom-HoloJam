using UnityEngine;

public class BulletStateMachine : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        if (animatorStateInfo.IsName("Burst"))
            Destroy(animator.gameObject);
    }
}
