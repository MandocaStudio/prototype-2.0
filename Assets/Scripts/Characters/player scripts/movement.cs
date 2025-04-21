using UnityEngine;
using UnityEngine.InputSystem;

public class movement : MonoBehaviour
{
    public PlayerInput playerInput;
    Rigidbody rbPlayer;

    Animator playerAnimator;

    private float moveX;

    [SerializeField] float speed;

    [SerializeField] bool traversing = false;


    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rbPlayer = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInput.actions["traversableButton"].WasPressedThisFrame() && !traversing)
        {
            traversing = true;
        }
    }

    void FixedUpdate()
    {
        moveX = playerInput.actions["movement"].ReadValue<Vector2>().x;

        Vector3 move = new Vector3(moveX, 0f, 0f) * speed;
        rbPlayer.linearVelocity = new Vector3(move.x, rbPlayer.linearVelocity.y, rbPlayer.linearVelocity.z);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Traversable Floor"))
        {
            if (traversing)
            {
                inactivateCollider inactivator = other.GetComponent<inactivateCollider>();
                inactivator.inactivateSon();
                inactivator.canInactivate = false;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Traversable Floor"))
        {
            if (traversing)
            {
                traversing = false;
                inactivateCollider inactivator = other.GetComponent<inactivateCollider>();
                inactivator.canInactivate = true;

                inactivator.inactivateSon();

            }

        }
    }
}
