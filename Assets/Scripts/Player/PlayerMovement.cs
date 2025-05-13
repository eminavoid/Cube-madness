using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, IUpdatable
{
    [SerializeField] public float forwardSpeed = 5f;
    [SerializeField] public float horizontalSpeed = 5f;

    public string moveActionName = "Move";

    private Rigidbody rb;
    private CustomUpdateManager updateManager;
    private InputAction moveAction;

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
        }
    }
    private void OnDisable()
    {
        updateManager.Unregister(this);
        moveAction.Disable();
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
