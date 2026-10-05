using UnityEngine;

public class startManager : MonoBehaviour
{
    bool canstart;

    public GameObject firststartpanel;
    public GameObject secondstatpane;

    private void Awake()
    {
        Invoke(nameof(allowstart), 0.2f);
    }
    void allowstart()
    {
        canstart = true;
    }
    public void OnclickStart()
    {
        if (!canstart)
            return;
        secondstatpane.SetActive(true);
        firststartpanel.SetActive(false);
        canstart = false;
        Invoke(nameof(allowstart), 0.2f);
    }
    public void OnClick2ndStart()
    {
        if (!canstart)
            return;
        secondstatpane.SetActive(false); 
        game.SetActive(true);
    }
    public GameObject game;
}
