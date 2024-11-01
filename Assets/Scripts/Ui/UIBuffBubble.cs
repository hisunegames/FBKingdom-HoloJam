using TMPro;
using UnityEngine;

public class UIBuffBubble : MonoBehaviour
{
    [SerializeField] private Febucci.UI.TypewriterByCharacter message;
    [SerializeField] private TMP_Text buff;

    [SerializeField] private float lifetime = 5f;

    private float disposeTime;

    public void Initialize(string messageText, string buffText)
    {
        message.ShowText(messageText);
        buff.text = $"{{vertexp}}<bounce a=0.3>{buffText}</bounce>{{/vertexp}}";

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
