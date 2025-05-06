using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;


public class lvlNameAndInventoryUi : MonoBehaviour
{

    BasicInventory keyScript;

    [SerializeField] GameObject inventoryKeysObject;
    [SerializeField] int lastKey;

    [Range(0, 2)]
    [SerializeField] int maximumKeys;
    void Start()
    {


        inventoryKeysObject = transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).gameObject;

        for (int i = 2; i > maximumKeys; i--)
        {
            GameObject key = inventoryKeysObject.transform.GetChild(i).gameObject;
            key.SetActive(false);
        }

        keyScript = transform.GetComponentInParent<BasicInventory>();

        lastKey = keyScript.keyAmount;

        string sceneName = SceneManager.GetActiveScene().name;
        string lastTwoChars = sceneName.Substring(sceneName.Length - 2);

        TextMeshProUGUI lvlName = transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();

        lvlName.text = "LV." + lastTwoChars;
    }

    // Update is called once per frame
    void Update()
    {
        if (lastKey < keyScript.keyAmount)
        {
            Debug.Log("entro");
            GameObject key = inventoryKeysObject.transform.GetChild(lastKey).GetChild(0).gameObject;
            key.SetActive(true);

            lastKey = keyScript.keyAmount;
        }
    }
}



