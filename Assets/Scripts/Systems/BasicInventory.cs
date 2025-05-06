using UnityEngine;

public class BasicInventory : MonoBehaviour
{


    public int keyAmount;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("key"))
        {
            Destroy(other.gameObject);
            keyAmount += 1;
        }
    }
}
