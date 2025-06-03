using System.Collections;
using UnityEngine;

public class classRompiblePlatform : MonoBehaviour
{

    [SerializeField]
    protected enum HowToBreak
    {
        polarity,
        collision
    }

    [SerializeField] protected HowToBreak platformType;

    [SerializeField] protected float timeToBreak;

    protected bool canBreakPlatform;

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player") && platformType == HowToBreak.collision && collision.GetContact(0).normal.y < 0)
        {
            Debug.Log("entro");
            StartCoroutine(breakThePlatform());
        }

        // if (collision.collider.CompareTag("Player") && platformType == HowToBreak.polarity)
        // {

        //     if (collision.collider.GetComponent<movement>().isRepulsing == true)
        //     {
        //         StartCoroutine(breakThePlatform());
        //     }
        // }
    }

    //para plataformas que se rompen al impulsarte en ellas
    protected virtual void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Player") && platformType == HowToBreak.polarity)
        {

            if (collision.collider.GetComponent<movement>().isRepulsing == true)
            {
                StartCoroutine(breakThePlatform());
            }
        }
    }

    private IEnumerator breakThePlatform()
    {
        yield return new WaitForSeconds(timeToBreak);

        Destroy(gameObject);
    }

}
