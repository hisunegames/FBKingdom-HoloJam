using System.Collections;
using UnityEngine;

public class AfterImageGenerator : MonoBehaviour
{
    public static AfterImageGenerator instance;
    public GameObject afterImage;
    public GameObject player;
    public float afterImageDelay = 0.1f;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator GenerateAfterImage()
    {
        GameObject _afterImage = Instantiate(afterImage, player.transform.position, player.transform.rotation);
        _afterImage.transform.localScale = player.transform.localScale;
        _afterImage.GetComponent<SpriteRenderer>().enabled = true;
        _afterImage.GetComponent<SpriteRenderer>().sprite = player.GetComponent<SpriteRenderer>().sprite;
        yield return new WaitForSeconds(afterImageDelay);

    }
}
