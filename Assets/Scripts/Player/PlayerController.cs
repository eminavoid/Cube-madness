using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IUpdatable
{
    [SerializeField] public float forwardSpeed = 4f;
    [SerializeField] public float horizontalSpeed = 4f;
    public string moveActionName = "Move";
    private InputAction moveAction;

    private Rigidbody rb;
    private CustomUpdateManager updateManager;

    public int numberOfMiniPlayers = 5;
    public float followingDistance = 0.5f;
    public GameObject miniPlayerPrefab;
    private MiniPlayerPool miniPlayerPool;
    private List<MiniPlayer> activeMiniPlayers = new List<MiniPlayer>();

    

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();

        updateManager = FindFirstObjectByType<CustomUpdateManager>();
        if (updateManager != null)
        {
            updateManager.Register(this);

            PlayerInput playerInput = GetComponent<PlayerInput>();
            moveAction = playerInput.actions.FindAction(moveActionName);
            moveAction.Enable();
            miniPlayerPool = new MiniPlayerPool(miniPlayerPrefab, numberOfMiniPlayers);

            for (int i = 0; i < numberOfMiniPlayers; i++)
            {
                MiniPlayer miniPlayer = miniPlayerPool.GetObject();
                if (miniPlayer != null)
                {
                    miniPlayer.Initialize(transform, miniPlayerPool);
                    activeMiniPlayers.Add(miniPlayer);
                }
            }

            for (int i = 0; i < activeMiniPlayers.Count; i++)
            {
                float offset = -followingDistance * (i + 1);
                activeMiniPlayers[i].transform.position = transform.position + new Vector3(offset, 0f, 0f);
            }
        }
    }
    private void OnDisable()
    {
        updateManager.Unregister(this);
        moveAction.Disable();

        foreach (var miniPlayer in activeMiniPlayers)
        {
            if (miniPlayer != null)
            {
                miniPlayer.ReturnToPool();
            }
        }
        activeMiniPlayers.Clear();
    }


    public void Tick(float deltaTime)
    {
        if (rb == null || moveAction == null) return;

        rb.linearVelocity = transform.forward * forwardSpeed;

        float horizontalInput = moveAction.ReadValue<Vector2>().x;

        Vector3 horizontalMovement = transform.right * horizontalInput * horizontalSpeed;

        rb.linearVelocity += horizontalMovement;


    }
}
