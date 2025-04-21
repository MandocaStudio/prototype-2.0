using UnityEngine;

public class inactivateCollider : MonoBehaviour
{
    [SerializeField] bool inactivated = false;
    public bool canInactivate = true;

    Collider colliderSon;
    private void Start()
    {
        colliderSon = transform.GetChild(0).GetComponent<Collider>();
    }
    public void inactivateSon()
    {
        if (canInactivate)
        {
            if (!inactivated)
            {
                inactivated = true;
                colliderSon.enabled = false;
            }
            else
            {
                inactivated = false;
                colliderSon.enabled = true;
            }
        }

    }
}
