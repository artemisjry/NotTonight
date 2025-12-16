using UnityEngine;

public class InspectableObject : MonoBehaviour
{
    [HideInInspector]
    public bool inspected = false;


    public void Inspect()
    {
        inspected = true;

        Debug.Log($"{gameObject.name} inspected!");
    }
}
