using UnityEngine;

public class polarityChanger : MonoBehaviour
{
    [SerializeField] Polarity polarity;

    movement PlayerMovement;

    [SerializeField] bool canChancePolarity;

    Renderer PlayerRenderer;
    [SerializeField] Material positive;
    [SerializeField] Material negative;

    private void Start()
    {
        PlayerMovement = GetComponent<movement>();
        PlayerRenderer = GetComponent<Renderer>();

        switch (polarity)
        {
            case Polarity.positive:
                PlayerRenderer.material = positive;
                break;
            case Polarity.negative:
                PlayerRenderer.material = negative;
                break;
        }
    }
    void Update()
    {
        if (PlayerMovement.playerInput.actions["polarityChanger"].WasPressedThisFrame() && canChancePolarity)
        {
            switch (polarity)
            {
                case Polarity.positive:
                    polarity = Polarity.negative;
                    PlayerRenderer.material = negative;
                    break;
                case Polarity.negative:
                    polarity = Polarity.positive;
                    PlayerRenderer.material = positive;
                    break;
            }

        }



    }
}

public enum Polarity
{
    positive,
    negative
}
