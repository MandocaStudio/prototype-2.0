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
    [SerializeField] float maxSpeed;


    [SerializeField] bool traversing = false;

    [SerializeField] bool grounded;

    [SerializeField] bool isClimbing;

    int floorLayer;



    public bool canMove;

    [Header("variables de repulsion")]
    [SerializeField] float polarityForceWeakFloor; //6.7
    [SerializeField] float polarityForceStrongFloor; //10.3
    [SerializeField] float polarityForceWeakWall; //6.7
    [SerializeField] float polarityForceStrongWall; //10.3

    [SerializeField] bool canRepulse;

    public bool isRepulsing;

    [Header("variables de caida rapida")]

    [SerializeField] float fallMultiplier;

    [SerializeField] bool canUsefallMultiplier;

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
        //debug de fuerza de impacto eje y
        // if (!grounded)
        // {
        //     float velocidadY = rbPlayer.linearVelocity.y;
        //     Debug.Log("Velocidad en Y: " + velocidadY);
        // }

        if (playerInput.actions["traversableButton"].WasPressedThisFrame() && !traversing)
        {
            traversing = true;
        }

        if (playerInput.actions["polarityChanger"].WasPressedThisFrame())
        {
            canRepulse = true;
        }
    }



    void FixedUpdate()
    {
        if (canMove)
        {
            moveX = playerInput.actions["movement"].ReadValue<Vector2>().x;

            if (Mathf.Abs(moveX) > 0.01f)
            {
                // Calcula velocidad deseada
                float targetSpeed = moveX * speed;

                // Calcula la diferencia entre la velocidad actual y la deseada
                float speedDiff = targetSpeed - rbPlayer.linearVelocity.x;

                // Aplica fuerza proporcional solo para acercarse a la velocidad deseada
                rbPlayer.AddForce(new Vector3(speedDiff, 0, 0), ForceMode.VelocityChange);

                // Limita velocidad máxima sin anular el viento
                float clampedX = Mathf.Clamp(rbPlayer.linearVelocity.x, -maxSpeed, maxSpeed);
                rbPlayer.linearVelocity = new Vector3(clampedX, rbPlayer.linearVelocity.y, rbPlayer.linearVelocity.z);
            }
            else if (grounded)
            {
                // Frenado suave al soltar input (solo en el suelo)
                Vector3 velocity = rbPlayer.linearVelocity;
                velocity.x = Mathf.Lerp(velocity.x, 0, 0.2f); // puedes ajustar el 0.2f para más o menos frenado
                rbPlayer.linearVelocity = velocity;
            }
        }

        // Aumenta la gravedad cuando cae
        if (rbPlayer.linearVelocity.y < 0 && canUsefallMultiplier)
        {
            rbPlayer.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        //
        if (collision.gameObject.layer == floorLayer && collision.GetContact(0).normal.y > 0)
        {
            grounded = true;
            canUsefallMultiplier = true;
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
            else if (contactNormal.x != 0 && contactNormal.y != 0 && (grounded || !grounded))
            {
                if (polarityChangerScript.polarity == structurePolarity.polarity)
                {

                    magneticForce(structurePolarity, polarityForceWeakWall, contactNormal);
                }
            }

        }

    }

    void OnCollisionExit(Collision collision)
    {
        // &&collision.gameObject.layer == floorLayer
        if (collision.gameObject.layer == floorLayer)
        {
            grounded = false;

        }



        if (rbPlayer.constraints.HasFlag(RigidbodyConstraints.FreezePositionY))
        {
            canUsefallMultiplier = false;
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
                            isRepulsing = true;

                        }
                    }
                }

            }
        }
    }

    void OnCollisionStay(Collision collision)
    {

        if (!grounded && collision.gameObject.layer == floorLayer && collision.GetContact(0).normal.y > 0 && !isRepulsing)
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
                            isRepulsing = true;

                            canRepulse = false;

                            magneticForce(structurePolarity, polarityForceStrongFloor, contactNormal);
                        }

                    }
                }

                //repulsion pared
                else if (contactNormal.x != 0 && contactNormal.y == 0 && isClimbing)
                {
                    if (structurePolarity != null)
                    {

                        if (polarityChangerScript.polarity == structurePolarity.polarity && canRepulse == true)
                        {
                            isRepulsing = true;

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
        //canMove = false;

        rbPlayer.linearVelocity = Vector3.zero; // resetea velocidad previa


        float initialForceMultiplier = 1.5f; // Multiplicador para aumentar la fuerza inicial

        rbPlayer.AddForce(direction * force * initialForceMultiplier, ForceMode.VelocityChange);






        if (direction.x != 0 && direction.y == 0)
        {
            yield return new WaitForSeconds(0.2f);

        }
        else if (direction.x == 0 && direction.y != 0)
        {
            yield return new WaitForSeconds(0.25f);

        }
        else if (direction.x != 0 && direction.y != 0)
        {

        }

        // Aquí podemos reducir la velocidad progresivamente para controlar la distancia
        rbPlayer.linearVelocity = Vector3.Scale(rbPlayer.linearVelocity, new Vector3(0.5f, 0.5f, 0));

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


    //morir
    public void death()
    {
        //animacion (si tan solo la tuviera)
        resetScene.Instancia.ResetFunction(SceneManager.GetActiveScene().buildIndex);

    }
}
