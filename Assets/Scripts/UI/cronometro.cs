using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class cronometro : MonoBehaviour
{
    Image fillImage;

    public float currentTime;
    public float totalTime;

    [SerializeField] Material greenMaterial;
    [SerializeField] Material redMaterial;
    [SerializeField] Material yellowMaterial;

    [SerializeField] bool greenTime;
    [SerializeField] bool yellowTime;
    [SerializeField] bool redTime;

    public bool timeRuning;


    void Start()
    {
        fillImage = transform.GetComponent<Image>();

        fillImage.material = greenMaterial;

        currentTime = totalTime;

        timeRuning = true;

        greenTime = true;
        yellowTime = true;
        redTime = true;
    }

    void Update()
    {
        if (currentTime > 0 && timeRuning)
        {
            currentTime -= Time.deltaTime;

            fillImage.fillAmount = currentTime / totalTime;

            float percentage = (currentTime / totalTime) * 100f;

            if (percentage <= 60f && yellowTime)
            {
                yellowTime = false;
                fillImage.material = yellowMaterial;
            }

            if (percentage <= 30f && redTime)
            {
                redTime = false;
                fillImage.material = redMaterial;
            }
        }

        else if (currentTime <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
