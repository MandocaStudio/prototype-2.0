using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIIconManager : MonoBehaviour
{
    public TMP_SpriteAsset keyboardIcons;
    public TMP_SpriteAsset xboxIcons;
    public TMP_SpriteAsset playstationIcons;

    // public PlayerInput playerInput;


    private string currentControlScheme;

    // private void Awake()
    // {
    //     playerInput = GetComponentInParent<PlayerInput>();

    //     if (playerInput == null)
    //         Debug.LogWarning("No se encontró PlayerInput en el padre del objeto UIIconManager.");
    // }

    void OnEnable()
    {
        InputSystem.onActionChange += HandleDeviceChange;
    }

    void OnDisable()
    {
        InputSystem.onActionChange -= HandleDeviceChange;
    }

    void HandleDeviceChange(object action, InputActionChange change)
    {
        if (change == InputActionChange.ActionStarted)
        {
            var scheme = PlayerInput.all[0].currentControlScheme;

            Debug.Log(scheme);

            if (scheme != currentControlScheme)
            {
                currentControlScheme = scheme;
                CambiarSpriteAssetGlobalmente(scheme);
            }
        }
    }

    void CambiarSpriteAssetGlobalmente(string scheme)
    {
        TMP_SpriteAsset nuevoAsset = keyboardIcons;

        if (scheme == "Gamepad")
            nuevoAsset = xboxIcons;
        else if (scheme == "DualShockGamepad")
            nuevoAsset = playstationIcons;

        // Aplica a UI
        foreach (var tmpUI in FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None))
        {
            tmpUI.spriteAsset = nuevoAsset;
            tmpUI.ForceMeshUpdate();
        }

        // Aplica a textos 3D
        foreach (var tmp3D in FindObjectsByType<TextMeshPro>(FindObjectsSortMode.None))
        {
            tmp3D.spriteAsset = nuevoAsset;
            tmp3D.ForceMeshUpdate();
        }
    }
}
