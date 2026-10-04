using UnityEngine;

public class TutoTrigger : MonoBehaviour
{

    public GameObject E;
    public Sprite spriteToshow;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(spriteToshow != null)
        {
            E.GetComponent<SpriteRenderer>().sprite = spriteToshow;
        }
        E.SetActive(true);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        E.SetActive(false);
    }
}
