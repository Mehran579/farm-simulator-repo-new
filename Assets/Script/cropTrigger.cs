using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class cropTrigger : MonoBehaviour
{
    bool hasplaced;
    public bool isBossTrigger;
    public float knocbackforce;
    public GameObject bossSequence;
    public static GameObject carrotSeed;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (PlayerManager.hasCarrotSeeds && !hasplaced && Keyboard.current.leftShiftKey.isPressed)
        {
            Instantiate(carrotSeed, new Vector3(transform.position.x - 0.03112f, transform.position.y - 0.55f, 0) + (Random.onUnitSphere * 0.03f), Quaternion.identity);
            //Debug.Log("fuhhhhhhhhhh nuh");
            hasplaced = true;
            if (isBossTrigger)
                startbossSequence(collision.gameObject);
        }
    }
    void startbossSequence(GameObject Player)
    {
        Rigidbody2D rb = Player.GetComponent<Rigidbody2D>();
        Player.GetComponent<PlayerManager>().currentState = PlayerManager.PlayerState.Cutscene;
        rb.AddForce((Player.transform.position - transform.position).normalized * knocbackforce, ForceMode2D.Impulse);
        //Player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        bossSequence.SetActive(true);
        StartCoroutine(StopPlayer(rb));
    }
    IEnumerator StopPlayer(Rigidbody2D Player)
    {
        //Debug.Log("caleed");
        yield return new WaitForSecondsRealtime(0.15f);
        //Debug.Log("should stop");
        Player.linearVelocity = Vector2.zero;
    }
}