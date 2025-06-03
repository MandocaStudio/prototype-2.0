using UnityEngine;

public class windForce : MonoBehaviour
{
    //public Vector3 windDirection = Vector3.right;

    public enum windDirection
    {
        right, left, up, down
    }

    public windDirection windDirectionVar;
    public float windStrength;

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            switch (windDirectionVar)
            {
                case windDirection.right:
                    rb.AddForce(Vector3.right.normalized * windStrength, ForceMode.Force);

                    break;
                case windDirection.left:
                    rb.AddForce(Vector3.left.normalized * windStrength, ForceMode.Force);

                    break;
                case windDirection.up:
                    rb.AddForce(Vector3.up.normalized * windStrength, ForceMode.Force);

                    break;
                case windDirection.down:
                    rb.AddForce(Vector3.down.normalized * windStrength, ForceMode.Force);

                    break;
            }
        }
    }
}
