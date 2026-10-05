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
    //void startbossSequence(GameObject Player)
    //{
    //    Rigidbody2D rb = Player.GetComponent<Rigidbody2D>();

    //    rb.AddForce(
    //        (Player.transform.position - transform.position).normalized * knocbackforce,
    //        ForceMode2D.Impulse
    //    );

    //    bossSequence.SetActive(true);
    //    StartCoroutine(StopPlayer(rb));
    //}
    void startbossSequence(GameObject Player)
    {
        Rigidbody2D rb = Player.GetComponent<Rigidbody2D>();
        PlayerManager pm = Player.GetComponent<PlayerManager>();
        pm.currentState = PlayerManager.PlayerState.knockback;
        Vector2 direction = (Player.transform.position - transform.position).normalized;
        rb.AddForce(direction * knocbackforce, ForceMode2D.Impulse);
        bossSequence.SetActive(true);
        inverontru.SetActive(false);
        StartCoroutine(StopPlayer(rb, pm));

    }
    public GameObject inverontru;

    IEnumerator StopPlayer(Rigidbody2D rb, PlayerManager pm)
    {
        yield return new WaitForSeconds(0.15f);

        rb.linearVelocity = Vector2.zero;
        pm.currentState = PlayerManager.PlayerState.Cutscene;
    }
    private void OnDestroy()
{
    //Debug.LogError(
    //    $"CROP DESTROYED: {gameObject.name}\n" +
    //    $"Scene: {gameObject.scene.name}\n" +
    //    $"Instance ID: {GetInstanceID()}\n" +
    //    $"Stack:\n{System.Environment.StackTrace}"
    //);
}
}