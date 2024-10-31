using TMPro;
using UnityEngine;

public class UIBuffBubble : MonoBehaviour
{
    [SerializeField] private TMP_Text message;
    [SerializeField] private TMP_Text buff;

    [SerializeField] private float lifetime = 5f;

    private float disposeTime;

    public void Initialize(string messageText, string buffText)
    {
        message.text = messageText;
        buff.text = buffText;

        disposeTime = Time.time + lifetime;
    }

    public void Update()
    {
        transform.position += Vector3.up * Time.deltaTime;

        if (Time.time >= disposeTime)
        {
            Destroy(gameObject);
        }
    }
}
