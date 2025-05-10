using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class portal : MonoBehaviour
{

    [SerializeField] int keyRequiredAmount;

    [SerializeField] Scene lvlScene;

    int nextSceneIndex;
    PlayerInput playerInput;

    bool canUsePortal;

    BasicInventory inventory;
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        inventory = GetComponent<BasicInventory>();

        int actualSceneIndex = SceneManager.GetActiveScene().buildIndex;
        nextSceneIndex = actualSceneIndex + 1;
    }

    private void Update()
    {
        if (playerInput.actions["Interact"].WasPressedThisFrame())
        {
            int keyAmount = inventory.keyAmount;

            if (keyRequiredAmount == keyAmount)
            {
                resetScene.Instancia.ResetFunction(nextSceneIndex);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("portal"))
        {
            canUsePortal = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Portal"))
        {
            canUsePortal = false;
        }
    }
}
