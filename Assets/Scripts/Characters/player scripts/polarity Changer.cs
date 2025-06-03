using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class polarityChanger : MonoBehaviour
{
    public Polarity polarity;

    movement PlayerMovement;

    [SerializeField] bool canChancePolarity;

    Renderer PlayerRenderer;
    [SerializeField] Material positive;
    [SerializeField] Material negative;
    [SerializeField] Material neutro;

    [SerializeField] float neutroTime;
    private void Start()
    {
        PlayerMovement = GetComponent<movement>();
        PlayerRenderer = GetComponent<Renderer>();

        polarityCHanger(polarity);

    }
    void Update()
    {
        if (PlayerMovement.playerInput.actions["polarityChanger"].WasPressedThisFrame() && canChancePolarity)
        {
            polarityCHanger(polarity);
        }

        if (PlayerMovement.playerInput.actions["exitButton"].WasPressedThisFrame() && canChancePolarity)
        {
            polarityCHanger(Polarity.neutro);

        }

    }

    private void polarityCHanger(Polarity polarityVar)
    {
        switch (polarityVar)
        {
            case Polarity.positive:
                polarity = Polarity.negative;
                PlayerRenderer.material = negative;
                break;
            case Polarity.negative:
                polarity = Polarity.positive;
                PlayerRenderer.material = positive;
                break;
            case Polarity.neutro:
                StartCoroutine(changeToNeutroPolarity());
                break;

        }
    }

    private IEnumerator changeToNeutroPolarity()
    {
        Polarity lastPolarity = polarity;
        polarity = Polarity.neutro;
        PlayerRenderer.material = neutro;
        canChancePolarity = false;

        yield return new WaitForSeconds(neutroTime);

        canChancePolarity = true;
        polarityCHanger(lastPolarity);

    }
}

public enum Polarity
{
    positive,
    negative,
    neutro
}
