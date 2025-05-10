using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

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
        //collision.gameObject.layer == floorLayer &&
        if (collision.GetContact(0).normal.y > 0)
        {
            grounded = true;
        }
        // repulsion general
        if (collision.collider.CompareTag("Magnetic Structure"))
        {
            Vector3 contactNormal = collision.GetContact(0).normal;

            structurePolarityChanger structurePolarity = collision.collider.GetComponent<structurePolarityChanger>();

            Debug.Log(contactNormal);
            //repulsion en el suelo
            if (grounded)
            {
                isRepulsing = false;

                if (structurePolarity != null)
                {
                    if (polarityChangerScript.polarity == structurePolarity.polarity)
                    {
                        isRepulsing = true;

                        magneticForce(structurePolarity, polarityForceWeakFloor, contactNormal);

                    }
                }
            }

            //repulsion en la pared
            //modificar si habran paredes pegadas a suelos
            else if (contactNormal.x != 0 && contactNormal.y == 0 && (grounded || !grounded))
            {
                Debug.Log("entra");
                isClimbing = true;

                if (structurePolarity != null)
                {
                    if (polarityChangerScript.polarity == structurePolarity.polarity)
                    {

                        magneticForce(structurePolarity, polarityForceWeakWall, contactNormal);
                    }

                    if (polarityChangerScript.polarity != structurePolarity.polarity)
                    {
                        rbPlayer.constraints |= RigidbodyConstraints.FreezePositionY;
                    }
                }
            }

            //repulsion en esquinas
            // else if (contactNormal.x != 0 && contactNormal.y != 0 && (grounded || !grounded))
            // {
            //     if (polarityChangerScript.polarity == structurePolarity.polarity)
            //     {

            //         magneticForce(structurePolarity, polarityForceWeakWall, contactNormal);
            //     }
            // }

        }
    }

    void OnCollisionExit(Collision collision)
    {
        // &&collision.gameObject.layer == floorLayer

        grounded = false;

        if (rbPlayer.constraints.HasFlag(RigidbodyConstraints.FreezePositionY))
        {
            rbPlayer.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
            isClimbing = false;
        }
        //repulsion general
        if (collision.collider.CompareTag("Magnetic Structure"))
        {
            if (collision.contactCount > 0)
            {

                Vector3 contactNormal = collision.GetContact(0).normal;

                structurePolarityChanger structurePolarity = collision.collider.GetComponent<structurePolarityChanger>();

                //repulsion suelo
                if (grounded)
                {
                    if (structurePolarity != null)
                    {
                        if (polarityChangerScript.polarity == structurePolarity.polarity)
                        {
                            isRepulsing = false;

                        }
                    }
                }

                //repulsion pared
                // else if (contactNormal.x != 0 && contactNormal.y == 0 && (grounded || !grounded))
                // {
                //     isClimbing = false;

                //     if (structurePolarity != null)
                //     {

                //         if (polarityChangerScript.polarity != structurePolarity.polarity)
                //         {
                //             if (rbPlayer.constraints.HasFlag(RigidbodyConstraints.FreezePositionY))
                //             {
                //                 rbPlayer.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;

                //             }
                //         }
                //     }
                // }
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        //collision.gameObject.layer == floorLayer &&
        if (collision.GetContact(0).normal.y > 0 && !isRepulsing)
        {
            grounded = true;
        }

        if (!collision.collider.CompareTag("Magnetic Structure") && grounded && (canRepulse || isClimbing))
        {
            canRepulse = false;

            isClimbing = false;

        }
        //repulsion general
        if (collision.collider.CompareTag("Magnetic Structure"))
        {
            if (collision.contactCount > 0)
            {
                Vector3 contactNormal = collision.GetContact(0).normal;

                structurePolarityChanger structurePolarity = collision.collider.GetComponent<structurePolarityChanger>();

                //repulsion suelo
                if (contactNormal.x == 0 && contactNormal.y != 0 && grounded)
                {
                    if (structurePolarity != null)
                    {

                        if (polarityChangerScript.polarity == structurePolarity.polarity && canRepulse == true && !isRepulsing)
                        {
                            canRepulse = false;

                            magneticForce(structurePolarity, polarityForceStrongFloor, contactNormal);
                        }
                        // else if (polarityChangerScript.polarity != structurePolarity.polarity && canRepulse == true)
                        // {
                        // }
                    }
                }

                //repulsion pared
                else if (contactNormal.x != 0 && contactNormal.y == 0 && isClimbing)
                {
                    if (structurePolarity != null)
                    {

                        if (polarityChangerScript.polarity == structurePolarity.polarity && canRepulse == true)
                        {
                            canRepulse = false;
                            //rbPlayer.constraints &= ~RigidbodyConstraints.FreezePositionY;

                            magneticForce(structurePolarity, polarityForceStrongWall, contactNormal);
                        }
                        else if (polarityChangerScript.polarity != structurePolarity.polarity && canRepulse == true)
                        {
                            canRepulse = false;
                        }
                    }
                }
            }

        }
    }

    private void magneticForce(structurePolarityChanger structurePolarity, float magneticForce, Vector3 contactNormal)
    {
        Vector3 repulsionDirection = contactNormal.normalized;

        Debug.Log($"Repulsión en dirección: {repulsionDirection}");


        StartCoroutine(waitSeconds(repulsionDirection, magneticForce));

    }

    private IEnumerator waitSeconds(Vector3 direction, float force)
    {
        canMove = false;

        rbPlayer.linearVelocity = Vector3.zero; // resetea velocidad previa

        rbPlayer.AddForce(direction * force, ForceMode.VelocityChange);

        if (direction.x != 0 && direction.y == 0)
        {
            yield return new WaitForSeconds(0.5f);

        }
        else if (direction.x == 0 && direction.y != 0)
        {

        }
        else if (direction.x != 0 && direction.y != 0)
        {

        }

        canMove = true;

    }






    //caida fuera del mapa y reset lvl
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Limit Wall"))
        {
            resetScene.Instancia.ResetFunction(SceneManager.GetActiveScene().buildIndex);
        }
    }

    //piso traspasable
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
