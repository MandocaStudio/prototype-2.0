using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class movablePlatform : MonoBehaviour
{

    [SerializeField] Vector3 targetA;
    [SerializeField] Vector3 targetB;

    [SerializeField] float speed;

    [SerializeField] float waitTime;

    [SerializeField] bool canMove;

    [SerializeField] bool moving;

    bool toTargetB;

    bool isWaiting;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        toTargetB = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {


            if (moving)
            {
                //inicial
                if (Vector3.Distance(targetA, targetB) > 0.1f && toTargetB)
                {
                    if (Vector3.Distance(transform.position, targetB) < 0.1f)
                    {
                        moving = false;
                    }
                    transform.position = Vector3.MoveTowards(transform.position, targetB, speed * Time.deltaTime);
                }
                //retorno
                else if (Vector3.Distance(targetA, targetB) > 0.1f && !toTargetB)
                {
                    if (Vector3.Distance(transform.position, targetA) < 0.1f)
                    {
                        moving = false;
                    }
                    transform.position = Vector3.MoveTowards(transform.position, targetA, speed * Time.deltaTime);
                }

            }
            else
            {
                if (!isWaiting)
                {
                    StartCoroutine(waitSecons());
                }
            }
        }
    }

    IEnumerator waitSecons()
    {
        Debug.Log("esperando");

        if (toTargetB)
        {
            toTargetB = false;
        }
        else
        {
            toTargetB = true;
        }

        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        isWaiting = false;

        moving = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Player"))
        {
            other.transform.SetParent(this.transform);

        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.collider.CompareTag("Player"))
        {
            other.transform.SetParent(null);

        }
    }

}
