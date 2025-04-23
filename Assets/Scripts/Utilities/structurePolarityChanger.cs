using UnityEngine;

public class structurePolarityChanger : MonoBehaviour
{

    public Polarity polarity;

    Renderer structureRenderer;

    [SerializeField] bool change;

    [SerializeField] Material positive;
    [SerializeField] Material negative;

    public enum forceDirection
    {
        right,
        left,
        up,
        down
    }

    public forceDirection forceDirectionVar;
    private void Start()
    {
        structureRenderer = GetComponent<Renderer>();
        switch (polarity)
        {
            case Polarity.positive:
                structureRenderer.material = positive;
                change = false;
                break;
            case Polarity.negative:
                structureRenderer.material = negative;
                change = false;

                break;

        }
    }
    void Update()
    {
        if (change)
        {
            switch (polarity)
            {
                case Polarity.positive:
                    polarity = Polarity.negative;
                    structureRenderer.material = negative;
                    change = false;
                    break;
                case Polarity.negative:
                    polarity = Polarity.positive;
                    structureRenderer.material = positive;
                    change = false;

                    break;

            }
        }

    }


}
