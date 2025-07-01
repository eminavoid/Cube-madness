using Unity.VisualScripting;
using UnityEngine;

public class Lava : MonoBehaviour
{
    public string miniPlayerTag = "MiniPlayer"; // Assign this tag to your MiniPlayer prefab

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(miniPlayerTag))
        {
            MiniPlayer miniPlayer = other.GetComponent<MiniPlayer>();
            if (miniPlayer != null)
            {
                // Find the DotController in the scene
                DotController dotController = FindAnyObjectByType<DotController>();
                if (dotController != null)
                {
                    Debug.Log($"Hazard hit. Removing miniplayer. Current count: {dotController.activeMiniPlayers.Count}");

                    dotController.RemoveMiniPlayer(miniPlayer);
                }
                else
                {
                    Debug.LogError("DotController not found in the scene.");
                }
            }
        }
    }
}
