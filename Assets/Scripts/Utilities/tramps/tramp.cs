using System.Collections; // Necesario para IEnumerator
using UnityEngine;

public class tramp : MonoBehaviour
{
    [SerializeField]
    protected enum trampType
    {
        trigger,
        collider
    }

    [SerializeField] protected trampType tipoTrampa;

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            movement playerScript = collision.collider.GetComponent<movement>();

            playerScript.death();
        }
    }

    protected virtual void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            movement playerScript = collision.collider.GetComponent<movement>();

            playerScript.death();
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            movement playerScript = other.GetComponent<movement>();

            playerScript.death();
        }
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            movement playerScript = other.GetComponent<movement>();

            playerScript.death();
        }
    }

}
