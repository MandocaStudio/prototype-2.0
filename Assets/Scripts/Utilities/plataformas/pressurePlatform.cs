using UnityEngine;

public class pressurePlatform : MonoBehaviour
{
    [SerializeField] float velocidadMinimaRequerida;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            float impactSpeed = collision.relativeVelocity.magnitude;

            if (impactSpeed >= velocidadMinimaRequerida)
            {
                // Activar botón o acción
                Debug.Log("¡Golpe válido con velocidad: " + impactSpeed + "!");
            }
            else
            {
                Debug.Log("Golpe muy débil: " + impactSpeed);
            }
        }
    }

}
