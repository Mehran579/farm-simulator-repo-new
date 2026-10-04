using UnityEngine;

public class cutSceneTrigger : MonoBehaviour
{
    bool started;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerManager playerManager = collision.GetComponent<PlayerManager>();
            if (playerManager != null && PlayerManager.canMove && !started)
            {
                playerManager.currentCutSceneState = PlayerManager.CutSceneState.GoToFather;
                playerManager.currentState = PlayerManager.PlayerState.MovingCutScene;
                started = true;
            }
        }

    }
}
