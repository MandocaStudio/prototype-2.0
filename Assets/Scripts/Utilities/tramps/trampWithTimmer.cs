using System.Collections;
using UnityEngine;

public class trampWithTimmer : tramp
{
    [SerializeField] bool active;

    [SerializeField] float inactiveTime;

    [SerializeField] float activeTime;


    private void Start()
    {
        if (tipoTrampa == trampType.trigger)
        {
            transform.GetComponent<Collider>().isTrigger = true;
        }
        else if (tipoTrampa == trampType.collider)
        {
            transform.GetComponent<Collider>().isTrigger = false;
        }


        StartCoroutine(trampActivate());
    }

    private IEnumerator trampActivate()
    {
        while (true)
        {
            active = true;
            yield return new WaitForSeconds(activeTime); // Tiempo activa
            active = false;
            yield return new WaitForSeconds(inactiveTime);
        }
    }

    protected override void OnCollisionEnter(Collision other)
    {
        if (!active || tipoTrampa != trampType.collider) return;
        base.OnCollisionEnter(other);
    }


    protected override void OnCollisionStay(Collision other)
    {
        if (!active || tipoTrampa != trampType.collider) return;
        base.OnCollisionEnter(other);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (!active || tipoTrampa != trampType.trigger) return;
        base.OnTriggerEnter(other);
    }

    protected override void OnTriggerStay(Collider other)
    {
        if (!active || tipoTrampa != trampType.trigger) return;
        base.OnTriggerEnter(other);
    }

}
