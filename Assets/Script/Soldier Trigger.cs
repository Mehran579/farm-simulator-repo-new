using System.Collections;
using System.IO;
using UnityEngine;

public class SoldierTrigger : MonoBehaviour
{
    public GameObject[] newExclampations;
    public DialogueData soldierentry;
    public KnightsHopAway[] soldiersHoppingLmao;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && PlayerManager.hasTalkedToBiblicallyAccurateAngel)
        {
            //Debug.Log("Triggered");
            CameraShake.TriggerShake(0.5f, 0.5f);
            collision.GetComponent<PlayerManager>().currentState = PlayerManager.PlayerState.Cutscene;
            //foreach (GameObject exclampation in newExclampations)
            //{
            //exclampation.SetActive(true);
            Invoke(nameof(setExclampationtoAcive), Random.Range(0, 0.3f));
            //Destroy(exclampation, 0.5f);
            //}
            DialogueManager.Instance.StartDialogue(soldierentry, () =>
            {
                foreach (KnightsHopAway soldier in soldiersHoppingLmao)
                {
                    soldier.StartHopping();
                    //DialogueManager.Instance.StartDialogue(soldierConvo1);
                }
            });
            Invoke(nameof(startSoldierConvo), 4.5f);
            GetComponent<Collider2D>().enabled = false;
        }
    }

    public soldierHead soldierhead;
    public DialogueData soldierConvo1;
    public DialogueData soldiverConvo2;
    public DialogueData lastsoldierCOnvo;
    void startSoldierConvo()
    {
        //Debug.Log("Starting soldier conversation.");
        DialogueManager.Instance.StartDialogue(soldierConvo1, () =>
        {
            soldierhead.getOffHorse();
            DialogueManager.Instance.StartDialogue(soldiverConvo2, () =>
            {
                CameraShake.TriggerShake(0.3f, 0.2f);
                DialogueManager.Instance.StartDialogue(lastsoldierCOnvo, () =>
                {
                    marketDoor.StartCoroutine(marketDoor.ChangeLevel(player));
                });
                //StartCoroutine(endroutine());
                //meowwwwwwwwwwwww
            });
        });
    }
    public Door_House marketDoor;
    public Collider2D player;
    void setExclampationtoAcive()
    {
        foreach (GameObject exclampation in newExclampations)
        {
            exclampation.SetActive(true);
            //Invoke(nameof(setExclampationtoAcive), Random.Range(0, 0.3f));
            Destroy(exclampation, Random.Range(1, 1.5f));
        }
    }

}
