using UnityEngine;
using UnityEngine.InputSystem;

public class movement : MonoBehaviour
{
    [System.NonSerialized]
    public PlayerInput playerInput;

    [System.NonSerialized]
    public Rigidbody rbPlayer;

    [System.NonSerialized]
    public Animator playerAnimator;

    polarityChanger polarityChangerScript;

    private float moveX;

    [SerializeField] float speed;

    [SerializeField] bool traversing = false;

    [SerializeField] bool grounded;

    int magneticLayer;
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rbPlayer = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        polarityChangerScript = GetComponent<polarityChanger>();

        magneticLayer = LayerMask.NameToLayer("floor");
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

    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.layer == magneticLayer)
        {
            grounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {

        if (collision.gameObject.layer == magneticLayer)
        {
            grounded = false;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Magnetic Structure") && grounded)
        {
            structurePolarityChanger structurePolarity = collision.collider.GetComponent<structurePolarityChanger>();

            if (structurePolarity != null)
            {
                if (polarityChangerScript.polarity == structurePolarity.polarity)
                {
                    switch (structurePolarity.forceDirectionVar)
                    {
                        case structurePolarityChanger.forceDirection.right:

                            break;

                        case structurePolarityChanger.forceDirection.left:

                            break;

                        case structurePolarityChanger.forceDirection.up:

                            break;

                        case structurePolarityChanger.forceDirection.down:

                            break;
                    }

                }
            }

        }

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
