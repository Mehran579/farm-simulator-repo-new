using System.Collections;
using UnityEngine;

// Prefab layout:
//   Root:  this script + a Collider2D (Is Trigger)
//   Child: "Shadow"        - dark sprite shown as the warning
//   Child: "DamageVisual"  - the actual attack sprite (give it a higher Order in Layer than Shadow, it draws on top)
[RequireComponent(typeof(Collider2D))]
public class GroundHazard : MonoBehaviour
{
    [SerializeField] GameObject shadow;
    [SerializeField] GameObject damageVisual;
    [SerializeField] int damage = 1;

    Collider2D hitbox;
    bool active;
    bool hasHit;       // each hazard hurts the player at most once

    void Awake() => hitbox = GetComponent<Collider2D>();

    // shadowDelay   = seconds before the shadow appears
    // telegraphTime = how long the shadow is shown before the damage comes up
    // activeTime    = how long the damage stays up (shadow stays under it), then everything disappears
    public void Init(float shadowDelay, float telegraphTime, float activeTime)
    {
        StartCoroutine(Run(shadowDelay, telegraphTime, activeTime));
    }

    IEnumerator Run(float shadowDelay, float telegraphTime, float activeTime)
    {
        shadow.SetActive(false);
        damageVisual.SetActive(false);
        hitbox.enabled = false;

        yield return new WaitForSeconds(shadowDelay);
        shadow.SetActive(true);

        yield return new WaitForSeconds(telegraphTime);
        damageVisual.SetActive(true);   // drawn on top of the shadow, which stays
        hitbox.enabled = true;          // damage starts at the same moment the visual appears
        active = true;

        yield return new WaitForSeconds(activeTime);
        Destroy(gameObject);
    }

    // Stay (not Enter), so a player already standing inside when it goes live still gets hit
    void OnTriggerStay2D(Collider2D other)
    {
        if (!active || hasHit || !other.CompareTag("Player")) return;

        if (other.TryGetComponent<IDamageable>(out var target))
        {
            target.TakeDamage(damage);
            hasHit = true;
        }
    }
}