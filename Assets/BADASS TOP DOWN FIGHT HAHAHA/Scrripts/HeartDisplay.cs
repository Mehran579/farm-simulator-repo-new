using UnityEngine;
using UnityEngine.UI;

public class HeartsDisplay : MonoBehaviour
{
    public playerhealth player;
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    int lastHealth = int.MinValue;
    bool wasActive;

    void Update()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<playerhealth>();
        }

        bool active = player != null && player.gameObject.activeInHierarchy;

        if (!active)
        {
            if (wasActive || lastHealth != int.MinValue)
            {
                SetAll(false);
                lastHealth = int.MinValue;
                wasActive = false;
            }
            return;
        }

        if (!wasActive || player.health != lastHealth)
        {
            wasActive = true;
            lastHealth = player.health;
            Refresh(lastHealth);
        }
    }

    void SetAll(bool on)
    {
        for (int i = 0; i < hearts.Length; i++)
            if (hearts[i] != null) hearts[i].enabled = on;
    }

    void Refresh(int current)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null) continue;

            if (emptyHeart != null)
            {
                hearts[i].enabled = true;
                hearts[i].sprite = i < current ? fullHeart : emptyHeart;
            }
            else
            {
                hearts[i].enabled = i < current;
            }
        }
    }
}