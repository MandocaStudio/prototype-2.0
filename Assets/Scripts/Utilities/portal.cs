using UnityEngine;
using UnityEngine.InputSystem;

public class portal : MonoBehaviour
{

    [SerializeField] int keyRequiredAmount;


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInput playerInput = other.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                if (playerInput.actions["Interact"].WasPressedThisFrame())
                {
                    int keyAmount = other.GetComponent<BasicInventory>().keyAmount;

                    if (keyRequiredAmount == keyAmount)
                    {
                        Debug.Log("hacia el otro nivel");
                    }
                }
            }

        }
    }
}
