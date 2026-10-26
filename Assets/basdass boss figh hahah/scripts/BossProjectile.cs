using UnityEngine;

public class BossProjectile : MonoBehaviour, IDamageable
{
    [SerializeField] int damage = 1;

    [Header("Reflect (for the bigger projectile)")]
    [SerializeField] bool reflectable = false;
    [SerializeField] float reflectSpeed = 20f;
    [SerializeField] int reflectedDamage = 5;     // damage dealt to the boss when it comes back

    Vector2 direction;
    float speed;
    bool launched;
    bool reflected;

    Transform homingTarget;
    float turnSpeed;          // degrees per second
    float homingTimeLeft;

    // Straight shot
    public void Launch(Vector2 dir, float speed, float lifetime)
    {
        direction = dir.normalized;
        this.speed = speed;
        launched = true;
        Destroy(gameObject, lifetime);
    }

    // Homing missile: turns toward the target until homingTime runs out, then flies straight
    public void LaunchHoming(Vector2 dir, float speed, Transform target, float turnSpeed, float homingTime, float lifetime)
    {
        homingTarget = target;
        this.turnSpeed = turnSpeed;
        homingTimeLeft = homingTime;
        Launch(dir, speed, lifetime);
    }

    void Update()
    {
        if (!launched) return;   // while hovering before launch it does nothing (and can't hurt anyone)

        if (homingTarget != null && homingTimeLeft > 0f)
        {
            homingTimeLeft -= Time.deltaTime;

            Vector2 toTarget = (Vector2)homingTarget.position - (Vector2)transform.position;
            float current = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float wanted = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;

            float angle = Mathf.MoveTowardsAngle(current, wanted, turnSpeed * Time.deltaTime) * Mathf.Deg2Rad;
            direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        transform.right = direction;
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    // The player's swing calls this (same path as enemies and the boss).
    // A reflectable projectile turns around and heads for the boss.
    public void TakeDamage(int amount)
    {
        if (!reflectable || !launched || reflected) return;

        BossController boss = FindFirstObjectByType<BossController>();
        if (boss == null)
        {
            Destroy(gameObject);
            return;
        }

        reflected = true;
        homingTarget = null;
        speed = reflectSpeed;
        direction = ((Vector2)boss.transform.position - (Vector2)transform.position).normalized;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!launched) return;

        // After a reflect it can only hurt the boss, never the player
        if (reflected)
        {
            if (other.TryGetComponent<BossController>(out var boss))
            {
                boss.TakeDamage(reflectedDamage);
                Destroy(gameObject);
            }
            return;
        }

        if (!other.CompareTag("Player")) return;

        if (other.TryGetComponent<IDamageable>(out var target))
            target.TakeDamage(damage);

        Destroy(gameObject);
    }
}