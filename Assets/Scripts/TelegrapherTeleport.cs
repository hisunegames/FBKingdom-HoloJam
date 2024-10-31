using UnityEngine;

public class TelegrapherTeleport : MonoBehaviour
{
    public Animator anim;

    private void OnEnable()
    {
        anim.Play("Telegrapher", 0, 0);
    }

    public void Despawn()
    {
        gameObject.SetActive(false);
    }
}
