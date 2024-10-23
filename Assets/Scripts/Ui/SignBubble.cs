using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignBubble : MonoBehaviour
{
    [SerializeField] private TMP_Text tmp_text;
    [SerializeField] private SpriteRenderer sprite_renderer;
    private string dialog;
    private Vector2 offset;


    private void Start() 
    {
        tmp_text.ForceMeshUpdate();
        offset = sprite_renderer.size - new Vector2(tmp_text.textBounds.size.x+offset.x,tmp_text.textBounds.size.y+offset.y);
        dialog = tmp_text.text;
        StopAllCoroutines();
        StartCoroutine(ShowSignText());
    }


    private IEnumerator ShowSignText()
    {
        tmp_text.text = "";
        
        for (int i = 0; i < dialog.Length; i++)
        {   
            tmp_text.text += dialog[i];
            tmp_text.ForceMeshUpdate();
            sprite_renderer.size = new Vector2(tmp_text.textBounds.size.x+offset.x,tmp_text.textBounds.size.y+offset.y);
            
            yield return new WaitForSeconds(0.05f);
            
        } 
    }

    
}
