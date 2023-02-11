using UnityEngine;

public class Bread : MonoBehaviour
{
    public GameObject cage;

    public bool locked {
        get
        {
            return cage.activeInHierarchy;
        }

        set
        {
            cage.SetActive(value);
        }
    }
}
