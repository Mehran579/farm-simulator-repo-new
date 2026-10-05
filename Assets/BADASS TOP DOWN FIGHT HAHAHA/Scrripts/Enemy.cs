using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public int health = 3;
    public float moveSpeed = 3f;

    [Header("Targeting")]
    public float offsetRadius = 0.8f;
    public float repickMin = 2f;
    public float repickMax = 4f;

    [Header("Contact Damage")]
    public int contactDamage = 1;
    public float contactCooldown = 0.5f;

    Rigidbody2D rb;
    float nextHitTime;
    Transform[] targets;
    Transform target;
    Vector2 offset;

    HitFlash hitFlash;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hitFlash = GetComponent<HitFlash>();
    }

    public void Init(Transform[] targets)
    {
        this.targets = targets;
        offset = Random.insideUnitCircle * offsetRadius;
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

        Vector2 goal = (Vector2)target.position + offset;
        rb.MovePosition(Vector2.MoveTowards(rb.position, goal, moveSpeed * Time.fixedDeltaTime));
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if (Time.time < nextHitTime) return;
        if (!col.gameObject.CompareTag("Player")) return;

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