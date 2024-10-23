using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class Cooldown : MonoBehaviour
{
    public Image skillImage;
    public float cooldown;
    bool isCooldown = false;
    public KeyCode key;

    void Start()
    {
        skillImage.fillAmount = 0;
    }

    void Update()
    {
        Skill();
    }
    void Skill()
    {
        if (Input.GetKey(key) && isCooldown == false)
        {
            print("Used Skill");
            isCooldown = true;
            skillImage.fillAmount = 1;
        }

        if (isCooldown) 
        {
            skillImage.fillAmount -= 1 / cooldown * Time.deltaTime;

            if (skillImage.fillAmount <= 0)
            {
                skillImage.fillAmount = 0;
                isCooldown = false;
            }
        }

    }
}
