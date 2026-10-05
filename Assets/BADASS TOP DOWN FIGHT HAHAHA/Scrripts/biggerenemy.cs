using UnityEngine;

public class biggerenemy : MonoBehaviour, IDamageable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    SpriteRenderer sr;
    Enemy enemey;
    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        enemey = GetComponent<Enemy>();
    }
    public Color NOrmalcolor;
    public void TakeDamage(int damage)
    {
        if(enemey.health == 2)
            sr.color = NOrmalcolor; 
        
        if (enemey.health == 1)
            transform.localScale = Vector3.one;
    }
}
