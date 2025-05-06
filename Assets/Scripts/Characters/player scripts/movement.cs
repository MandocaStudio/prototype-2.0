using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using System.Collections;

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

    [SerializeField] bool isClimbing;

    int floorLayer;

    [SerializeField] bool canRepulse;

    [SerializeField] bool isRepulsing;

    public bool canMove;

    [SerializeField] float polarityForceWeakFloor; //6.7
    [SerializeField] float polarityForceStrongFloor; //10.3
    [SerializeField] float polarityForceWeakWall; //6.7
    [SerializeField] float polarityForceStrongWall; //10.3


    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rbPlayer = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        polarityChangerScript = GetComponent<polarityChanger>();

        floorLayer = LayerMask.NameToLayer("floor");
        canRepulse = false;

        isClimbing = true;

        canMove = true;

        isRepulsing = false;

        //Debug.DrawRay(transform.position, Vector3.right * 2f, Color.red, 1f);

    }

    // Update is called once per frame
    void Update()
    {
        if (playerInput.actions["traversableButton"].WasPressedThisFrame() && !traversing)
        {
            traversing = true;
        }

        if (playerInput.actions["polarityChanger"].WasPressedThisFrame())
        {
            canRepulse = true;
        }
    }

    [SerializeField] float maxSpeed = 6f;

    void FixedUpdate()
    {
        if (canMove)
        {
            moveX = playerInput.actions["movement"].ReadValue<Vector2>().x;

            if (Mathf.Abs(moveX) > 0.01f)
            {
                // Aplica fuerza inmediata en el eje X
                rbPlayer.AddForce(new Vector3(moveX, 0, 0) * speed, ForceMode.VelocityChange);

                // Limita la velocidad máxima en X
                float clampedX = Mathf.Clamp(rbPlayer.linearVelocity.x, -maxSpeed, maxSpeed);
                rbPlayer.linearVelocity = new Vector3(clampedX, rbPlayer.linearVelocity.y, rbPlayer.linearVelocity.z);
            }
            else if (Mathf.Abs(moveX) < 0.01f && grounded)

            {
                // Detiene completamente el movimiento en X si no hay input
                rbPlayer.linearVelocity = new Vector3(0f, rbPlayer.linearVelocity.y, rbPlayer.linearVelocity.z);
            }
        }
        else
        {
            moveX = 0;

        }
    }


    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.layer == floorLayer)
        {
            grounded = true;
        }

        if (collision.collider.CompareTag("Magnetic Structure") && grounded)
        {
            isRepulsing = false;

            structurePolarityChanger structurePolarity = collision.collider.GetComponent<structurePolarityChanger>();

            if (structurePolarity != null)
            {
                if (polarityChangerScript.polarity == structurePolarity.polarity)
                {
                    isRepulsing = true;

                    magneticForce(structurePolarity, polarityForceWeakFloor);

                }
            }
        }

        if (collision.collider.CompareTag("Magnetic Structure Wall") && !grounded)
        {

            isClimbing = true;

            structurePolarityChanger structurePolarity = collision.collider.GetComponent<structurePolarityChanger>();

            if (structurePolarity != null)
            {
                if (polarityChangerScript.polarity == structurePolarity.polarity)
                {

                    magneticForce(structurePolarity, polarityForceWeakWall);
                }

                if (polarityChangerScript.polarity != structurePolarity.polarity)
                {
                    rbPlayer.constraints |= RigidbodyConstraints.FreezePositionY;


                }
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {

        if (collision.gameObject.layer == floorLayer)
        {
            grounded = false;
        }

        if (collision.collider.CompareTag("Magnetic Structure Wall") && !grounded)
        {
            isClimbing = false;

            structurePolarityChanger structurePolarity = collision.collider.GetComponent<structurePolarityChanger>();

            if (structurePolarity != null)
            {

                if (polarityChangerScript.polarity != structurePolarity.polarity)
                {
                    rbPlayer.constraints &= ~RigidbodyConstraints.FreezePositionY;

                }
            }
        }

        if (collision.collider.CompareTag("Magnetic Structure") && grounded)
        {
            structurePolarityChanger structurePolarity = collision.collider.GetComponent<structurePolarityChanger>();

            if (structurePolarity != null)
            {
                if (polarityChangerScript.polarity == structurePolarity.polarity)
                {
                    isRepulsing = false;
                }
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == floorLayer)
        {
            grounded = true;
        }

        if (collision.collider.CompareTag("Magnetic Structure") && grounded)
        {
            structurePolarityChanger structurePolarity = collision.collider.GetComponent<structurePolarityChanger>();

            if (structurePolarity != null)
            {

                if (polarityChangerScript.polarity == structurePolarity.polarity && canRepulse == true && !isRepulsing)
                {
                    canRepulse = false;

                    magneticForce(structurePolarity, polarityForceStrongFloor);
                }
                else if (polarityChangerScript.polarity != structurePolarity.polarity && canRepulse == true)
                {
                    canRepulse = false;
                }
            }
        }

        if (collision.collider.CompareTag("Magnetic Structure Wall") && isClimbing)
        {
            structurePolarityChanger structurePolarity = collision.collider.GetComponent<structurePolarityChanger>();

            if (structurePolarity != null)
            {

                if (polarityChangerScript.polarity == structurePolarity.polarity && canRepulse == true)
                {
                    canRepulse = false;
                    rbPlayer.constraints &= ~RigidbodyConstraints.FreezePositionY;

                    magneticForce(structurePolarity, polarityForceStrongWall);
                }
                else if (polarityChangerScript.polarity != structurePolarity.polarity && canRepulse == true)
                {
                    canRepulse = false;
                }
            }
        }

    }

    private void magneticForce(structurePolarityChanger structurePolarity, float magneticForce)
    {
        switch (structurePolarity.forceDirectionVar)
        {
            case structurePolarityChanger.forceDirection.right:
                Debug.Log("entra right");
                StartCoroutine(waitSeconds(Vector3.right, magneticForce));

                break;

            case structurePolarityChanger.forceDirection.left:
                Debug.Log("entra left");

                StartCoroutine(waitSeconds(Vector3.left, magneticForce));


                break;

            case structurePolarityChanger.forceDirection.up:
                rbPlayer.AddForce(Vector3.up * magneticForce, ForceMode.VelocityChange);

                break;

            case structurePolarityChanger.forceDirection.down:
                rbPlayer.AddForce(Vector3.down * magneticForce, ForceMode.VelocityChange);

                break;
        }

    }

    private IEnumerator waitSeconds(Vector3 direction, float force)
    {
        canMove = false;

        rbPlayer.linearVelocity = Vector3.zero; // resetea velocidad previa

        rbPlayer.AddForce(direction * force, ForceMode.VelocityChange);


        yield return new WaitForSeconds(0.5f);

        canMove = true;

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
