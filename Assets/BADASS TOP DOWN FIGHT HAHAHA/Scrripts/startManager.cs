using UnityEngine;
using UnityEngine.SceneManagement;

public class startManager : MonoBehaviour
{
    bool canstart;

    static float fightStartTime = -1f; 
    const float skipDelay = 12f;

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

        if (fightStartTime < 0f)
            fightStartTime = Time.unscaledTime;

        float remaining = Mathf.Max(0f, skipDelay - (Time.unscaledTime - fightStartTime));
        Invoke(nameof(ShowSKipButton), remaining);
    }
    public void ShowSKipButton()
    {
        skipBUtton.SetActive(true);
    }
    public void OnClickSkip()
    {
        fightStartTime = -1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
    public GameObject game;
    public GameObject skipBUtton;
}