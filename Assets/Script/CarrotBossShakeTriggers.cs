using System.Collections;
using UnityEngine;

public class CarrotBossShakeTriggers : MonoBehaviour
{

    public GameObject boss;
    public GameObject shockWave;
    void firstShake()
    {
        CameraShake.TriggerShake(duration: 0.1f, magnitude: 0.1f);
    }
    void secondShake()
    {
        CameraShake.TriggerShake(duration: 0.2f, magnitude: 0.2f);
    }
    void thirdshake()
    {
        CameraShake.TriggerShake(duration: 0.3f, magnitude: 0.3f);
    }
    void BossAwake()
    {
        boss.SetActive(true);
        StartCoroutine(ScaleUpBoss(boss, Vector3.zero, Vector3.one));
        Invoke(nameof(lastShake), 0.5f);
    }
    void lastShake()
    {
        CameraShake.TriggerShake(duration: 0.5f, magnitude: 1f);
        StartCoroutine(ScaleUp(shockWave));
        DestroyGrassEtc();
    }

    IEnumerator ScaleUp(GameObject obj)
    {
        obj.SetActive(true);
        Vector3 startScale = obj.transform.localScale;
        Vector3 targetScale = startScale * 100f;

        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;
            obj.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);

            yield return null;
        }

        obj.transform.localScale = targetScale;
        //Debug.Log("reached here");
        Destroy(obj, 3f);


        DialogueManager.Instance.StartDialogue(bossDialogue, () => 
        { 
            StartCoroutine(ScaleUpBoss(boss, Vector3.one, Vector3.zero));
            //Invoke(nameof(playanimationInreverse), 0f);
            GetComponent<Animator>().SetTrigger("reverse");
            exclamption.SetActive(true);
            fatherDTrigger.enabled = false;
            _2ndfatherDTrigger.enabled = true;
        });
    }
    public GameObject exclamption;
    public DialogueTrigger fatherDTrigger;
    public customDialogueTriggerForFather _2ndfatherDTrigger;
    public DialogueData fatherdialogue2;

    public DialogueData bossDialogue;

    IEnumerator ScaleUpBoss(GameObject obj, Vector3 startscale, Vector3 targetscale)
    {
        //Debug.Log("called");
        obj.SetActive(true);
        //Vector3 startScale = obj.transform.localScale;
        //Vector3 targetScale = startScale * 100f;
        //Vector3 targetScale = Vector3.one;

        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Debug.Log("scaling up");
            elapsed += Time.deltaTime;

            float t = elapsed / duration;
            obj.transform.localScale = Vector3.Lerp(startscale, targetscale, t);

            yield return null;
        }

        obj.transform.localScale = targetscale;
        //Debug.Log("finished scaling");
        //Debug.Log("reached here");
        //Destroy(obj, 3f);
        //DialogueManager.Instance.StartDialogue(bossDialogue);
    }

    void playanimationInreverse()
    {
    }

    private void DestroyGrassEtc()
    {
        GameObject[] grass = GameObject.FindGameObjectsWithTag("grass");

        foreach (GameObject g in grass)
        {
            Instantiate(todestroyGrassParticleSystem, g.transform.position, Quaternion.identity);
            Destroy(g);
        }
        GameObject[] carrotbags = GameObject.FindGameObjectsWithTag("carrot bags");

        foreach (GameObject g in carrotbags)
        {
            //Debug.Log("called");
            //Debug.Log(g);
            //Debug.Log(g.transform.position);
            Instantiate(todestroyCarrotBagsParticleSystem, g.transform.position, Quaternion.identity);
            Destroy(g);
        }
        PlayerManager.hasCarrotSeeds = false;
    }
    public GameObject todestroyGrassParticleSystem;
    public GameObject todestroyCarrotBagsParticleSystem;
}
