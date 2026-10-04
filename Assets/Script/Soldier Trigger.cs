using UnityEngine;

public class SoldierTrigger : MonoBehaviour
{
    public GameObject[] newExclampations;
    public DialogueData soldierentry;
    public HopAway[] soldiersHoppingLmao;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && PlayerManager.hasTalkedToBiblicallyAccurateAngel)
        {
            Debug.Log("Triggered");
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
                foreach (HopAway soldier in soldiersHoppingLmao)
                {
                    soldier.StartHopping();
                }
            });
        }
    }
    void setExclampationtoAcive()
    {
        foreach (GameObject exclampation in newExclampations)
        {
            exclampation.SetActive(true);
            //Invoke(nameof(setExclampationtoAcive), Random.Range(0, 0.3f));
            Destroy(exclampation, Random.Range(0.3f, 0.6f));
        }
    }
}
