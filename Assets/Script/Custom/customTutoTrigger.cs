using UnityEngine;

public class customTutoTrigger : MonoBehaviour
{

    public GameObject E;
    public Sprite spriteToshow;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (spriteToshow != null)
        {
            E.GetComponent<SpriteRenderer>().sprite = spriteToshow;
        }
        E.SetActive(true);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        E.SetActive(false);
    }
}
