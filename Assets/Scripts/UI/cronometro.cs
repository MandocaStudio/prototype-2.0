using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class cronometro : MonoBehaviour
{
    public GameObject[] barritas;

    [SerializeField] GameObject barritasFather;

    public float currentTime;
    public float totalTime;

    float lastPercentageStep = 100f;

    int currentBarIndex;
    [SerializeField] Color32 greenColor;
    [SerializeField] Color32 redColor;
    [SerializeField] Color32 yellowColor;
    [SerializeField] Color32 withoutColor;

    [SerializeField] bool greenTime;
    [SerializeField] bool yellowTime;
    [SerializeField] bool redTime;

    public bool timeRuning;

    void Start()
    {
        greenColor = new Color32(132, 255, 71, 255);
        redColor = new Color32(255, 0, 33, 255);
        yellowColor = new Color32(255, 240, 55, 255);
        withoutColor = new Color32(79, 85, 100, 255);

        barritasFather = transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        int cantidad = barritasFather.transform.childCount;
        barritas = new GameObject[cantidad];


        for (int i = 0; i < cantidad; i++)
        {
            barritas[i] = barritasFather.transform.GetChild(i).gameObject;
            barritas[i].GetComponent<Image>().color = greenColor;
        }


        currentTime = totalTime;
        currentBarIndex = cantidad - 1;
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

            float percentage = (currentTime / totalTime) * 100f;

            if (percentage <= 60f && yellowTime)
            {
                for (int i = 0; i < currentBarIndex; i++)
                {
                    barritas[i].GetComponent<Image>().color = yellowColor;
                }
                yellowTime = false;
            }

            if (percentage <= 30f && redTime)
            {
                for (int i = 0; i < currentBarIndex; i++)
                {
                    barritas[i].GetComponent<Image>().color = redColor;
                }
                redTime = false;
            }

            // Eliminar una barrita cada 2.5% de pérdida
            if (percentage <= lastPercentageStep - 2.5f && currentBarIndex >= 0)
            {
                // Cambia el color antes de eliminar visualmente
                barritas[currentBarIndex].GetComponent<Image>().color = withoutColor;

                currentBarIndex--;
                lastPercentageStep -= 2.5f;

            }
        }

        else if (currentTime <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
