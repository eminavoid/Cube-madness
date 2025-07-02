using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DotController : MonoBehaviour, IUpdatable
{
    public float moveSpeed = 5f;
    public float forwardSpeed = 3f; 
    public string moveActionName = "Move";
    public GameObject miniPlayerPrefab;
    public int numberOfMiniPlayers = 5;
    public Vector3 spreadOffset = new Vector3(-1f, 0f, 0f);
    public float followingDistance = 1f; 
    public float followSpeedMultiplier = 1f;

    private Rigidbody rb;
    private CustomUpdateManager updateManager;
    private InputAction moveAction;
    private MiniPlayerPool miniPlayerPool;
    public List<MiniPlayer> activeMiniPlayers = new List<MiniPlayer>();

    public CanvasGroup canvas;
    public float delay = 3f;

    private UIManager ui;

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
        ui = FindAnyObjectByType<UIManager>();
        if (updateManager != null)
        {
            updateManager.Register(this);

            var hider = new TimedUIHider(canvas, delay, (self) =>
            {
                updateManager.Unregister(self);
            });
            
            updateManager.Register(hider);

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

        for (int i = 0; i < numberOfMiniPlayers; i++)
        {
            MiniPlayer miniPlayer = miniPlayerPool.GetObject();
            if (miniPlayer != null)
            {
                activeMiniPlayers.Add(miniPlayer);
            }
        }

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
        foreach (var miniPlayer in activeMiniPlayers)
        {
            if (miniPlayer != null)
            {
                miniPlayer.ReturnToPool();
            }
        }
        activeMiniPlayers.Clear();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Win"))
        {
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            int nextIndex = currentIndex + 1;

            // Verificamos que exista una siguiente escena
            if (nextIndex < SceneManager.sceneCountInBuildSettings)
            {
                ui.Play();
            }
            else
            {
                PlayerPrefs.DeleteKey("SavedLevelAddress");
                ui.BackToMenu();
            }
        }
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
                    float offsetFactor = .5f;
                    Vector3 offset = new Vector3(followingDistance * offsetFactor, spreadOffset.y * offsetFactor, spreadOffset.z * offsetFactor);
                    Vector3 spawnPosition = transform.position + offset * activeMiniPlayers.Count;
                    miniPlayer.transform.position = spawnPosition;
                    miniPlayer.SetTarget(transform.position + new Vector3(followingDistance * (activeMiniPlayers.Count) * 0.5f, spreadOffset.y * (activeMiniPlayers.Count) * 0.5f, spreadOffset.z * (activeMiniPlayers.Count) * 0.5f));
                    miniPlayer.followSpeed = activeMiniPlayers[^1].followSpeed * followSpeedMultiplier;
                }
            }
            Debug.Log($"Spawned players. New count: {activeMiniPlayers.Count}");

        }
        else if (difference < 0)
        {
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

        float horizontalInput = moveAction.ReadValue<Vector2>().x;

        Vector3 zMovement = new Vector3(0f, 0f, horizontalInput) * moveSpeed;

        Vector3 forwardMovement = Vector3.left * forwardSpeed;

        rb.linearVelocity = forwardMovement + zMovement; // Using linearVelocity

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
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    public void ApplyMathOperation(MathOperation operation, int value)
    {
        switch (operation)
        {
            case MathOperation.Add: numberOfMiniPlayers += value; break;
            case MathOperation.Subtract: numberOfMiniPlayers -= value; break;
            case MathOperation.Multiply: numberOfMiniPlayers *= value; break;
            case MathOperation.Divide:
                if (value != 0) numberOfMiniPlayers /= value;
                else Debug.LogWarning("Division by zero in MathWall");
                break;
        }

        numberOfMiniPlayers = Mathf.Max(0, numberOfMiniPlayers);
        AdjustMiniPlayerCount();
    }
}