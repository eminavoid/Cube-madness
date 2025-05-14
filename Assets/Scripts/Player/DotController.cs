using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DotController : MonoBehaviour, IUpdatable
{
    public float moveSpeed = 5f;
    public float forwardSpeed = 3f; // Speed to move forward in -X
    public string moveActionName = "Move"; // Name of the input action for horizontal movement (now in Z-axis)
    public GameObject miniPlayerPrefab;
    public int numberOfMiniPlayers = 5;
    public Vector3 spreadOffset = new Vector3(-1f, 0f, 0f); // Offset between each mini-player (local offset from the target)
    public float followingDistance = 1f; // Base distance behind the dot
    public float followSpeedMultiplier = 1f; // Adjust how quickly mini-players follow

    private Rigidbody rb;
    private CustomUpdateManager updateManager;
    private InputAction moveAction;
    private MiniPlayerPool miniPlayerPool;
    public List<MiniPlayer> activeMiniPlayers = new List<MiniPlayer>();

    void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ; // Prevent rotation in Y and Z
            rb.isKinematic = false;
        }

        updateManager = FindAnyObjectByType<CustomUpdateManager>();
        if (updateManager != null)
        {
            updateManager.Register(this);

            PlayerInput playerInput = GetComponentInParent<PlayerInput>();
            if (playerInput != null)
            {
                moveAction = playerInput.actions.FindAction(moveActionName);
                if (moveAction == null)
                {
                    Debug.LogError($"Input Action '{moveActionName}' not found in the PlayerInput Asset.");
                    enabled = false;
                }
                else
                {
                    moveAction.Enable();
                }
            }
            else
            {
                Debug.LogError("PlayerInput component not found on this GameObject or its parents.");
                enabled = false;
            }
        }
        else
        {
            Debug.LogError("CustomUpdateManager not found in the scene. Please add one.");
            enabled = false;
        }

        if (miniPlayerPrefab == null)
        {
            Debug.LogError("Mini Player Prefab not assigned in the Inspector.");
            enabled = false;
            return;
        }

        miniPlayerPool = new MiniPlayerPool(miniPlayerPrefab, numberOfMiniPlayers, updateManager);

        // Spawn initial mini players
        for (int i = 0; i < numberOfMiniPlayers; i++)
        {
            MiniPlayer miniPlayer = miniPlayerPool.GetObject();
            if (miniPlayer != null)
            {
                activeMiniPlayers.Add(miniPlayer);
            }
        }

        // Initial positioning behind the dot (in positive X)
        for (int i = 0; i < activeMiniPlayers.Count; i++)
        {
            float offset = followingDistance * (i + 1);
            activeMiniPlayers[i].transform.position = transform.position + new Vector3(offset, spreadOffset.y * (i + 1), spreadOffset.z * (i + 1));
            activeMiniPlayers[i].SetTarget(transform.position + new Vector3(offset, spreadOffset.y * (i + 1), spreadOffset.z * (i + 1))); // Initial target position
            activeMiniPlayers[i].followSpeed = activeMiniPlayers[i].followSpeed * followSpeedMultiplier; // Apply a multiplier for individual control
        }
    }

    void OnDisable()
    {
        if (updateManager != null)
        {
            updateManager.Unregister(this);
        }
        if (moveAction != null)
        {
            moveAction.Disable();
        }
        // Return all active mini players to the pool when the dot is disabled
        foreach (var miniPlayer in activeMiniPlayers)
        {
            if (miniPlayer != null)
            {
                miniPlayer.ReturnToPool();
            }
        }
        activeMiniPlayers.Clear();
    }
    public void RemoveMiniPlayer(MiniPlayer miniPlayerToRemove)
    {
        if (activeMiniPlayers.Contains(miniPlayerToRemove))
        {
            activeMiniPlayers.Remove(miniPlayerToRemove);
            Debug.Log($"Removing miniplayer. Remaining count: {activeMiniPlayers.Count}");

            miniPlayerToRemove.ReturnToPool();
            numberOfMiniPlayers--;
            UpdateMiniPlayerTargets();
        }
    }
    public void AdjustMiniPlayerCount()
    {
        int difference = numberOfMiniPlayers - activeMiniPlayers.Count;
        Debug.Log($"AdjustMiniPlayerCount called. Target count: {numberOfMiniPlayers}, Active count: {activeMiniPlayers.Count}, Difference: {difference}");

        Debug.Log(difference);

        if (difference > 0)
        {
            // Spawn new mini players
            for (int i = 0; i < difference; i++)
            {
                MiniPlayer miniPlayer = miniPlayerPool.GetObject();
                if (miniPlayer != null)
                {
                    Rigidbody rb = miniPlayer.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.linearVelocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;
                    }
                    miniPlayer.gameObject.SetActive(true);
                    activeMiniPlayers.Add(miniPlayer);
                    // Position the new mini player (you might want a more sophisticated way to position them)
                    // MODIFIED LINES BELOW:
                    float offsetFactor = .5f;
                    Vector3 offset = new Vector3(followingDistance * offsetFactor, spreadOffset.y * offsetFactor, spreadOffset.z * offsetFactor);
                    Vector3 spawnPosition = transform.position + offset * activeMiniPlayers.Count;
                    miniPlayer.transform.position = spawnPosition;
                    miniPlayer.SetTarget(transform.position + new Vector3(followingDistance * (activeMiniPlayers.Count) * 0.5f, spreadOffset.y * (activeMiniPlayers.Count) * 0.5f, spreadOffset.z * (activeMiniPlayers.Count) * 0.5f));
                    // END OF MODIFIED LINES
                    miniPlayer.followSpeed = activeMiniPlayers[^1].followSpeed * followSpeedMultiplier;
                }
            }
            Debug.Log($"Spawned players. New count: {activeMiniPlayers.Count}");

        }
        else if (difference < 0)
        {
            // Despawn extra mini players
            for (int i = 0; i < Mathf.Abs(difference); i++)
            {
                if (activeMiniPlayers.Count > 0)
                {
                    int lastIndex = activeMiniPlayers.Count - 1;
                    if (activeMiniPlayers[lastIndex] != null)
                    {
                        activeMiniPlayers[lastIndex].ReturnToPool();
                    }
                    activeMiniPlayers.RemoveAt(lastIndex);
                }
            }
            Debug.Log($"Despawned players. New count: {activeMiniPlayers.Count}");
        }
        // Optionally update targets for all remaining mini players
        UpdateMiniPlayerTargets();
    }

    private void UpdateMiniPlayerTargets()
    {
        for (int i = 0; i < activeMiniPlayers.Count; i++)
        {
            if (activeMiniPlayers[i] != null)
            {
                Vector3 targetPosition = transform.position + new Vector3(followingDistance * (i + 1) + spreadOffset.x * (i + 1), spreadOffset.y * (i + 1), spreadOffset.z * (i + 1));
                activeMiniPlayers[i].SetTarget(targetPosition);
            }
        }
    }
    public void Tick(float deltaTime)
    {
        if (rb == null || moveAction == null) return;

        // Get horizontal input value for Z-axis movement (-1 to 1)
        float horizontalInput = moveAction.ReadValue<Vector2>().x;

        // Calculate movement in the z-axis
        Vector3 zMovement = new Vector3(0f, 0f, horizontalInput) * moveSpeed;

        // Always move forward in the negative X direction
        Vector3 forwardMovement = Vector3.left * forwardSpeed;

        // Apply both movements to the dot's Rigidbody
        rb.linearVelocity = forwardMovement + zMovement; // Using linearVelocity

        // Update mini-player targets to be behind the dot
        for (int i = 0; i < activeMiniPlayers.Count; i++)
        {
            if (activeMiniPlayers[i] != null)
            {
                Vector3 targetPosition = transform.position + new Vector3(followingDistance * (i + 1) + spreadOffset.x * (i + 1), spreadOffset.y * (i + 1), spreadOffset.z * (i + 1));
                activeMiniPlayers[i].SetTarget(targetPosition);
            }
        }

        if (activeMiniPlayers.Count <= 0)
        {
            SceneManager.LoadScene("Main Menu");
        }
    }
}