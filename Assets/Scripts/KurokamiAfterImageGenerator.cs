using System.Collections;
using UnityEngine;

public class KurokamiAfterImageGenerator : MonoBehaviour
{
    public static KurokamiAfterImageGenerator instance;
    public GameObject afterImage;
    public GameObject kurokami;
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
        // kurokami = GameObject.Find("KurokamiBoss");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator GenerateAfterImage()
    {
        GameObject _afterImage = Instantiate(afterImage, kurokami.transform.position, kurokami.transform.rotation);
        _afterImage.transform.localScale = kurokami.transform.localScale;
        _afterImage.GetComponent<SpriteRenderer>().enabled = true;
        _afterImage.GetComponent<SpriteRenderer>().sprite = kurokami.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        yield return new WaitForSeconds(afterImageDelay);

    }
}
