using UnityEngine;

public class DestroyOnShockWave : MonoBehaviour
{
    public GameObject destroyparticle;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("shockwave"))
        {
            Debug.Log("collided");
            if (destroyparticle != null)
            {
                Instantiate(destroyparticle, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}
