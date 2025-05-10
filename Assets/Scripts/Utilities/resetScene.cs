using UnityEngine;
using UnityEngine.SceneManagement;

public class resetScene : MonoBehaviour
{

    public static resetScene Instancia { get; private set; }

    private void Awake()
    {
        Debug.LogError("¡Ya existe una instancia de AdministradorGlobal!");

        if (Instancia == null)
        {
            Instancia = this;
            // Opcional: Evitar que se destruya al cargar nuevas escenas
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogError("¡Ya existe una instancia de AdministradorGlobal!");
            Destroy(gameObject);
        }
    }

    public void ResetFunction(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
