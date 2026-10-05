using UnityEngine;
using UnityEngine.InputSystem;

// Put this on the indicator object, which is a child of the player.
public class AimIndicator : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float radius = 1.5f;

    Camera cam;

    void Awake()
    {
        cam = Camera.main;
        if (player == null) player = transform.parent;
    }

    void LateUpdate()
    {
        if (Mouse.current == null) return;

        Vector2 mouseWorld = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 dir = mouseWorld - (Vector2)player.position;

        if (dir.sqrMagnitude < 0.0001f) return;

        // Position around player
        Vector2 pos = (Vector2)player.position + dir.normalized * radius;
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);

        // Rotate towards mouse
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}