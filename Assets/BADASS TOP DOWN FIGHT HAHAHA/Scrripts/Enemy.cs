using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] int health = 3;
    [SerializeField] float moveSpeed = 3f;

    [Header("Targeting")]
    [SerializeField] float offsetRadius = 0.8f;   // each enemy aims slightly off the target so they don't stack
    [SerializeField] float repickMin = 2f;        // seconds before choosing a new target
    [SerializeField] float repickMax = 4f;

    [Header("Contact Damage")]
    [SerializeField] int contactDamage = 1;
    [SerializeField] float contactCooldown = 0.5f;   // seconds between hits from this enemy

    Rigidbody2D rb;
    float nextHitTime;
    Transform[] targets;      // the 4 track points + the player
    Transform target;
    Vector2 offset;

    HitFlash hitFlash;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hitFlash = GetComponent<HitFlash>();
    }

    // The spawner calls this right after Instantiate
    public void Init(Transform[] targets)
    {
        this.targets = targets;
        offset = Random.insideUnitCircle * offsetRadius;   // picked once, kept for this enemy's life
        StartCoroutine(RepickRoutine());
    }

    IEnumerator RepickRoutine()
    {
        while (true)
        {
            target = targets[Random.Range(0, targets.Length)];
            yield return new WaitForSeconds(Random.Range(repickMin, repickMax));
        }
    }

    void FixedUpdate()
    {
        if (target == null) return;

        // Re-read the position every tick, so the enemy follows the target as it moves
        Vector2 goal = (Vector2)target.position + offset;
        rb.MovePosition(Vector2.MoveTowards(rb.position, goal, moveSpeed * Time.fixedDeltaTime));
    }

    // Runs every physics tick while this enemy's collider touches another collider
    void OnCollisionStay2D(Collision2D col)
    {
        if (Time.time < nextHitTime) return;                 // still on cooldown
        if (!col.gameObject.CompareTag("Player")) return;    // enemies are IDamageable too, so filter by tag

        if (col.gameObject.TryGetComponent<IDamageable>(out var target))
        {
            target.TakeDamage(contactDamage);
            nextHitTime = Time.time + contactCooldown;
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0) Destroy(gameObject);
        else if (hitFlash != null) hitFlash.Flash();
    }
}